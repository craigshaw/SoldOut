using SoldOutBusiness.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SoldOutBusiness.DAL
{
    public class SoldOutDbInitialiser : DropCreateDatabaseIfModelChanges<SoldOutContext>
    {
        protected override void Seed(SoldOutContext context)
        {
            base.Seed(context);

            #region Currencies
            var currencies = new List<Currency>()
            {
                    new Currency() { CurrencyCode = "GBP" },
                    new Currency() { CurrencyCode = "USD" },
                    new Currency() { CurrencyCode = "JPY" },
                    new Currency() { CurrencyCode = "EUR" }
            };

            context.Currencies.AddRange(currencies);
            context.SaveChanges();
            #endregion

            #region Categories
            Category legoCategory = new Category() { Name = "Lego", IncludeInKeywordSearch = true };
            context.Categories.Add(legoCategory);
            context.SaveChanges();

            context.Categories.AddRange(new List<Category>()
            {
                new Category() { Name = "DC Comics Super Heroes", ParentCategoryId = legoCategory.CategoryID },
                new Category() { Name = "Minifigure", ParentCategoryId = legoCategory.CategoryID, IncludeInKeywordSearch = true },
                new Category() { Name = "Star Wars", ParentCategoryId = legoCategory.CategoryID },
                new Category() { Name = "Ghostbusters", ParentCategoryId = 1, IncludeInKeywordSearch = true }
            });
            context.SaveChanges();

            var dcCategory = context.Categories.Where(c => c.Name == "DC Comics Super Heroes").Select(s => s).Single();

            Category batmanCategory = new Category() { Name = "Batman Classic TV Series", ParentCategoryId = dcCategory.CategoryID };
            context.Categories.Add(batmanCategory);
            context.SaveChanges();
            #endregion

            #region Products
            Product batCave = new Product()
            {
                Name = "Batcave",
                YearOfRelease = "2016",
                ManufacturerCode = "76052",
                OriginalRRP = new List<Price>() { new Price() { Amount = 229.99, CurrencyCode = "GBP", IsRRP = true },
                                                  new Price() { Amount = 269.99, CurrencyCode = "USD", IsRRP = true },
                                                  new Price() { Amount = 249.99, CurrencyCode = "EUR", IsRRP = true } }
            };
            batCave.Categories.Add(dcCategory);

            context.Products.Add(batCave);
            context.SaveChanges();

            var miniFigsCategory = context.Categories.Where(c => c.Name == "Minifigure").Single();

            var Products = new List<Product>()
            {
                new Product() { Categories = new List<Category>() { dcCategory }, Name = "The Bat vs. Bane", ManufacturerCode = "76001"  },
                new Product() { ParentProducts = new List<Product>() { batCave }, Categories = new List<Category> { batmanCategory, miniFigsCategory }, Name = "Alfred Pennyworth" },
                new Product() { ParentProducts = new List<Product>() { batCave }, Categories = new List<Category> { batmanCategory, miniFigsCategory }, Name = "Batman" },
                new Product() { ParentProducts = new List<Product>() { batCave }, Categories = new List<Category> { batmanCategory, miniFigsCategory }, Name = "Bruce Wayne" },
                new Product() { ParentProducts = new List<Product>() { batCave }, Categories = new List<Category> { batmanCategory, miniFigsCategory }, Name = "Catwoman" },
                new Product() { ParentProducts = new List<Product>() { batCave }, Categories = new List<Category> { batmanCategory, miniFigsCategory }, Name = "Dick Grayson" },
                new Product() { ParentProducts = new List<Product>() { batCave }, Categories = new List<Category> { batmanCategory, miniFigsCategory }, Name = "Robin" },
                new Product() { ParentProducts = new List<Product>() { batCave }, Categories = new List<Category> { batmanCategory, miniFigsCategory }, Name = "The Joker" },
                new Product() { ParentProducts = new List<Product>() { batCave }, Categories = new List<Category> { batmanCategory, miniFigsCategory }, Name = "The Penguin" },
                new Product() { ParentProducts = new List<Product>() { batCave }, Categories = new List<Category> { batmanCategory, miniFigsCategory }, Name = "The Riddler" }
            };

            context.Products.AddRange(Products);
            context.SaveChanges();
            #endregion

            #region Searches
            // Searches
            var set = context.Products.Where(p => p.Name == "The Bat vs. Bane").Single();
            var searches = new List<Search>()
            {
                new Search() { Name = "Lego 76001", Description = "The Bat vs. Bane", Link = "http://brickset.com/sets/76001-1/The-Bat-vs-Bane-Tumbler-Chase",
                ProductId = set.ProductId, LastCleansed = DateTime.Now.AddYears(-1), LastRun = DateTime.Now.AddYears(-1), OriginalRRP = 39.99}
            };

            context.Searches.AddRange(searches);
            context.SaveChanges();
            #endregion
 
            #region Conditions
            // Conditions
            var conditions = new List<Condition>()
            {
                new Condition() { ConditionId = 1, Description = "Unknown", eBayConditionId = 0 },
                new Condition() { ConditionId = 2, Description = "New", eBayConditionId = 1000 },
                new Condition() { ConditionId = 3, Description = "New other (see details)", eBayConditionId = 1500 },
                new Condition() { ConditionId = 4, Description = "New with defects", eBayConditionId = 1750 },
                new Condition() { ConditionId = 5, Description = "Manufacturer refurbished", eBayConditionId = 2000 },
                new Condition() { ConditionId = 6, Description = "Seller refurbished", eBayConditionId = 2500 },
                new Condition() { ConditionId = 7, Description = "Used", eBayConditionId = 3000 },
                new Condition() { ConditionId = 8, Description = "Very Good", eBayConditionId = 4000 },
                new Condition() { ConditionId = 9, Description = "Good", eBayConditionId = 5000 },
                new Condition() { ConditionId = 10, Description = "Acceptable", eBayConditionId = 6000 },
                new Condition() { ConditionId = 11, Description = "For parts or not working", eBayConditionId = 7000 }
            };

            context.Conditions.AddRange(conditions);
            context.SaveChanges();
            #endregion

            #region SearchResults
            var search = context.Searches.First();
            var results = new List<SearchResult>()
            {
                new SearchResult() { ConditionId = 2, DateOfMatch = DateTime.Now, ProductId = set.ProductId,
                EndTime = DateTime.Now.AddHours(-1), Price = 29.99, SearchID = search.SearchId, }
            };

            context.SearchResults.AddRange(results);
            context.SaveChanges();
            #endregion

            #region Phrases
            var phrases = new List<SuspiciousPhrase>()
            {
                new SuspiciousPhrase() { Phrase = "minifigure" },
                new SuspiciousPhrase() { Phrase = "minifigures" },
                new SuspiciousPhrase() { Phrase = "no box" }
            };

            context.SuspiciousPhrases.AddRange(phrases);
            context.SaveChanges();

            var searchPhrases = new List<SearchSuspiciousPhrase>()
            {
                new SearchSuspiciousPhrase() { SearchId = search.SearchId, Phrase = "bane" }
            };

            context.SearchSuspiciousPhrases.AddRange(searchPhrases);
            context.SaveChanges();
            #endregion
        }
    }
}
