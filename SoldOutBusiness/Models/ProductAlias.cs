using System.Collections.Generic;

namespace SoldOutBusiness.Models
{
    public class ProductAlias
    {
        public int AliasId { get; set; }

        public string Alias { get; set; }

        public ICollection<ProductRegion> Region { get; set; }

        public virtual Product ProductId { get; set; }
    }
}
