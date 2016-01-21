﻿using eBay.Services.Finding;
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
        //(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private CancellationTokenSource _cts;
        private Action<Task> _searchTask;
        private List<Search> _searches;
        private int _currentSearchIdx;
        private TimeSpan _delay;
        private IEbayFinder finder;


        public SearchMonkeyService()
        {
            _cts = new CancellationTokenSource();

            _currentSearchIdx = 0;

            _searchTask = t =>
            {
                ExecuteSearch(_searches[_currentSearchIdx++]);

                if (_currentSearchIdx == _searches.Count)
                    _currentSearchIdx = 0;

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

        private void ExecuteSearch(Search search)
        {
            try
            {

                if ((DateTime.Now - search.LastRun).TotalHours < 24) return;

                using (var repo = new SearchRepository())
                {
                    _log.Info($"Processing {search.Name}");

                    // Create a request to get our completed items
                    var response = finder.GetCompletedItems(search.Name, search.LastRun);

                    // Show output
                    if (response.ack == AckValue.Success || response.ack == AckValue.Warning)
                    {
                        _log.Info("Found " + response.searchResult.count + " new items");

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
            catch(Exception ex)
            {
                _log.ErrorFormat("Error: ", ex);
            }
        }

        public void Start()
        {
            // Get all searches
            _searches = GetSearches();

            _delay = CalculateDelay();

            // Kick off a search
            Task.Delay(100, _cts.Token).ContinueWith(_searchTask, _cts.Token);

            // When the search has completed, schedule the next one based on a calculated delay
        }

        private TimeSpan CalculateDelay()
        {
            return TimeSpan.FromMilliseconds(10000);
        }

        private List<Search> GetSearches()
        {
            using (var repo = new SearchRepository())
            {
                return (List<Search>)repo.GetAllSearches();
            }
        }

        public void Stop()
        {
            _cts.Cancel();
            _cts.Token.WaitHandle.WaitOne();
            _log.Info("Stopped");
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
