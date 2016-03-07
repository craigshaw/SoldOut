using System;
using System.Collections.Generic;
using SoldOutBusiness.Models;
using SoldOutSearchMonkey.Model;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SoldOutSearchMonkey.Services
{
    public class CompletedItemReviewer : ICompletedItemReviewer
    {
        private const int SetSizeForPriceReview = 20;
        private readonly IList<SuspiciousPhrase> _basicSuspiciousPhrases;

        public CompletedItemReviewer(IList<SuspiciousPhrase> basicSuspiciousPhrases)
        {
            if (basicSuspiciousPhrases == null)
                throw new ArgumentNullException(nameof(basicSuspiciousPhrases));

            _basicSuspiciousPhrases = basicSuspiciousPhrases.ToList();
        }

        // TODO: This can only be refreshed with a service restart. Would like to add a periodic update of this property
        private IList<SuspiciousPhrase> BasicSuspiciousPhrases
        {
            get { return _basicSuspiciousPhrases;  }
        }

        public ItemReviewSummary ReviewCompletedItems(IEnumerable<SearchResult> items, PriceStats priceStats, ICollection<SearchSuspiciousPhrase> searchSuspiciousPhrases)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            if (priceStats == null)
                throw new ArgumentNullException(nameof(priceStats));

            if (searchSuspiciousPhrases == null)
                throw new ArgumentNullException(nameof(searchSuspiciousPhrases));

            // If we haven't got any previous stats, we can't run any historical price checks
            if (priceStats.NumberOfResults > 0 && priceStats.NumberOfResults >= SetSizeForPriceReview)
            {
                var upperPriceBound = priceStats.AverageSalePrice + (2 * priceStats.StandardDeviation);
                var lowerPriceBound = priceStats.AverageSalePrice - (2 * priceStats.StandardDeviation);

                // If the price is outside of 2 std deviations (68 - 95 - 99.7 rule)
                items.Where(i => i.Price <= lowerPriceBound || i.Price >= upperPriceBound)
                    .ToList().ForEach(i => i.Suspicious = true);
            }

            // Now look for signs in the auction title that it's not a complete item
            if (BasicSuspiciousPhrases.Count > 0 || searchSuspiciousPhrases.Count > 0)
            {
                Regex regex = BuildSuspiciousPhrasesRegex(searchSuspiciousPhrases);

                items.Where(i =>
                    regex.Matches(i.Title).Count > 0
                ).ToList().ForEach(i => i.Suspicious = true);
            }

            var summary = new ItemReviewSummary()
            {
                SuspiciousItems = items.Where(i => i.Suspicious == true).Select(sr => sr).ToList(),
                DeletedItems = null // We aren't supporting auto deletion yet. When we do though...
            };

            return summary;
        }

        private Regex BuildSuspiciousPhrasesRegex(ICollection<SearchSuspiciousPhrase> searchSuspiciousPhrases)
        {
            StringBuilder regularExpression = new StringBuilder();

            // Basic global phrases first
            for (int i = 0; i < BasicSuspiciousPhrases.Count; i++)
            {
                regularExpression.AppendFormat("\\b{0}\\b{1}", BasicSuspiciousPhrases[i].Phrase, i < BasicSuspiciousPhrases.Count - 1 ? "|" : "");
            }

            // Now search specific phrases
            if (searchSuspiciousPhrases.Count > 0)
            {
                var additionalPhrases = searchSuspiciousPhrases.ToList();
                regularExpression.Append("|");
                for (int i = 0; i < searchSuspiciousPhrases.Count; i++)
                {
                    regularExpression.AppendFormat("\\b{0}\\b{1}", additionalPhrases[i].Phrase, i < additionalPhrases.Count - 1 ? "|" : "");
                }
            }

            return new Regex($"({regularExpression.ToString()})", RegexOptions.IgnoreCase);
        }
    }
}
