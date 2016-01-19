using EFSQLTest.Models;
using System;

namespace EFSQLTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var repo = new SearchRepository();
            var searches = repo.GetAllSearchesWithResults();

            foreach (var search in searches)
            {
                Console.WriteLine($"{search.SearchId} {search.Name}");

                if(search.SearchResults != null)
                {
                    foreach (var result in search.SearchResults)
                    {
                        Console.WriteLine($"  {result.SearchResultID} {result.Price:C2} {result.DateOfMatch}");
                    }
                }
            }

            // Add a test result
            repo.AddSearchResult(1, new SearchResult()
            {
                DateOfMatch = DateTime.Now,
                EndTime = DateTime.Now.AddHours(-1),
                ImageURL = "http://test.com/l.jpg",
                ItemNumber = 5674,
                Link = "http://ebay.co.uk/123456",
                NumberofBidders = 1,
                Price = 40.98,
                StartTime = DateTime.Now.AddDays(-2),
                Title = "Another lego auction"
            });

            Console.WriteLine($"{repo.SaveAll()}");   

            Console.ReadKey();
        }
    }
}
