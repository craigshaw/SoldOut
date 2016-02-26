using System;
using System.Linq;
using System.Collections.Generic;

namespace SoldOutWeb.Models
{
    public class PriceHistory
    {

        private DateTime pricePeriod;
        private SoldOutBusiness.Models.Search _parent;

        public DateTime PricePeriod
        {
            get
            {
                return this.pricePeriod;
            }

            set
            {
                this.pricePeriod = (DateTime)value;
            }
        }

        public double MinPrice { get; set; }

        public double MaxPrice { get; set; }

        public double AveragePrice { get; set; }

        public SoldOutBusiness.Models.Search Parent
        { 
            set
            {
                _parent = value;
            }

        }

        public int Interval { get; set; }

        public IEnumerable<PriceHistory> GetPriorPrices(int interval)
        {
            List<PriceHistory> prices;

            prices = new List<PriceHistory>();

            var allPrices =  from pricepoint in _parent.SearchResults
                    group pricepoint by new { pricepoint.EndTime.Value.Day, pricepoint.EndTime.Value.Month, pricepoint.EndTime.Value.Year } into grp
                    orderby grp.Key.Year, grp.Key.Month, grp.Key.Day                       
                    select new PriceHistory()
                    {
                        PricePeriod = DateTime.Parse($"{grp.Key.Day}/{grp.Key.Month:D2}/{grp.Key.Year}"),
                        AveragePrice = (double)grp.Average(pricepoint => pricepoint.Price)
                    };

            // Create a list of all the prices prior to ourselves that are within the required interval i.e 5 days
            foreach (PriceHistory price in allPrices)
            {
                if (price.PricePeriod < this.PricePeriod && price.PricePeriod >= this.PricePeriod.Subtract(new TimeSpan(interval,0, 0, 0)))
                {
                    prices.Add(new PriceHistory()
                    {
                        PricePeriod = price.PricePeriod,
                        AveragePrice = price.AveragePrice

                    });
                }
            }

            return prices;
        }

        private double? CalculatePriceSTDDeviation(int interval)
        {
            double M = 0.0;
            double S = 0.0;
            int k = 0;

            IEnumerable<PriceHistory> priorPrices = GetPriorPrices(interval);

            if (priorPrices == null) return null;

            foreach (PriceHistory price in priorPrices)
            {
                k++;
                double tmpM = M;
                M += (price.AveragePrice - tmpM) / k;
                S += (price.AveragePrice - tmpM) * (price.AveragePrice - M);
            }

            return Math.Sqrt(S / (k - 1));
        }

        public double? SMA
        {
            get
            {
                double simpleMovingAverage = 0.00d;

                if (_parent.SearchResults.Count() - 1 < this.Interval) return null;

                IEnumerable<PriceHistory> priorPrices = GetPriorPrices(this.Interval -1); // -1 as we need to add our own price

                // We have no prior prices
                if (priorPrices == null) return null;

                // We don't have enough prices
                if (priorPrices.Count() < this.Interval - 1) return null;

                foreach (PriceHistory price in priorPrices)
                {
                    simpleMovingAverage += price.AveragePrice;
                }

                simpleMovingAverage += this.AveragePrice; // Add ourselves

                return simpleMovingAverage / this.Interval;
            }
        }

        public double? EMA
        {
            get
            {
                double? exponentialMovingAverage = 0.00d;
                double multiplier = 0.00d;
                PriceHistory yesterdaysPrice;

                if (_parent.SearchResults.Count() - 1 < this.Interval) return null;

                multiplier = 2 / this.Interval + 1;

                yesterdaysPrice = (PriceHistory)GetPriorPrices(1);

                exponentialMovingAverage += this.AveragePrice * multiplier + yesterdaysPrice.EMA *(1 - multiplier);

                return exponentialMovingAverage;
            }
        }

            public double? MACD
        {
            get
            {
                double? lowerMACD = 0.00;
                double? upperMACD = 0.00;

                this.Interval = 12;
                lowerMACD = this.EMA;

                this.Interval = 26;
                upperMACD = this.EMA;

                return lowerMACD - upperMACD;
            }
        }
    }
}