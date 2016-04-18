using eBay.Services.Finding;
using SoldOutBusiness.Mappers;
using SoldOutBusiness.Repository;
using SoldOutBusiness.Services;
using SoldOutBusiness.Utilities.Conditions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
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
            new Harness().TestStats();
            //new Harness().TestDB();
            //new Harness().Run();
            //Harness test = new Harness();

            //test.GetAllCategories();
            //test.GetAllProducts();
            //test.GetProductsByCategoryId();
            //test.GetAllProductsByParentProductId();

            Console.WriteLine("Press any key to close the program.");
            Console.ReadKey();

        }

        private void TestStats()
        {
            using (var repo = new StatsRepository())
            {
                var top = repo.MostPopularProducts(2, 10, 30);

                var position = 1;
                foreach (var product in top)
                {
                    Console.WriteLine($"{position++}. {product.ManufacturerCode} {product.Name} {product.ItemCount} {product.AveragePrice:C2}");
                }
            }



            //    var r = new SoldOutRepository();
            //using (var repo = new StatsRepository())
            //{
            //    var top = repo.MostPopularProducts(7);

            //    foreach(var search in top)
            //    {
            //        var p = r.GetProductByID(search.ProductId);
            //        Console.WriteLine($"{p.Name}: {search.ItemCount}");
            //    }

            //    Console.WriteLine(Environment.NewLine); Console.WriteLine(Environment.NewLine);
            //    Console.WriteLine("Most expensive items");

            //    var exp = repo.MostExpensiveProducts(2);

            //    foreach (var res in exp)
            //    {
            //        var pr = res.Product;
            //        Console.WriteLine($"{pr.Name}: {res.Price:C2} {res.SearchResultID}");
            //    }
            //}
            //r.Dispose();
        }

        private void TestDB()
        {
            using (var repo = new SoldOutRepository())
            {
                var ctx = repo._context;

                #region Product and SubProducts
                Console.WriteLine("Product tests");
                Console.WriteLine("-------------");
                var product = ctx.Products.Where(p => p.ManufacturerCode == "76052").Single();
                Console.WriteLine($"{product.Name} ({product.ProductId}) has {product.ParentProducts.Count} parent products and {product.SubProducts.Count} child products:");
                foreach (var sub in product.SubProducts)
                {
                    Console.WriteLine($" {sub.Name} ({sub.ProductId})");
                }

                Console.WriteLine();
                var product2 = ctx.Products.Where(p => p.Name == "Batman").Single();
                Console.WriteLine($"{product2.Name} ({product2.ProductId}) has {product2.ParentProducts.Count} parent products:");
                foreach (var sub2 in product2.ParentProducts)
                {
                    Console.WriteLine($" {sub2.Name} ({sub2.ProductId})");
                }
                #endregion

                Console.WriteLine(); Console.WriteLine();

                #region Categories child and parent relations
                Console.WriteLine("Category tests");
                Console.WriteLine("--------------");
                var cat = ctx.Categories.Where(c => c.Name == "Lego").First();
                Console.WriteLine($"{cat.Name} ({cat.CategoryID}) has {cat.Children.Count} children:");
                foreach (var c in cat.Children)
                {
                    Console.WriteLine($" {c.Name} ({c.CategoryID}) [parent details - {c.Parent.Name} ({c.Parent.CategoryID})]");
                }
                #endregion

                Console.WriteLine(); Console.WriteLine();

                #region SearchResults
                Console.WriteLine("Search Result tests");
                Console.WriteLine("-------------------");
                var results = ctx.SearchResults.Select(sr => sr).ToList();
                Console.WriteLine($"There are {results.Count} search results:");
                foreach (var res in results)
                {
                    Console.WriteLine($" {res.DateOfMatch.ToString()} {res.Price:C2} - {res.Product.Name} - {res.Search.Name} - {res.Condition.Description} ");
                }
                #endregion


                #region Stored Procs
                //var stats = repo.GetPriceStatsForSearch(1, 2);
                //Console.WriteLine($"{stats.NumberOfResults} results for search 1");
                #endregion
            }
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

            //var prod = repo._context.Products.Where(p => p.ProductId == 1).Select(s => s).First();

            //foreach (var item in prod.ParentProducts)
            //{
            //    Console.WriteLine($"{item.Name}");
            //}


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
                    var response = finder.GetCompletedItems(search, 1);

                    // Show output
                    if (response.ack == AckValue.Success || response.ack == AckValue.Warning)
                    {
                        Console.WriteLine("Found " + response.searchResult.count + " new items");

                        // Set the last ran time
                        search.LastRun = response.timestamp;

                        if (response.searchResult.count > 0)
                        {
                            // Map returned items to our SoldItems model
                            var newItems = eBayMapper.MapSearchItemsToSearchResults(response.searchResult.item, conditionResolver, search.ProductId); 

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
