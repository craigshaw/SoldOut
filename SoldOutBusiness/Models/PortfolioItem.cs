using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoldOutBusiness.Models
{
    public class PortfolioItem
    {
        public int PortfolioItemId { get; set; }
        public int PortfolioId { get; set; }

        public List<Product> Products { get; set; }

        public int Quantity { get; set; }

        public DateTime DatePurchased { get; set; }

        public Condition ItemCondition { get; set; }

        public Price PricePaid { get; set; }

        public PortfolioItem()
        {
            Products = new List<Product>();
        }
    }
}
