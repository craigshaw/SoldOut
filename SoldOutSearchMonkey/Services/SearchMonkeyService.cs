using eBay.Services.Finding;
using log4net;
using Metrics;
using SoldOutBusiness.Mappers;
using SoldOutBusiness.Models;
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
using System.Collections.Generic;
using System.Configuration;

namespace SoldOutSearchMonkey.Services
{
    internal class ConditionalSummary
    {
        public string Condition { get; set; }
        public int Total { get; set; }
        public int Suspicious { get; set; }
    }

    class SearchMetrics
    {
        public long ExecutionTime { get; set; }
        public int ApiCallsMade { get; set; }
    }

    class SearchSummary
    {
        public SearchSummary()
        {
            Summary = new List<ConditionalSummary>();
        }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
        public int TotalResults { get; set; }
        public IList<ConditionalSummary> Summary { get; set; }
    }

    internal class SearchMonkeyService
    {
        private readonly int _apiDailyRateLimit;
        private readonly double _apiRateThreshold;
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
        private IConditionResolver _conditionResolver;
        private IResultAggregator _resultAggregator;

        public SearchMonkeyService(IEbayFinder finder, INotifier notifier, ISearchRepositoryFactory repoFactory,
                                   ICompletedItemReviewer completedItemReviewer, IResultAggregator resultAggregator)
        {
            if (finder == null)
                throw new ArgumentNullException(nameof(finder));

            if (notifier == null)
                throw new ArgumentNullException(nameof(notifier));

            if (repoFactory == null)
                throw new ArgumentNullException(nameof(repoFactory));

            if (completedItemReviewer == null)
                throw new ArgumentNullException(nameof(completedItemReviewer));

            if (resultAggregator == null)
                throw new ArgumentNullException(nameof(resultAggregator));

            // Config settings
            _apiDailyRateLimit = Int32.Parse(ConfigurationManager.AppSettings["ApiDailyRateLimit"]);
            _apiRateThreshold = double.Parse(ConfigurationManager.AppSettings["ApiRateThreshold"]);

            _finder = finder;
            _notifier = notifier;
            _repoFactory = repoFactory;
            _completedItemReviewer = completedItemReviewer;
            _resultAggregator = resultAggregator;

            _cts = new CancellationTokenSource();

            _searchTask = t =>
            {
                try
                {
                    // Execute current search
                    var searchMetrics = ExecuteSearch();

                    // Schedule the next search
                    var nextSearchInterval = (SearchScheduleInterval * ((searchMetrics.ApiCallsMade) == 0 ? 1 : searchMetrics.ApiCallsMade)) - searchMetrics.ExecutionTime;
                    Task.Delay(TimeSpan.FromMilliseconds(nextSearchInterval >= 0 ? nextSearchInterval : 0), _cts.Token)
                                .ContinueWith(ta => _searchTask(t), _cts.Token);
                }
                catch(Exception ex)
                {
                    _log.Fatal(ex);
                }
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

        private SearchMetrics ExecuteSearch()
        {
            Stopwatch executionTimer = Stopwatch.StartNew();
            FindCompletedItemsResponse response;
            SearchMetrics searchMetrics = new SearchMetrics();

            try
            {
                using (var repo = _repoFactory.CreateSearchRepository())
                {
                    var search = repo.GetNextSearch(_currentSearchId);
                    _currentSearchId = search.SearchId;

                    if ((DateTime.Now - search.LastRun).TotalHours < 24)
                    {
                        _log.Debug($"No update for {search.Name} required. Last ran at {search.LastRun}");
                    }
                    else
                    {
                        _log.Info($"Running search for {search.Name}...");

                        var searchSummary = CreateSearchSummary(search);

                        // Run the search
                        int pageNumber = 1;
                        int foundItemCount = 0;
                        IEnumerable<SoldOutBusiness.Models.SearchResult> foundItems = null;
                        do
                        {
                            using (_serviceRequestTimer.NewContext())
                            {
                                response = _finder.GetCompletedItems(search, pageNumber++);
                                searchMetrics.ApiCallsMade++;
                            }

                            if (response.ack == AckValue.Success || response.ack == AckValue.Warning)
                            {
                                if (response.searchResult.count > 0)
                                {
                                    foundItemCount += response.searchResult.count;

                                    var items = eBayMapper.MapSearchItemsToSearchResults(response.searchResult.item, _conditionResolver, search.ProductId).ToList();

                                    foundItems = (foundItems == null) ? items : foundItems.Concat(items);
                                }
                            }
                        } while (response.paginationOutput.pageNumber < response.paginationOutput.totalPages);

                        // Process results
                        if (response.ack == AckValue.Success || response.ack == AckValue.Warning)
                        {
                            _log.Info($"Found {foundItemCount} new items");

                            // Set the last ran time
                            search.LastRun = response.timestamp;

                            searchSummary.TotalResults = foundItemCount;

                            if (searchSummary.TotalResults > 0)
                            {
                                // Get list of all the condition types in each result
                                var conditionsInResults = foundItems.Select(i => i.ConditionId).Distinct();

                                foreach (var condition in conditionsInResults)
                                {
                                    var filteredItems = foundItems.Where(i => i.ConditionId == condition);

                                    // Review for any suspicous results
                                    var reviewSummary = _completedItemReviewer.ReviewCompletedItems(filteredItems, repo.GetPriceStatsForSearchMonkeySuspiciousItemReviewer(search.SearchId, condition), search.SuspiciousPhrases);

                                    searchSummary.Summary.Add(new ConditionalSummary()
                                    {
                                        Condition = _conditionResolver.ConditionDescriptionFromConditionId(condition),
                                        Suspicious = reviewSummary.SuspiciousItems.Count,
                                        Total = filteredItems.Count()
                                    });
                                }

                                // Add them to the relevant search
                                repo.AddSearchResults(search.SearchId, foundItems);

                                ReportOnResults(searchSummary);

                                // Add to the total harvested
                                _resultCounter.Increment(search.Description, searchSummary.TotalResults);
                            }

                            // Aggregate
                            _resultAggregator.Add(searchSummary);

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

            searchMetrics.ExecutionTime = executionTimer.ElapsedMilliseconds;

            return searchMetrics;
        }

        private SearchSummary CreateSearchSummary(Search search)
        {
            return new SearchSummary()
            {
                Name = search.Name,
                Description = search.Description,
                Link = search.Link
            };
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

        private void ReportOnResults(SearchSummary summary)
        {
            // Log something
            StringBuilder notification = new StringBuilder($"I've just logged {summary.TotalResults} new search results for {summary.Name} (<{summary.Link}|{summary.Description}>). ");

            foreach (var condition in summary.Summary)
            {
                notification.Append($"{condition.Condition} - {condition.Total} total, {condition.Suspicious} suspicious. ");
            }

            _log.Info(notification.ToString());
        }

        public void Start()
        {
            // Reset the search we last ran
            _currentSearchId = 0;

            // Calculate the delay between searches
            SearchScheduleInterval = CalculateScheduleInterval();

            // Create the condition resolver
            _conditionResolver = CreateConditionResolver();

            // Kick off the first search
            Task.Delay(100, _cts.Token).ContinueWith(_searchTask, _cts.Token);

            // Mark start time
            StartTime = DateTime.Now;

            // Start the result notifier
            _resultAggregator.Start();

            _log.Info($"SearchMonkey v{Version} Started");
#if (DEBUG == false)
            _notifier.PostMessage($"SearchMonkey v{Version} Started");
#endif
        }

        private IConditionResolver CreateConditionResolver()
        {
            using (var repo = _repoFactory.CreateSearchRepository())
            {
                return new ConditionResolver(repo.GetConditions());
            }
        }

        private Version Version
        {
            get { return Assembly.GetEntryAssembly().GetName().Version; }
        }

        private double CalculateScheduleInterval()
        {
            // The interval that would allow us a percentage (defined by _apiRateThreshold) of our daily allowance (in milliseconds)
            return ((24 * 60 * 60) / (_apiDailyRateLimit * _apiRateThreshold)) * 1000;
        }

        public void Stop()
        {
            _cts.Cancel();
            _cts.Token.WaitHandle.WaitOne();
            _resultAggregator.Stop();
            _log.Info("SearchMonkey Stopped");
#if (DEBUG == false)
            _notifier.PostMessage("SearchMonkey Stopped");
#endif
        }
    }
}
