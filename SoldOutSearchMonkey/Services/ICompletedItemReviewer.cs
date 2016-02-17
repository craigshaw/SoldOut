using SoldOutBusiness.Models;
using SoldOutSearchMonkey.Model;
using System.Collections.Generic;

namespace SoldOutSearchMonkey.Services
{
    internal interface ICompletedItemReviewer
    {
        ItemReviewSummary ReviewCompletedItems(IList<SearchResult> items, PriceStats priceStats, ICollection<SearchSuspiciousPhrase> searchSuspiciousPhrases);
    }
}
