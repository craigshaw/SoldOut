using SoldOutWeb.Models;
using System.Collections.Generic;

namespace SoldOutWeb.Services
{
    public enum AggregationPeriod
    {
        Daily,
        Weekly,
        Monthly
    }

    public interface IPriceHistoryService
    {
        IList<PriceHistory> CreateBasicPriceHistory(int searchId, int condition, AggregationPeriod aggregationPeriod = AggregationPeriod.Daily);
        void AddSimpleMovingAverage(IList<PriceHistory> prices, int interval);
        void AddExponentialMovingAverage(IList<PriceHistory> prices, int interval);
    }
}
