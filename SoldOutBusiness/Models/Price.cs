using System.ComponentModel.DataAnnotations;

namespace SoldOutBusiness.Models
{
    public class Price
    {

        public int PriceId { get; set; }
        public double Amount { get; set; }

        public Ccy PricedIn { get; set; }

        public bool IsRRP { get; set; }
    }
}
