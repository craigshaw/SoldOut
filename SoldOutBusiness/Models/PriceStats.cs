namespace SoldOutBusiness.Models
{
    public class PriceStats
    {
        public double OriginalRRP { get; set; }
        public double AverageSalePrice { get; set; }
        public double StandardDeviation { get; set; }
        public int NumberOfResults { get; set; }
    }
}
