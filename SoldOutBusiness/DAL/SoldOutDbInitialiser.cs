using SoldOutBusiness.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace SoldOutBusiness.DAL
{
    public class SoldOutDbInitialiser : DropCreateDatabaseIfModelChanges<SearchContext>
    {
        protected override void Seed(SearchContext context)
        {
            base.Seed(context);

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
