using System;
using System.Collections.Generic;


namespace SoldOutBusiness.Models
{
    public class Category
    {
        public long CategoryId { get; set; }

        public string Name { get; set; }

        public long? ParentCategoryId { get; set; }

    }
}
