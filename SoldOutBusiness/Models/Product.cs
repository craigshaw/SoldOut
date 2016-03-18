using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SoldOutBusiness.Models
{
    public class Product
    {

        public int ProductId { get; set; }

        [Required]
        public virtual ICollection<Category> CategoryIds { get; set; }

        public string Name { get; set; }        

        public virtual ICollection<AliasCollection> Aliases { get; set; } // Think games with regional variation in their title

        public string YearOfRelease { get; set; }

        public string YearOfRetirement { get; set; }

        public virtual ICollection<Product> ParentProducts { get; set; } // Ideally there should only be one parent Product but you never know...

        public string ManufacturerCode { get; set; } // Think Lego code, Addidas code, something to make a search unique apart from name
        public virtual ICollection<Price> OriginalRRP { get; set; }

        public virtual ICollection<Product> SubProducts { get; set; }

        public Product()
        {            
            CategoryIds = new List<Category>();
            OriginalRRP = new List<Price>();         
        }

    }
}
