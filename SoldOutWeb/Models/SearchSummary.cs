using System;
using System.Collections.Generic;

namespace SoldOutWeb.Models
{
    public class SearchSummary
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int SearchID { get; set; }
        public DateTime LastRun { get; set; }
        public int TotalResults { get; set; }
        public string Link { get; set; }
    }
}