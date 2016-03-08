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

        public ICollection<int> ParentProductIds { get; set; } // Ideally there should only be one parent Product but you never know...

        public string ManufacturerCode { get; set; } // Think Lego code, Addidas code, something to make a search unique apart from name
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
