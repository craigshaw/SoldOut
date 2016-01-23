using System;
using System.Collections.Generic;

namespace SoldOutBusiness.Models
{
    public class Search
    {
        public Search()
        {
            SearchResults = new List<SearchResult>();
        }

        public long SearchId { get; set; }
        public string Name { get; set; }
        public DateTime LastRun { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }

        public ICollection<SearchResult> SearchResults { get; set; }
        public ICollection<SearchCriteria> SearchCriteria { get; set; }
    }
}
