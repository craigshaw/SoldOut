using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoldOutBusiness.Models
{
    public class Category
    {
        public int CategoryID { get; set; }
        public int ParentCategoryId { get; set; }
        public string Name { get; set; }

        public bool IncludeInKeywordSearch { get; set; }
    }
}
