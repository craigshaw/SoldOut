using System;

namespace SoldOutWeb.Models
{
    public class PriceHistory
    {
        public string PricePeriod { get; set; }
        public double MinPrice { get; set; }

        public double MaxPrice { get; set; }

        public double AveragePrice { get; set; }

        public double[] PriorPrices { get; set; }
    }
}