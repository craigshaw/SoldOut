using System;

namespace SoldOutBusiness.Models
{
    [Serializable]
    public class SoldItem
    {
        public string ItemID { get; set; }
        public string Title { get; set; }
        public string ItemURL { get; set; }
        public string GalleryURL { get; set; }
        public double FinalValue { get; set; }
        public string Currency { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Location { get; set; }
        public string SiteId { get; set; }
        public string Type { get; set; }
        public int NumberOfBidders { get; set; }
    }
}
