using SoldOutBusiness.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace SoldOutBusiness.DAL
{
    public class SoldOutDbInitialiser : DropCreateDatabaseIfModelChanges<SoldOutContext>
    {
        protected override void Seed(SoldOutContext context)
        {
            base.Seed(context);

            var currencies = new List<Ccy>()
            {
                    new Ccy() { Currency = "GBP" },
                    new Ccy() { Currency = "USD" },
                    new Ccy() { Currency = "JPY" },
                    new Ccy() { Currency = "EUR" }
            };

            Category legoCategory = new Category() { Name = "Lego", CategoryID = 1, IncludeInKeywordSearch = true };
            Category dcComicsCategory = new Category() { CategoryID = 2, Name = "DC Comics Super Heroes", ParentCategoryId = 1 };
            Category batmanCategory = new Category() { CategoryID = 3, Name = "Batman Classic TV Series", ParentCategoryId = 2 };
            Category miniFigsCategory = new Category() { CategoryID = 4, Name = "Minifigure", ParentCategoryId = 1, IncludeInKeywordSearch = true };

            var Categories = new List<Category>()
            {
                    legoCategory,
                    miniFigsCategory,
                    new Category() { Name = "Star Wars", ParentCategoryId = 1 },
                    new Category() { Name = "Ghostbusters", ParentCategoryId = 1, IncludeInKeywordSearch = true },
                    dcComicsCategory,
                    batmanCategory
            };

            context.Categories.AddRange(Categories);
            context.SaveChanges();

            Product batCave = new Product()
            {
                ProductId = 2,
                CategoryIds = new List<Category>() { dcComicsCategory, batmanCategory },
                Name = "Batcave",
                YearOfRelease = "2016",
                ManufacturerCode = "76052",
                OriginalRRP = new List<Price>() { new Price() { Amount = 229.99, PricedIn = new Ccy() { Currency = "GBP" }, IsRRP = true },
                                                                  new Price() { Amount = 269.99, PricedIn = new Ccy() { Currency = "USD" }, IsRRP = true },
                                                                  new Price() { Amount = 249.99, PricedIn = new Ccy() { Currency = "EUR" }, IsRRP = true }},
            };

            var Products = new List<Product>()
            {
                new Product() { ProductId = 1, CategoryIds = new List<Category>() { dcComicsCategory }, Name = "The Bat vs. Bane", ManufacturerCode = "76001"  },
                batCave,
                new Product() { ParentProducts = new List<Product>() { batCave }, CategoryIds = new List<Category> { batmanCategory, miniFigsCategory }, Name = "Alfred Pennyworth" },
                new Product() { ParentProducts = new List<Product>() { batCave }, CategoryIds = new List<Category> { batmanCategory, miniFigsCategory }, Name = "Batman" },
                new Product() { ParentProducts = new List<Product>() { batCave }, CategoryIds = new List<Category> { batmanCategory, miniFigsCategory }, Name = "Bruce Wayne" },
                new Product() { ParentProducts = new List<Product>() { batCave }, CategoryIds = new List<Category> { batmanCategory, miniFigsCategory }, Name = "Catwoman" },
                new Product() { ParentProducts = new List<Product>() { batCave }, CategoryIds = new List<Category> { batmanCategory, miniFigsCategory }, Name = "Dick Grayson" },
                new Product() { ParentProducts = new List<Product>() { batCave }, CategoryIds = new List<Category> { batmanCategory, miniFigsCategory }, Name = "Robin" },
                new Product() { ParentProducts = new List<Product>() { batCave }, CategoryIds = new List<Category> { batmanCategory, miniFigsCategory }, Name = "The Joker" },
                new Product() { ParentProducts = new List<Product>() { batCave }, CategoryIds = new List<Category> { batmanCategory, miniFigsCategory }, Name = "The Penguin" },
                new Product() { ParentProducts = new List<Product>() { batCave }, CategoryIds = new List<Category> { batmanCategory, miniFigsCategory }, Name = "The Riddler" }
            };

            context.Products.AddRange(Products);
            context.SaveChanges();


            // Searches
            var searches = new List<Search>()
            {
                new Search() { Name = "Lego 76001", Description = "The Bat vs. Bane", Link = "http://brickset.com/sets/76001-1/The-Bat-vs-Bane-Tumbler-Chase",
                LastCleansed = DateTime.Now.AddYears(-1), LastRun = DateTime.Now.AddYears(-1), OriginalRRP = 39.99}
            };

            context.Searches.AddRange(searches);
            context.SaveChanges();

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

            // TODO: Phrases
        }
    }
}
