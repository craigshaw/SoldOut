using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoldOutBusiness.Models
{
    public class Product
    {

        public int ProductId { get; set; }

        public List<int> CategoryIds { get; set; }

        public string Name { get; set; }

        public string YearOfRelease { get; set; }

        public string YearOfRetirement { get; set; }

        public List<int> ParentProductIds { get; set; }

        public string ManufacturerCode { get; set; }
        public List<Price> OriginalRRP { get; set; }

        public Product()
        {
            ParentProductIds = new List<int>();
            CategoryIds = new List<int>();
            OriginalRRP = new List<Price>();
        }

    }
}
