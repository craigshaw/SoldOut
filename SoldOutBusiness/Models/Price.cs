using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoldOutBusiness.Models
{
    public class Price
    {
        public double Amount { get; set; }

        public Ccy PricedIn { get; set; }

        public bool IsRRP { get; set; }
    }
}
