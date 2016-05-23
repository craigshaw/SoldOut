using System;
using System.Collections.Generic;

namespace SoldOutWeb.Models
{
    public class Category
    {
        public string Name { get; set; }
        public int CategoryID { get; set; }

        public ICollection<Category> ChildCategories { get; set; }
    }
}