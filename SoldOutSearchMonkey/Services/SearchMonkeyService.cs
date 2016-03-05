﻿using eBay.Services.Finding;
using log4net;
using Metrics;
using SoldOutBusiness.Mappers;
using SoldOutBusiness.Models;
using SoldOutBusiness.Repository;
using SoldOutBusiness.Services;
using SoldOutBusiness.Services.Notifiers;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using SoldOutSearchMonkey.Factories;
using SoldOutBusiness.Utilities.Conditions;

namespace SoldOutSearchMonkey.Services
{
    internal class SearchMonkeyService
    {
        private const int ApiDailyRateLimit = 5000;
        private static readonly ILog _log = LogManager.GetLogger(typeof(SearchMonkeyService));

        private readonly CancellationTokenSource _cts;
        private readonly Action<Task> _searchTask;
        private long _currentSearchId;
        private int _errorCount;
        private Counter _resultCounter;
        private Metrics.Timer _serviceRequestTimer;
        private readonly IEbayFinder _finder;
        private readonly INotifier _notifier;
        private readonly ISearchRepositoryFactory _repoFactory;
        private readonly ICompletedItemReviewer _completedItemReviewer;

        public SearchMonkeyService(IEbayFinder finder, INotifier notifier, ISearchRepositoryFactory repoFactory,
                                   ICompletedItemReviewer completedItemReviewer)
        {
            if (finder == null)
                throw new ArgumentNullException(nameof(finder));

            if (notifier == null)
                throw new ArgumentNullException(nameof(notifier));

            if (repoFactory == null)
                throw new ArgumentNullException(nameof(repoFactory));

            if (completedItemReviewer == null)
                throw new ArgumentNullException(nameof(completedItemReviewer));

            _finder = finder;
            _notifier = notifier;
            _repoFactory = repoFactory;
            _completedItemReviewer = completedItemReviewer;

            _cts = new CancellationTokenSource();

            _searchTask = t =>
            {
                // Execute current search
                var executionTime = ExecuteSearch();

#if DEBUG
                executionTime = 0;
#endif

                // Schedule the next search
                Task.Delay(TimeSpan.FromMilliseconds(SearchScheduleInterval - executionTime), _cts.Token)
                            .ContinueWith(ta => _searchTask(t), _cts.Token);
            };

            InstallInstrumentation();
        }

        // TODO: Would like to see all the instrumentation stuff behind its own service interface
        private void InstallInstrumentation()
        {
            // Counters, timers, etc
            _resultCounter = Metric.Counter("Results Harvested", Unit.Custom("Results"));
            _serviceRequestTimer = Metric.Timer("eBay Service Requests", Unit.Requests);

            // Register gauges
            Metric.Gauge("Error Count", () => _errorCount, Unit.Errors);
            Metric.Gauge("Uptime", () => (DateTime.Now - StartTime).TotalHours, Unit.Custom("Hours"));
        }

        #region Properties
        private DateTime StartTime { get; set; }
        public double SearchScheduleInterval { get; set; }
        #endregion

        private long ExecuteSearch()
        {
            Stopwatch executionTimer = Stopwatch.StartNew();
            FindCompletedItemsResponse response;

            try
            {
                using (var repo = _repoFactory.CreateSearchRepository())
                {
                    var conditions = repo.GetConditions();
                    var search = repo.GetNextSearch(_currentSearchId);
                    _currentSearchId = search.SearchId;

                    if ((DateTime.Now - search.LastRun).TotalHours < 24)
                    {
                        _log.Debug($"No update for {search.Name} required. Last ran at {search.LastRun}");
                    }
                    else
                    {
                        _log.Info($"Running search for {search.Name}...");

                        // Run the search
                        using (_serviceRequestTimer.NewContext())
                        {
                            response = _finder.GetCompletedItems(search);
                        }

                        // Process results
                        if (response.ack == AckValue.Success || response.ack == AckValue.Warning)
                        {
                            _log.Info($"Found {response.searchResult.count} new items");

                            // Set the last ran time
                            search.LastRun = response.timestamp;

                            int numResults = response.searchResult.count;

                            if (numResults > 0)
                            {
                                // Map returned items to our SoldItems model
                                var newItems = eBayMapper.MapSearchItemsToSearchResults(response.searchResult.item, new ConditionResolver(conditions)).ToList();

                                // Review for any suspicous results
                                var reviewSummary = _completedItemReviewer.ReviewCompletedItems(newItems, repo.GetPriceStatsForSearch(search.SearchId), search.SuspiciousPhrases);

                                _log.Info($"{reviewSummary.SuspiciousItems.Count} look suspicious");

                                // Add them to the relevant search
                                repo.AddSearchResults(search.SearchId, newItems);

                                // Send a notification out
                                NotifyResultsReady(search, numResults, reviewSummary.SuspiciousItems.Count);

                                // Add to the total harvested
                                _resultCounter.Increment(search.Description, numResults);
                            }

                            repo.SaveAll();
                        }
                        else
                        {
                            LogEBayError(response);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error: {0}", ex);
                _errorCount++;
            }

            executionTimer.Stop();

            return executionTimer.ElapsedMilliseconds;
        }

        private void LogEBayError(FindCompletedItemsResponse response)
        {
            StringBuilder errorMessage = new StringBuilder("Errors occured when calling the eBay API:" + Environment.NewLine);

            _errorCount++;

            if (response.errorMessage != null && response.errorMessage.Length > 0)
            {
                foreach (var error in response.errorMessage)
                {
                    errorMessage.AppendLine($"{error.exceptionId}, {error.errorId} - {error.message}");
                }
            }

            _log.Error(errorMessage);
            _notifier.PostMessage($"The eBay API has given me an error: {errorMessage}");
        }

        private void NotifyResultsReady(Search search, int numResults, int numSuspiciousResults)
        {
#if (DEBUG == false)
            _notifier.PostMessage($"I've just logged {numResults} new search results for {search.Name} (<{search.Link}|{search.Description}>). {numSuspiciousResults} of them look suspicious");
#endif
        }

        public void Start()
        {
            // Reset the search we last ran
            _currentSearchId = 0;

            // Calculate the delay between searches
            SearchScheduleInterval = CalculateScheduleInterval();

            // Kick off the first search
            Task.Delay(100, _cts.Token).ContinueWith(_searchTask, _cts.Token);

            // Mark start time
            StartTime = DateTime.Now;

            _log.Info($"SearchMonkey v{Version} Started");
#if (DEBUG == false)
            _notifier.PostMessage("SearchMonkey Started");
#endif
        }

        private Version Version
        {
            get { return Assembly.GetEntryAssembly().GetName().Version; }
        }

        private double CalculateScheduleInterval()
        {
            // The interval that would allow us 99% of our daily allowance (in milliseconds)
            return ((24 * 60 * 60) / (ApiDailyRateLimit * 0.99)) * 1000;
        }

        public void Stop()
        {
            _cts.Cancel();
            _cts.Token.WaitHandle.WaitOne();
            _log.Info("SearchMonkey Stopped");
#if (DEBUG == false)
            _notifier.PostMessage("SearchMonkey Stopped");
#endif
        }
    }
}