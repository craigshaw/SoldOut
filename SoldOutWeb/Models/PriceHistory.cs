using System;
using System.Linq;
using System.Collections.Generic;

namespace SoldOutWeb.Models
{
    public class PriceSummary
    {
        public double Price { get; set; }
        public DateTime PricePoint { get; set; }

    }

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

            //where DateTime.Parse($"{grp.Key.Day}/{grp.Key.Month:D2}/{grp.Key.Year}") < this.PricePeriod // Exclude ourselves and any prices after us
            //&& DateTime.Parse($"{grp.Key.Day}/{grp.Key.Month:D2}/{grp.Key.Year}") >= this.PricePeriod.Subtract(new TimeSpan(0,interval,0))



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

        private double CalculatePriceSTDDeviation(int interval)
        {
            double M = 0.0;
            double S = 0.0;
            int k = 0;

            IEnumerable<PriceHistory> priorPrices = GetPriorPrices(interval);

            foreach (PriceHistory price in priorPrices)
            {
                k++;
                double tmpM = M;
                M += (price.AveragePrice - tmpM) / k;
                S += (price.AveragePrice - tmpM) * (price.AveragePrice - M);
            }

            return Math.Sqrt(S / (k - 1));
        }

        public double SMA
        {
            get
            {
                double simpleMovingAverage = 0.00d;

                IEnumerable<PriceHistory> priorPrices = GetPriorPrices(this.Interval -1); // -1 as we need to add our own price

                foreach (PriceHistory price in priorPrices)
                {
                    simpleMovingAverage += price.AveragePrice;
                }

                simpleMovingAverage += this.AveragePrice; // Add ourselves

                return simpleMovingAverage / this.Interval;
            }
        }
    }
}