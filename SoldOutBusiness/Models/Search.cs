﻿using System;
using System.Collections.Generic;

namespace SoldOutBusiness.Models
{
    public class Search
    {
        public Search()
        {
            SearchResults = new List<SearchResult>();
            SearchCriteria = new List<SearchCriteria>();
        }

        public long SearchId { get; set; }
        public string Name { get; set; }
        public DateTime LastRun { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
        public DateTime LastCleansed { get; set; }
        public double OriginalRRP { get; set; }

        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        public ICollection<SearchResult> SearchResults { get; set; }
        public ICollection<SearchCriteria> SearchCriteria { get; set; }
        public virtual ICollection<SearchSuspiciousPhrase> SuspiciousPhrases { get; set; } // Lazy loaded
    }
}
