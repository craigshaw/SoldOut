namespace SoldOutBusiness.Models
{
    public class Price
    {
        public int PriceId { get; set; }
        public double Amount { get; set; }
        public bool IsRRP { get; set; }

        public string CurrencyCode { get; set; }
        public virtual Currency Currency { get; set; }

        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
    }
}
