using System;

namespace SoldOutWeb.Models
{
    public class SearchSummary
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public long SearchID { get; set; }
        public DateTime LastRun { get; set; }
        public int TotalResults { get; set; }
        public string Link { get; set; }
    }
}