using System.Collections.Generic;

namespace SoldOutBusiness.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }      
        public string YearOfRelease { get; set; }
        public string YearOfRetirement { get; set; }
        public string ManufacturerCode { get; set; } // Think Lego code, Addidas code, something to make a search unique apart from name

        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<Price> OriginalRRP { get; set; }

        public virtual ICollection<Product> ParentProducts { get; set; } // Ideally there should only be one parent Product but you never know...
        public virtual ICollection<Product> SubProducts { get; set; }

        public Product()
        {            
            Categories = new List<Category>();
            OriginalRRP = new List<Price>();         
        }
    }
}
