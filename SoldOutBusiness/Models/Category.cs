using System.Collections.Generic;

namespace SoldOutBusiness.Models
{
    public class Category
    {
        public int CategoryID { get; set; }
        public int ParentCategoryId { get; set; }
        public string Name { get; set; }

        public ICollection<AliasCollection> Aliases { get; set; }

        public virtual IEnumerable<Category> Children { get; set; }

        public virtual ICollection<Product> Products { get; set; }

        public bool IncludeInKeywordSearch { get; set; }

        public Category()
        {
            Children = new List<Category>();
        }
    }
}
