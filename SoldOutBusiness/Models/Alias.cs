using System.Collections.Generic;

namespace SoldOutBusiness.Models
{
    public class Alias
    {
        public int AliasId { get; set; }

        public string Name { get; set; }
        public Region Region { get; set; }
    }
}
