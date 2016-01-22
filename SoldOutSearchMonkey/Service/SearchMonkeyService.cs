using eBay.Services.Finding;
using log4net;
using SoldOutBusiness.Models;
using SoldOutBusiness.Repository;
using SoldOutBusiness.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SoldOutSearchMonkey.Service
{
    public class SearchMonkeyService
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(SearchMonkeyService));

        private CancellationTokenSource _cts;
        private Action<Task> _searchTask;
        private long _currentSearchId;
        private TimeSpan _delay;
        private IEbayFinder finder;


        public SearchMonkeyService()
        {
            _cts = new CancellationTokenSource();

            _searchTask = t =>
            {
                ExecuteSearch();

                Task.Delay(_delay, _cts.Token).ContinueWith(ta => _searchTask(t), _cts.Token);
            };

            finder = new EbayFinder()
                            .Configure(c =>
                            {
                                // Initialize service end-point configuration
                                c.EndPointAddress = "http://svcs.ebay.com/services/search/FindingService/v1";
                                c.GlobalId = "EBAY-GB";
                                // set eBay developer account AppID here!
                                c.ApplicationId = ConfigurationManager.AppSettings["eBayApplicationId"];
                            });
        }

        private void ExecuteSearch()
        {
            try
            {
                using (var repo = new SearchRepository())
                {
                    var search = repo.GetNextSearch(_currentSearchId);
                    _currentSearchId = search.SearchId;

                    if ((DateTime.Now - search.LastRun).TotalHours < 24)
                    {
                        _log.Debug($"No update for {search.Name} required. Last ran at {search.LastRun}");
                        return;
                    }

                    _log.Info($"Running search for {search.Name}...");

                    // Create a request to get our completed items
                    var response = finder.GetCompletedItems(search.Name, search.LastRun);

                    // Show output
                    if (response.ack == AckValue.Success || response.ack == AckValue.Warning)
                    {
                        _log.Info($"Found {response.searchResult.count} new items");

                        // Set the last ran time
                        search.LastRun = response.timestamp;

                        if (response.searchResult.count > 0)
                        {
                            // Map returned items to our SoldItems model
                            var newItems = MapSearchResults(response.searchResult.item);

                            // Add them to the relevant search
                            repo.AddSearchResults(search.SearchId, newItems);
                        }

                        repo.SaveAll();
                    }
                }
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error: ", ex);
            }
        }

        public void Start()
        {
            // Reset the search we last ran
            _currentSearchId = 0;

            // Calculate the delay between searches
            _delay = CalculateDelay();

            // Kick off the first search
            Task.Delay(100, _cts.Token).ContinueWith(_searchTask, _cts.Token);

            _log.Info("Service Started");
        }

        private TimeSpan CalculateDelay()
        {
            return TimeSpan.FromMilliseconds(10000);
        }

        public void Stop()
        {
            _cts.Cancel();
            _cts.Token.WaitHandle.WaitOne();
            _log.Info("Service Stopped");
        }

        private IEnumerable<SoldOutBusiness.Models.SearchResult> MapSearchResults(SearchItem[] items)
        {
            return items.Select(i => new SoldOutBusiness.Models.SearchResult()
            {
                DateOfMatch = DateTime.Now,
                Link = i.viewItemURL,
                Title = i.title,
                Price = i.sellingStatus.currentPrice.Value,
                ItemNumber = i.itemId,
                StartTime = i.listingInfo.endTime,
                EndTime = i.listingInfo.endTime,
                NumberOfBidders = i.listingInfo.listingType.ToLowerInvariant() == "auction" ? i.sellingStatus.bidCount : 0,
                ImageURL = i.galleryURL,
                Currency = i.sellingStatus.currentPrice.currencyId,
                Location = i.location,
                SiteID = i.globalId,
                Type = i.listingInfo.listingType,
            });
        }
    }
}
