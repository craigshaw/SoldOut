using System;
using System.Linq;
using System.Collections.Generic;

namespace SoldOutWeb.Models
{
    public class PriceHistory
    {

        private SoldOutBusiness.Models.Search _parent;

        public DateTime PricePeriod { get; set; }

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

        public IEnumerable<PriceHistory> GetPriorPrices(int interval)
        {
                return from pricepoint in _parent.SearchResults
                       group pricepoint by new { pricepoint.EndTime.Value.Day, pricepoint.EndTime.Value.Month, pricepoint.EndTime.Value.Year } into grp
                       orderby grp.Key.Year, grp.Key.Month, grp.Key.Day
                       where DateTime.Parse($"{grp.Key.Day}/{grp.Key.Month:D2}/{grp.Key.Year}") < this.PricePeriod // Exclude ourselves and any prices after us
                       && DateTime.Parse($"{grp.Key.Day}/{grp.Key.Month:D2}/{grp.Key.Year}") >= this.PricePeriod.Subtract(new TimeSpan(0,interval,0))
                       select new PriceHistory()
                       {
                           PricePeriod = DateTime.Parse($"{grp.Key.Day}/{grp.Key.Month:D2}/{grp.Key.Year}"),
                           AveragePrice = (double)grp.Average(pricepoint => pricepoint.Price)
                       };
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
    }
}