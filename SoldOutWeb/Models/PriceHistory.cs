using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoldOutWeb.Models
{
    public class PriceHistory
    {
        public DateTime PricePeriod { get; set; }
        public double AveragePrice { get; set; }
    }
}