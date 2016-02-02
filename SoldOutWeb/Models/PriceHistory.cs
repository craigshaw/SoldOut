using System;

namespace SoldOutWeb.Models
{
    public class PriceHistory
    {
        public DateTime PricePeriod { get; set; }
        public double AveragePrice { get; set; }
    }
}