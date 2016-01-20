using eBay.Services;
using eBay.Services.Finding;
using SoldOutBusiness.Builders;
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
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SoldOutHarness
{
    class Harness
    {
        private string _searchTerm;
        private IEnumerable<SoldItem> _soldItems;
        private DateTime? _lastUpdatedTime;

        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage:{0}Harness.exe <search term>", Environment.NewLine);
                Console.WriteLine("Example:{0}Harness.exe \"Lego 76001\"{1}", Environment.NewLine, Environment.NewLine);
            }
            else
            {
                new Harness().Run(args[0]);
            }

            Console.WriteLine("Press any key to close the program.");
            Console.ReadKey();

        }

        private void Run(string searchTerm)
        {
            try
            {
                // Create the search catalogue
                var catalogue = new CatalogueBuilder()
                    .AddFromConfigFile("searches.cfg")
                    .Build();

                var finder = new EbayFinder()
                    .Configure(c =>
                   {
                       // Initialize service end-point configuration
                       c.EndPointAddress = "http://svcs.ebay.com/services/search/FindingService/v1";
                       c.GlobalId = "EBAY-GB";
                       // set eBay developer account AppID here!
                       c.ApplicationId = ConfigurationManager.AppSettings["eBayApplicationId"];
                   });

                foreach (var search in catalogue.Searches)
                {
                    //_searchTerm = search.Keywords;
                    _soldItems = null;
                    _lastUpdatedTime = null;

                    // Load data for the current search term if we've gathered results previously
                    LoadItemsFromFile();

                    Console.WriteLine("Requesting completed items for '{0}' {1}",
                        _searchTerm,
                        (_lastUpdatedTime != null) ? "since " + _lastUpdatedTime.Value.ToString() : string.Empty);

                    // Create a request to get our completed items
                    var response = finder.GetCompletedItems(_searchTerm, _lastUpdatedTime);

                    // Show output
                    if (response.ack == AckValue.Success || response.ack == AckValue.Warning)
                    {
                        Console.WriteLine("Found " + response.searchResult.count + " new items");

                        _lastUpdatedTime = response.timestamp;

                        if (response.searchResult.count > 0)
                        {
                            //foreach (var item in response.searchResult.item)
                            //{
                            //    Console.WriteLine(string.Format("{0}: {1}", item.itemId, item.title));
                            //}

                            // Map returned items to our SoldItems model
                            var newItems = MapSoldItems(response.searchResult.item);

                            // Combine with existing results if there are any
                            if (_soldItems != null)
                            {
                                _soldItems = newItems.Concat(_soldItems);
                            }
                            else
                            {
                                _soldItems = newItems;
                            }
                        }

                        // Write everything out to file
                        WriteItemsToFile();

                        // Output some basic stats
                        ShowSoldItemsTrend();
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

        private void WriteItemsToFile()
        {
            MemoryStream memorystream = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();

            bf.Serialize(memorystream, _lastUpdatedTime);
            bf.Serialize(memorystream, _soldItems.ToList());

            File.WriteAllBytes(_searchTerm + ".bin", memorystream.ToArray());
        }

        private void LoadItemsFromFile()
        {
            string fileName = _searchTerm + ".bin";

            if(File.Exists(fileName))
            {
                MemoryStream memorystream = new MemoryStream(File.ReadAllBytes(_searchTerm + ".bin"));
                BinaryFormatter bf = new BinaryFormatter();
                _lastUpdatedTime = (DateTime)bf.Deserialize(memorystream);
                _soldItems = (List<SoldItem>)bf.Deserialize(memorystream);
            }

        }

        private IEnumerable<SoldItem> MapSoldItems(SearchItem[] items)
        {
            return items.Select(i => new SoldItem()
            {
                ItemID = i.itemId,
                Title = i.title,
                ItemURL = i.viewItemURL,
                GalleryURL = i.galleryURL,
                FinalValue = i.sellingStatus.currentPrice.Value,
                Currency = i.sellingStatus.currentPrice.currencyId,
                StartTime = i.listingInfo.endTime,
                EndTime = i.listingInfo.endTime,
                Location = i.location,
                SiteId = i.globalId,
                Type = i.listingInfo.listingType,
                NumberOfBidders = i.listingInfo.listingType.ToLowerInvariant() == "auction" ? i.sellingStatus.bidCount : 0
            });
        }

        private FindCompletedItemsRequest CreateCompletedItemsRequest(DateTime? lastUpdated)
        {
            // Completed items, new only
            var request = new FindCompletedItemsRequest();

            // Search term
            request.keywords = _searchTerm;

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

        private void WriteItemsToCSV(SearchItem[] items)
        {
            StringBuilder csvContents = new StringBuilder();
            string csvFileName = _searchTerm + ".csv";

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

            if (!File.Exists(csvFileName))
            {
                File.WriteAllText(csvFileName, csvContents.ToString());
            }
            else
            {
                File.AppendAllText(csvFileName, csvContents.ToString());
            }
        }

        private void ShowSoldItemsTrend()
        {
            Console.WriteLine("Recent price trend:");

            var avgs = from item in _soldItems
                       group item by new { item.EndTime.Month, item.EndTime.Year } into grp
                       select new { Date = new DateTime(grp.Key.Year, grp.Key.Month, 1), Average = grp.Average(i => i.FinalValue) };

            foreach (var g in avgs)
            {
                Console.WriteLine(string.Format("{0:D2}/{1}: {2:C2}", g.Date.Month, g.Date.Year, g.Average));
            }
        }
    }
}
