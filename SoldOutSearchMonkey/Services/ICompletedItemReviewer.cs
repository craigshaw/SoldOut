using SoldOutBusiness.Models;
using SoldOutSearchMonkey.Model;
using System.Collections.Generic;

namespace SoldOutSearchMonkey.Services
{
    public interface ICompletedItemReviewer
    {
        ItemReviewSummary ReviewCompletedItems(IEnumerable<SearchResult> items, PriceStats priceStats, ICollection<SearchSuspiciousPhrase> searchSuspiciousPhrases);
    }
}
