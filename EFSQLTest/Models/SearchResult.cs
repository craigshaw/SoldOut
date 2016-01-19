 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFSQLTest.Models
{
    public class SearchResult
    {
        public int SearchResultID { get; set; }
        public DateTime DateOfMatch { get; set; }
        public string Link { get; set; }
        public string Title { get; set; }
        public double? Price { get; set; }
        public int? ItemNumber { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? NumberofBidders { get; set; }
        public string ImageURL { get; set; }
        public long SearchId { get; set; }

        public virtual Search Search { get; set; }


        //public string ItemID { get; set; }
        //public string Title { get; set; }
        //public string ItemURL { get; set; }
        //public string GalleryURL { get; set; }
        //public double FinalValue { get; set; }
        //public string Currency { get; set; }
        //public DateTime StartTime { get; set; }
        //public DateTime EndTime { get; set; }
        //public string Location { get; set; }
        //public string SiteId { get; set; }
        //public string Type { get; set; }
        //public int NumberOfBidders { get; set; }
    }
}
