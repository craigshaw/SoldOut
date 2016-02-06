using System;

namespace SoldOutBusiness.Models
{
    public class SearchResult
    {
        public int SearchResultID { get; set; }
        public DateTime DateOfMatch { get; set; }
        public string Link { get; set; }
        public string Title { get; set; }
        public double? Price { get; set; }
        public string ItemNumber { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? NumberOfBidders { get; set; }
        public string ImageURL { get; set; }
        public long SearchID { get; set; }
        public string Currency { get; set; }
        public string Location { get; set; }
        public string SiteID { get; set; }
        public string Type { get; set; }

        public double? ShippingCost { get; set; }

        public virtual Search Search { get; set; }
    }
}
