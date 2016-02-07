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

            var searches = new List<Search>()
            {
                new Search() { Name = "Lego 76001", Description = "The bat vs. Bane", Link = "http://brickset.com/sets/76001-1/The-Bat-vs-Bane-Tumbler-Chase",
                LastCleansed = DateTime.Now.AddYears(-1), LastRun = DateTime.Now.AddYears(-1)}
            };

            context.Searches.AddRange(searches);
            context.SaveChanges();
        }
    }
}
