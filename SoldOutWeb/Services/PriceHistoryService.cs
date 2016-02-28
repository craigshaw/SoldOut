using System.Collections.Generic;
using System.Linq;
using SoldOutWeb.Models;
using SoldOutBusiness.Repository;

namespace SoldOutWeb.Services
{
    public class PriceHistoryService : IPriceHistoryService
    {
        private ISearchRepository _repository;

        public PriceHistoryService(ISearchRepository repository)
        {
            _repository = repository;
        }

        public IList<PriceHistory> CreateBasicPriceHistory(int searchId)
        {
            var results = _repository.GetSearchResults(searchId).ToList();

            return (from item in results
                   group item by new { item.EndTime.Value.Day, item.EndTime.Value.Month, item.EndTime.Value.Year } into grp
                   orderby grp.Key.Year, grp.Key.Month, grp.Key.Day
                   select new PriceHistory()
                   {
                       PricePeriod = $"{grp.Key.Day:D2}/{grp.Key.Month:D2}/{grp.Key.Year}",
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