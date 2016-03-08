using System.Collections.Generic;

namespace SoldOutBusiness.Models
{
    public class AliasCollection
    {
        public int AliasCollectionId { get; set; }

        public ICollection<Alias> Aliases { get; set; }
    }
}
