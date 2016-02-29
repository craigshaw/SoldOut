using System;
using System.Collections.Generic;

namespace SoldOutBusiness.Models
{
    public class Product
    {

        public long ProductID { get; set; }

        public string Name { get; set; }

        public ICollection<Category> Categories { get; set; }

        public ICollection<Product> RelatedProducts { get; set; }
    }
}
