namespace SoldOutWeb.Models
{
    public class PriceHistory
    {
        public string PricePeriod { get; set; }

        public double MinPrice { get; set; }

        public double MaxPrice { get; set; }

        public double AveragePrice { get; set; }

        public double? SMA { get; set; }

        public double? EMA { get; set; }

        //    public double? MACD
        //{
        //    get
        //    {
        //        double? lowerMACD = 0.00;
        //        double? upperMACD = 0.00;

        //        this.Interval = 12;
        //        lowerMACD = this.EMA;

        //        this.Interval = 26;
        //        upperMACD = this.EMA;

        //        return lowerMACD - upperMACD;
        //    }
        //}
    }
}