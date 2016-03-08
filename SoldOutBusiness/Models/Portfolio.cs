using System.Collections.Generic;

namespace SoldOutBusiness.Models
{
    public class Portfolio
    {
        public int PortfolioId { get; set; }
        public string Name { get; set; }
        public ICollection<PortfolioItem> PortfolioContents { get; set; }

        public Portfolio()
        {
            PortfolioContents = new List<PortfolioItem>();
        }
    }
}
