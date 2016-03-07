using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoldOutWeb.Services;
using FakeItEasy;
using SoldOutBusiness.Repository;
using SoldOutBusiness.Models;
using System.Collections.Generic;
using System.Linq;
using SoldOutTests.Extensions;

namespace SoldOutTests
{
    [TestClass]
    public class PriceHistoryServiceTests
    {
        [TestMethod]
        public void CreatePriceHistoryReturnsIEnumerableForValidSearch()
        {
            int searchId = 1;
            var mockRepository = A.Fake<ISoldOutRepository>();

            A.CallTo(() => mockRepository.GetSearchResults(searchId)).Returns(CreateTestSearchResults());

            var sut = new PriceHistoryService(mockRepository);

            var results = sut.CreateBasicPriceHistory(searchId);

            A.CallTo(() => mockRepository.GetSearchResults(searchId)).MustHaveHappened();

            Assert.AreEqual(results.Count(), 16);
        }

        [TestMethod]
        public void CalculateSimpleMovingAverageFromBasicPriceHistory()
        {
            int searchId = 2;
            var mockRepository = A.Fake<ISoldOutRepository>();

            A.CallTo(() => mockRepository.GetSearchResults(searchId)).Returns(CreateTestSearchResults());

            var sut = new PriceHistoryService(mockRepository);

            var results = sut.CreateBasicPriceHistory(searchId);

            sut.AddSimpleMovingAverage(results, 5);

            var smas = results.Select(ph => ph.SMA.HasValue ? Math.Round(ph.SMA.Value, 3) : ph.SMA);

            smas.AssertSequenceIsEqual(null, null, null, null, 12.694, 12.804, 12.894,
                12.812, 13.168, 13.12, 13.21, 13.63, 14.036, 14.48, 13.786, 13.588);
        }

        [TestMethod]
        public void CalculateSimpleMovingAverageWhenIntervalBiggerThanNumberOfPricesDoesntSetSMAs()
        {
            var mockRepository = A.Fake<ISoldOutRepository>();

            A.CallTo(() => mockRepository.GetSearchResults(A<long>.Ignored)).Returns(CreateListOfTwoSearchResults());

            var sut = new PriceHistoryService(mockRepository);

            var results = sut.CreateBasicPriceHistory(1);

            sut.AddSimpleMovingAverage(results, 5);

            Assert.AreEqual(results.Count, 2);

            foreach (var result in results)
                Assert.IsNull(result.SMA);
        }

        [TestMethod]
        public void CalculateExponentialMovingAverageFromBasicPriceHistory()
        {
            int searchId = 2;
            var mockRepository = A.Fake<ISoldOutRepository>();

            A.CallTo(() => mockRepository.GetSearchResults(searchId)).Returns(CreateTestSearchResults());

            var sut = new PriceHistoryService(mockRepository);

            var results = sut.CreateBasicPriceHistory(searchId);

            sut.AddExponentialMovingAverage(results, 5);

            var emas = results.Select(ph => ph.EMA.HasValue ? Math.Round(ph.EMA.Value, 8) : ph.EMA);

            // Test results
            emas.AssertSequenceIsEqual(null, null, null, null, 12.694, 12.976, 12.64733333, 12.60822222, 12.66548148,
                13.36365432, 13.57243621, 13.74495748, 14.01663832, 14.34442554, 13.3262837, 13.21752246);
        }

        [TestMethod]
        public void CalculateExponentialMovingAverageWhenIntervalBiggerThanNumberOfPricesDoesntSetEMAs()
        {
            var mockRepository = A.Fake<ISoldOutRepository>();

            A.CallTo(() => mockRepository.GetSearchResults(A<long>.Ignored)).Returns(CreateListOfTwoSearchResults());

            var sut = new PriceHistoryService(mockRepository);

            var results = sut.CreateBasicPriceHistory(1);

            sut.AddExponentialMovingAverage(results, 5);

            foreach (var result in results)
                Assert.IsNull(result.EMA);
        }

        [TestMethod]
        public void CalculateExponentialMovingAverageWhenIntervalEqualsNumberOfPrices()
        {
            var mockRepository = A.Fake<ISoldOutRepository>();

            A.CallTo(() => mockRepository.GetSearchResults(A<long>.Ignored)).Returns(CreateTestSearchResults().Take(5));

            var sut = new PriceHistoryService(mockRepository);

            var results = sut.CreateBasicPriceHistory(1);

            sut.AddExponentialMovingAverage(results, 5);

            Assert.AreEqual(Math.Round(results[4].EMA.Value, 3), 12.694);
        }


        #region Utility Methods
        private IList<SearchResult> CreateTestSearchResults()
        {
            return new List<SearchResult>()
            {
                new SearchResult() { Price = 12.99, EndTime = new DateTime(2016, 01, 01) },
                new SearchResult() { Price = 11.54, EndTime = new DateTime(2016, 01, 02) },
                new SearchResult() { Price = 12.94, EndTime = new DateTime(2016, 01, 03) },
                new SearchResult() { Price = 11.00, EndTime = new DateTime(2016, 01, 04) },
                new SearchResult() { Price = 15.00, EndTime = new DateTime(2016, 01, 05) },
                new SearchResult() { Price = 13.54, EndTime = new DateTime(2016, 01, 06) },
                new SearchResult() { Price = 11.99, EndTime = new DateTime(2016, 01, 07) },
                new SearchResult() { Price = 12.53, EndTime = new DateTime(2016, 01, 08) },
                new SearchResult() { Price = 12.78, EndTime = new DateTime(2016, 01, 09) },
                new SearchResult() { Price = 14.76, EndTime = new DateTime(2016, 01, 10) },
                new SearchResult() { Price = 13.99, EndTime = new DateTime(2016, 01, 11) },
                new SearchResult() { Price = 14.09, EndTime = new DateTime(2016, 01, 12) },
                new SearchResult() { Price = 14.56, EndTime = new DateTime(2016, 01, 13) },
                new SearchResult() { Price = 15.00, EndTime = new DateTime(2016, 01, 14) },
                new SearchResult() { Price = 11.29, EndTime = new DateTime(2016, 01, 15) },
                new SearchResult() { Price = 13.00, EndTime = new DateTime(2016, 01, 16) }
            };
        }

        private IList<SearchResult> CreateListOfTwoSearchResults()
        {
            return new List<SearchResult>()
            {
                new SearchResult() { Price = 12.99, EndTime = new DateTime(2016, 01, 01) },
                new SearchResult() { Price = 13.99, EndTime = new DateTime(2016, 01, 02) }
            };
        }
        #endregion
    }
}
