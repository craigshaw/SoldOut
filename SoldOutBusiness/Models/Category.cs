﻿using System.Collections.Generic;

namespace SoldOutBusiness.Models
{
    public class Category
    {
        public int CategoryID { get; set; }
        public string Name { get; set; }
        public bool IncludeInKeywordSearch { get; set; }

        public Category Parent { get; set; }
        public int? ParentCategoryId { get; set; }

        public virtual ICollection<Category> Children { get; set; }
        public virtual ICollection<Product> Products { get; set; }

        public Category()
        {
            Children = new List<Category>();
        }
    }
}
