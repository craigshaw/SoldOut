using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoldOutBusiness.Models
{
    public class Portfolio
    {
        public int PortfolioId { get; set; }
        public string Name { get; set; }
        public List<PortfolioItem> PortfolioContents { get; set; }

        public Portfolio()
        {
            PortfolioContents = new List<PortfolioItem>();
        }
    }
}
