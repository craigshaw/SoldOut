using System.Collections.Generic;
using System.Linq;
using SoldOutBusiness.Models;
using SoldOutWeb.Models;
using SoldOutBusiness.Repository;
using System;

namespace SoldOutWeb.Services
{
    public class PriceHistoryService : IPriceHistoryService
    {
        private ISoldOutRepository _repository;

        public PriceHistoryService(ISoldOutRepository repository)
        {
            _repository = repository;
        }

        public IList<PriceHistory> CreateBasicPriceHistory(int searchId, int conditionId, AggregationPeriod aggregationPeriod)
        {
            var aggregator = CreatePriceAggregatorForAggregationPeriod(aggregationPeriod);
            var result = _repository.GetSearchResults(searchId, conditionId,false);
            return aggregator(result);
        }

        private Func<IEnumerable<SearchResult>, IList<PriceHistory>> CreatePriceAggregatorForAggregationPeriod(AggregationPeriod aggregationPeriod)
        {
            if(aggregationPeriod == AggregationPeriod.Daily)
                return new Func<IEnumerable<SearchResult>, IList<PriceHistory>>(AggregatePriceDataDaily);
            else
                return new Func<IEnumerable<SearchResult>, IList<PriceHistory>>(AggregatePriceDataMonthly);
        }

        private IList<PriceHistory> AggregatePriceDataDaily(IEnumerable<SearchResult> searchResults)
        {
            DateTime priorDate = DateTime.Now.AddMonths(-1);

            double totalDays = (DateTime.Now - priorDate).TotalDays;

            var allItems = (from item in searchResults
                           group item by new { item.EndTime.Value.Day, item.EndTime.Value.Month, item.EndTime.Value.Year } into grp
                           orderby grp.Key.Year, grp.Key.Month, grp.Key.Day
                           select new PriceHistory()
                           {
                               PricePeriod = $"{grp.Key.Day:D2}/{grp.Key.Month:D2}/{grp.Key.Year}",
                               AveragePrice = (double)(grp.Average(it => it.Price)),
                               MinPrice = (double)(grp.Min(it => it.Price)),
                               MaxPrice = (double)(grp.Max(it => it.Price)),
                           });

            var summaryItems = allItems.Select(i => i).Where(i => DateTime.Parse(i.PricePeriod) > priorDate).ToList();

            if (summaryItems.Count < totalDays)
                summaryItems = allItems.Take(Convert.ToInt32(totalDays)).ToList();

            return summaryItems;
        }

        private IList<PriceHistory> AggregatePriceDataMonthly(IEnumerable<SearchResult> searchResults)
        {
            return (from item in searchResults
                    group item by new { item.EndTime.Value.Month, item.EndTime.Value.Year } into grp
                    orderby grp.Key.Year, grp.Key.Month
                    select new PriceHistory()
                    {
                        PricePeriod = $"{grp.Key.Month:D2}/{grp.Key.Year}",
                        AveragePrice = (double)(grp.Average(it => it.Price)),
                        MinPrice = (double)(grp.Min(it => it.Price)),
                        MaxPrice = (double)(grp.Max(it => it.Price)),
                    }).ToList();
        }

        public void AddSimpleMovingAverage(IList<PriceHistory> prices, int interval)
        {
            // Loop through each price, starting at the end of the first interval
            for (int i = interval - 1; i < prices.Count; i++)
            {
                double sma = 0;

                // Now we average an interval's worth of prices looking back from our current position
                for (int j = i - (interval - 1); j <= i; j++)
                    sma += prices[j].AveragePrice;

                prices[i].SMA = sma / interval;
            }
        }

        public void AddExponentialMovingAverage(IList<PriceHistory> prices, int interval)
        {
            if (prices.Count < interval)
                return;

            double k = 2 / ((double)interval + 1);

            // Start with the average of the first 'interval' worth of prices
            prices[interval - 1].EMA = prices.Take(interval).Select(ph => ph.AveragePrice).Average();

            // Then use the EMA algorithm to calculate the remaining EMAs
            // http://www.iexplain.org/ema-how-to-calculate/
            for (int i = interval; i < prices.Count; i++)
                prices[i].EMA = prices[i].AveragePrice * k + prices[i - 1].EMA * (1 - k);
        }
    }
}