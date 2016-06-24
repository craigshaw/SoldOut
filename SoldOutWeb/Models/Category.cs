using SoldOutBusiness.Repository;
using System.Collections.Generic;

namespace SoldOutWeb.Models
{
    public class Category
    {
        private IStatsRepository _statsRepository;
        private IEnumerable<Categories> _categoryList;

        public Category()
        {
            _statsRepository = new StatsRepository();

            _categoryList = _statsRepository.GetCategories();
        }

        public IEnumerable<Categories> GetChildCategoriesForParent(int categoryId)
        {
            return null;
        }        
    }
}