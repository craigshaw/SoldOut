using eBay.Services.Finding;
using SoldOutBusiness.Mappers;
using SoldOutBusiness.Repository;
using SoldOutBusiness.Services;
using SoldOutBusiness.Utilities.Conditions;
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
        private IConditionResolver conditionResolver;
        

        static void Main(string[] args)
        {
            //new Harness().Run();

            
            Harness test = new Harness();

            test.GetAllCategories();
            test.GetAllProducts();
            test.GetProductsByCategoryId();
            test.GetAllProductsByParentProductId();

            Console.WriteLine("Press any key to close the program.");
            Console.ReadKey();

        }

        private void GetAllCategories()
        {
            var repo = new SoldOutRepository();

            var allCategories = repo.GetAllCategories();

            foreach (var category in allCategories)
            {
                Console.WriteLine(category.Name);
            }
        }

        private void GetAllProducts()
        {
            var repo = new SoldOutRepository();

            var allProducts = repo.GetAllProducts();

            foreach (var product in allProducts)
            {
                Console.WriteLine(product.Name);
            }
        }

        private void GetProductsByCategoryId()
        {
            var repo = new SoldOutRepository();

            Console.Write("Products with Category:");

            var childProducts = repo.GetProductsByCategoryId(4);

            foreach (var product in childProducts)
            {                
                Console.WriteLine(product.Name);
            }
        }

        private void GetAllProductsByParentProductId()
        {
            var repo = new SoldOutRepository();

            Console.Write("Products with ParentId:");

            var childProducts = repo.GetProductsByParentProductId(11);

            foreach (var product in childProducts)
            {
                Console.WriteLine(product.Name);
            }
        }

        private void Run()
        {
            try
            {
                var repo = new SoldOutRepository();

                // Create the search catalogue
                var searches = repo.GetAllSearchesWithSearchCriteria();
                conditionResolver = new ConditionResolver(repo.GetConditions());

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
                    var response = finder.GetCompletedItems(search);

                    // Show output
                    if (response.ack == AckValue.Success || response.ack == AckValue.Warning)
                    {
                        Console.WriteLine("Found " + response.searchResult.count + " new items");

                        // Set the last ran time
                        search.LastRun = response.timestamp;

                        if (response.searchResult.count > 0)
                        {
                            // Map returned items to our SoldItems model
                            var newItems = eBayMapper.MapSearchItemsToSearchResults(response.searchResult.item, conditionResolver); 

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

        private void WriteItemsToFile(string fileName, DateTime updateTime, IEnumerable<SoldOutBusiness.Models.SearchResult> items)
        {
            MemoryStream memorystream = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();

            bf.Serialize(memorystream, updateTime);
            bf.Serialize(memorystream, items.ToList());

            File.WriteAllBytes(fileName, memorystream.ToArray());
        }

        private IEnumerable<SoldOutBusiness.Models.SearchResult> LoadItemsFromFile(string fileName)
        {
            if(File.Exists(fileName))
            {
                MemoryStream memorystream = new MemoryStream(File.ReadAllBytes(fileName));
                BinaryFormatter bf = new BinaryFormatter();
                //_lastUpdatedTime = (DateTime)bf.Deserialize(memorystream);
                return (List<SoldOutBusiness.Models.SearchResult>)bf.Deserialize(memorystream);
            }

            return null;
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

        private void ShowSoldItemsTrend(IEnumerable<SoldOutBusiness.Models.SearchResult> items)
        {
            Console.WriteLine("Recent price trend:");

            var avgs = from item in items
                       group item by new { item.EndTime.Value.Month, item.EndTime.Value.Year } into grp
                       select new { Date = new DateTime(grp.Key.Year, grp.Key.Month, 1), Average = grp.Average(i => i.Price) };

            foreach (var g in avgs)
            {
                Console.WriteLine(string.Format("{0:D2}/{1}: {2:C2}", g.Date.Month, g.Date.Year, g.Average));
            }
        }
    }
}
