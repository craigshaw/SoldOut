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

namespace SoldOutSearchMonkey.Service
{
    public class SearchMonkeyService
    {
        private const int ApiDailyRateLimit = 5000;
        private static readonly ILog _log = LogManager.GetLogger(typeof(SearchMonkeyService));

        private CancellationTokenSource _cts;
        private Action<Task> _searchTask;
        private long _currentSearchId;
        private double _delay;
        private int _errorCount;
        private readonly IEbayFinder _finder;
        private readonly INotifier _notifier;

        private readonly Counter resultCounter = Metric.Counter("Results Harvested", Unit.Custom("Results"));
        private readonly Metrics.Timer timer = Metric.Timer("eBay Service Requests", Unit.Requests);

        public SearchMonkeyService(IEbayFinder finder, INotifier notifier)
        {
            _cts = new CancellationTokenSource();

            _finder = finder;
            _notifier = notifier;

            _searchTask = t =>
            {
                Stopwatch timer = Stopwatch.StartNew();

                ExecuteSearch();

                timer.Stop();

                Task.Delay(TimeSpan.FromMilliseconds(_delay - timer.ElapsedMilliseconds), _cts.Token)
                            .ContinueWith(ta => _searchTask(t), _cts.Token);
            };

            // Register gauges
            Metric.Gauge("Error Count", () => _errorCount, Unit.Errors);
        }

        private void ExecuteSearch()
        {
            FindCompletedItemsResponse response;

            try
            {
                using (var repo = new SearchRepository())
                {
                    var search = repo.GetNextSearch(_currentSearchId);
                    _currentSearchId = search.SearchId;

                    _log.Info($"Running search for {search.Name}...");

                    // Run the search
                    using (timer.NewContext())
                    {
                        response = _finder.GetCompletedItems(search);
                    }

                    // Show output
                    if (response.ack == AckValue.Success || response.ack == AckValue.Warning)
                    {
                        _log.Info($"Found {response.searchResult.count} new items");

                        // Set the last ran time
                        search.LastRun = response.timestamp;

                        int numResults = response.searchResult.count;

                        if (numResults > 0)
                        {
                            // Map returned items to our SoldItems model
                            var newItems = eBayMapper.MapSearchItemsToSearchResults(response.searchResult.item);

                            // Add them to the relevant search
                            repo.AddSearchResults(search.SearchId, newItems);

                            // Send a notification out
                            NotifyResultsReady(search, numResults);

                            // Add to the total harvested
                            resultCounter.Increment(search.Description, numResults);
                        }

                        repo.SaveAll();
                    }
                    else
                    {
                        LogEbayError(response);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error: {0}", ex);
                _errorCount++;
            }
        }

        private void LogEbayError(FindCompletedItemsResponse response)
        {
            StringBuilder errorMessage = new StringBuilder("Errors occured when calling the eBay API:" + Environment.NewLine);

            _errorCount++;

            if(response.errorMessage != null && response.errorMessage.Length > 0)
            {
                foreach(var error in response.errorMessage)
                {
                    errorMessage.AppendLine($"{error.exceptionId}, {error.errorId} - {error.message}");
                }
            }
        }

        private void NotifyResultsReady(Search search, int numResults)
        {
#if (DEBUG==false)
            _notifier.PostMessage($"I've just logged {numResults} new search results for {search.Name} (<{search.Link}|{search.Description}>)");
#endif
        }

        public void Start()
        {
            // Reset the search we last ran
            _currentSearchId = 0;

            // Calculate the delay between searches
            _delay = CalculateDelay();

            // Kick off the first search
            Task.Delay(100, _cts.Token).ContinueWith(_searchTask, _cts.Token);

            _log.Info($"SearchMonkey v{Version} Started");
#if (DEBUG == false)
            _notifier.PostMessage("SearchMonkey Started");
#endif
        }

        private Version Version
        {
            get { return Assembly.GetEntryAssembly().GetName().Version; }
        }

        private double CalculateDelay()
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
