using System.Collections.Generic;

namespace SoldOutBusiness.Models
{
    public class Product
    {

        public int ProductId { get; set; }

        public ICollection<int> CategoryIds { get; set; }

        public string Name { get; set; }        

        public ICollection<ProductAlias> Aliases { get; set; }

        public string YearOfRelease { get; set; }

        public string YearOfRetirement { get; set; }

        public ICollection<int> ParentProductIds { get; set; }

        public string ManufacturerCode { get; set; }
        public ICollection<Price> OriginalRRP { get; set; }

        public Product()
        {
            ParentProductIds = new List<int>();
            CategoryIds = new List<int>();
            OriginalRRP = new List<Price>();
            Aliases = new List<ProductAlias>();
        }

    }
}
