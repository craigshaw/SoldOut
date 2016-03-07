using SoldOutBusiness.Models;
using System.Collections.Generic;

namespace SoldOutSearchMonkey.Model
{
    public class ItemReviewSummary
    {
        public IList<SearchResult> SuspiciousItems { get; set; }
        public IList<SearchResult> DeletedItems { get; set; }
    }
}
