using eBay.Services.Finding;
using SoldOutBusiness.Models;
using SoldOutBusiness.Repository;
using SoldOutBusiness.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace SoldOutHarness
{
    class Harness
    {
        static void Main(string[] args)
        {
            new Harness().Run();

            Console.WriteLine("Press any key to close the program.");
            Console.ReadKey();

        }

        private void Run()
        {
            try
            {
                var repo = new SearchRepository();

                // Create the search catalogue
                var searches = repo.GetAllSearchesWithSearchCriteria();

                var finder = new EbayFinder()
                    .Configure(c =>
                   {
                       // Initialize service end-point configuration
                       c.EndPointAddress = "http://svcs.ebay.com/services/search/FindingService/v1";
                       c.GlobalId = "EBAY-GB";
                       // set eBay developer account AppID here!
                       c.ApplicationId = ConfigurationManager.AppSettings["eBayApplicationId"];
                   });

                foreach (var search in searches)
                {
                    Console.WriteLine("Requesting completed items for '{0}' {1}",
                        search.Name,
                        (search.LastRun != null) ? "since " + search.LastRun.ToString() : string.Empty);

                    // Create a request to get our completed items
                    var response = finder.GetCompletedItems(search.Name, search.LastRun);

                    // Show output
                    if (response.ack == AckValue.Success || response.ack == AckValue.Warning)
                    {
                        Console.WriteLine("Found " + response.searchResult.count + " new items");

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
                    else
                    {
                        Console.WriteLine(string.Format("Request failed: {0}", response.ack.ToString()));

                        response.errorMessage.ToList().ForEach(e => Console.WriteLine(e.message));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
            }

        }

        private void WriteItemsToFile(string fileName, DateTime updateTime, IEnumerable<SoldItem> items)
        {
            MemoryStream memorystream = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();

            bf.Serialize(memorystream, updateTime);
            bf.Serialize(memorystream, items.ToList());

            File.WriteAllBytes(fileName, memorystream.ToArray());
        }

        private IEnumerable<SoldItem> LoadItemsFromFile(string fileName)
        {
            if(File.Exists(fileName))
            {
                MemoryStream memorystream = new MemoryStream(File.ReadAllBytes(fileName));
                BinaryFormatter bf = new BinaryFormatter();
                //_lastUpdatedTime = (DateTime)bf.Deserialize(memorystream);
                return (List<SoldItem>)bf.Deserialize(memorystream);
            }

            return null;
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

        private FindCompletedItemsRequest CreateCompletedItemsRequest(string keywords, DateTime? lastUpdated)
        {
            // Completed items, new only
            var request = new FindCompletedItemsRequest();

            // Search term
            request.keywords = keywords;

            // Filters to specify new, sold and GBP
            List<ItemFilter> filters = new List<ItemFilter>()
            {
                    new ItemFilter() { name = ItemFilterType.Condition, value = new string[] { "New", "1000" } },
                    new ItemFilter() { name = ItemFilterType.SoldItemsOnly, value = new string[] { "True" } },
                    new ItemFilter() { name = ItemFilterType.Currency, value = new string[] { "GBP" } }
            };

            // Have we recently updated ... if so, ask for items since we last updated
            if (lastUpdated != null)
            {
                filters.Add(
                    new ItemFilter() { name = ItemFilterType.EndTimeFrom, value = new string[] { lastUpdated.Value.ToString("s") } }
                    );
            }

            request.itemFilter = filters.ToArray();
            request.sortOrder = SortOrderType.EndTimeSoonest;

            return request;
        }

        private void WriteItemsToCSV(string fileName, SearchItem[] items)
        {
            StringBuilder csvContents = new StringBuilder();

            foreach (var item in items)
            {
                csvContents.AppendLine(
                    string.Format("{0},{1},{2},\"{3}\",{4}",
                        item.sellingStatus.currentPrice.Value,
                        item.listingInfo.endTime,
                        item.itemId,
                        item.title,
                        item.viewItemURL)
                );
            }

            if (!File.Exists(fileName))
            {
                File.WriteAllText(fileName, csvContents.ToString());
            }
            else
            {
                File.AppendAllText(fileName, csvContents.ToString());
            }
        }

        private void ShowSoldItemsTrend(IEnumerable<SoldItem> items)
        {
            Console.WriteLine("Recent price trend:");

            var avgs = from item in items
                       group item by new { item.EndTime.Month, item.EndTime.Year } into grp
                       select new { Date = new DateTime(grp.Key.Year, grp.Key.Month, 1), Average = grp.Average(i => i.FinalValue) };

            foreach (var g in avgs)
            {
                Console.WriteLine(string.Format("{0:D2}/{1}: {2:C2}", g.Date.Month, g.Date.Year, g.Average));
            }
        }
    }
}
