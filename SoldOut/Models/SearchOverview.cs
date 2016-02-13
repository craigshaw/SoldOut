using System;

namespace SoldOutCleanser.Models
{
    internal class SearchOverview
    {
        public DateTime LastCleansed { get; set; }
        public DateTime LastRun { get; set; }
        public string Description { get; set; }
        public int UncleansedCount { get; set; }
        public long SearchId { get; set; }
    }
}
