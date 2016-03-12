namespace SoldOutWeb.Models
{
    public class PriceHistory
    {
        public string PricePeriod { get; set; }

        public double MinPrice { get; set; }

        public double MaxPrice { get; set; }

        public double AveragePrice { get; set; }

        public int Condition { get; set; }

        public double? SMA { get; set; }

        public double? EMA { get; set; }
    }
}