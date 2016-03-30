namespace SoldOutBusiness.Models
{
    /// <summary>
    /// Used by the SP, GetPriceStatsForSearch
    /// </summary>
    public class PriceStats
    {
        public double AverageSalePrice { get; set; }
        public double StandardDeviation { get; set; }
        public int NumberOfResults { get; set; }
    }
}
