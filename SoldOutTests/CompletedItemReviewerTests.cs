using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoldOutSearchMonkey.Services;
using SoldOutBusiness.Models;
using System.Collections.Generic;
using SoldOutTests.Extensions;

namespace SoldOutTests
{
    [TestClass]
    public class CompletedItemReviewerTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorThrowsIfGivenNullList()
        {
            var reviewer = new CompletedItemReviewer(null);
        }

        [TestMethod]
        public void ConstrucsWithEmptyList()
        {
            var reviewer = new CompletedItemReviewer(new List<SuspiciousPhrase>());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReviewCompletedItemsThrowsWithNullItems()
        {
            var reviewer = new CompletedItemReviewer(CreateSuspiciousPhraseList());
            reviewer.ReviewCompletedItems(null, new PriceStats(), CreateSearchSuspiciousPhraseList());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReviewCompletedItemsThrowsWithNullPriceStats()
        {
            var reviewer = new CompletedItemReviewer(CreateSuspiciousPhraseList());
            reviewer.ReviewCompletedItems(CreateTestSearchResults(), null, CreateSearchSuspiciousPhraseList());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReviewCompletedItemsThrowsWithNullSearchSuspiciousPhrases()
        {
            var reviewer = new CompletedItemReviewer(CreateSuspiciousPhraseList());
            reviewer.ReviewCompletedItems(CreateTestSearchResults(), new PriceStats(), null);
        }

        [TestMethod]
        public void ReviewCompletedItemsFindsNothingSuspiciousWithNoPricesOrSuspiciousPhrases()
        {
            var reviewer = new CompletedItemReviewer(new List<SuspiciousPhrase>());
            var results = CreateTestSearchResults();
            var summary = reviewer.ReviewCompletedItems(results, new PriceStats() , new List<SearchSuspiciousPhrase>());

            results.AssertNoneAreSuspicious();
            Assert.AreEqual(0, summary.SuspiciousItems.Count);
        }

        [TestMethod]
        public void ReviewCompletedItemsFindsItemsOutside2StandardDeviationsOfTheAveragePrice()
        {
            var reviewer = new CompletedItemReviewer(new List<SuspiciousPhrase>());
            var results = CreateTestSearchResults();
            var summary = reviewer.ReviewCompletedItems(results, new PriceStats() { AverageSalePrice = 28.54, StandardDeviation = 3.50, NumberOfResults = 25 }, new List<SearchSuspiciousPhrase>());

            Assert.AreEqual(1, summary.SuspiciousItems.Count);
            Assert.AreEqual("2", summary.SuspiciousItems[0].ItemNumber);
        }

        [TestMethod]
        public void ReviewCompletedItemsFindsItemsWithBasicSuspiciousPhrase()
        {
            var reviewer = new CompletedItemReviewer(CreateSuspiciousPhraseList());
            var results = CreateTestSearchResults();
            var summary = reviewer.ReviewCompletedItems(results, new PriceStats(), new List<SearchSuspiciousPhrase>());

            Assert.AreEqual(1, summary.SuspiciousItems.Count);
            Assert.AreEqual("3", summary.SuspiciousItems[0].ItemNumber);
        }

        [TestMethod]
        public void ReviewCompletedItemsFindsItemsWithSearchSuspiciousPhrase()
        {
            var reviewer = new CompletedItemReviewer(CreateSuspiciousPhraseList());
            var results = CreateTestSearchResults();
            var summary = reviewer.ReviewCompletedItems(results, new PriceStats(), CreateSearchSuspiciousPhraseList());

            Assert.AreEqual(2, summary.SuspiciousItems.Count);
            Assert.AreEqual("3", summary.SuspiciousItems[0].ItemNumber);
            Assert.AreEqual("4", summary.SuspiciousItems[1].ItemNumber);
        }

        private IList<SuspiciousPhrase> CreateSuspiciousPhraseList()
        {
            return new List<SuspiciousPhrase>() {
                new SuspiciousPhrase() { Phrase = "suspicious" },
                new SuspiciousPhrase() { Phrase = "hello" }
            };
        }

        private IList<SearchSuspiciousPhrase> CreateSearchSuspiciousPhraseList()
        {
            return new List<SearchSuspiciousPhrase>() {
                new SearchSuspiciousPhrase() { Phrase = "suspect" },
                new SearchSuspiciousPhrase() { Phrase = "hello" }
            };
        }

        private IList<SearchResult> CreateTestSearchResults()
        {
            return new List<SearchResult>()
            {
                new SearchResult() { ConditionId = 2, ItemNumber = "1", Price = 29.99, Title = "This is a new test result" },
                new SearchResult() { ConditionId = 2, ItemNumber = "2", Price = 99.99, Title = "This is a new test result with an unusual price" },
                new SearchResult() { ConditionId = 2, ItemNumber = "3", Price = 29.99, Title = "This is a new test result with a suspicious title" },
                new SearchResult() { ConditionId = 2, ItemNumber = "4", Price = 29.99, Title = "This is a new test result with a suspect title" }
            };
        }
    }
}
