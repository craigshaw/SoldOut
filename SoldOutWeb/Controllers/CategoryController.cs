//using SoldOutWeb.Models;
using SoldOutBusiness.Repository;
using SoldOutBusiness.Models;
using System.Web.Mvc;

namespace SoldOutWeb.Controllers
{
    public class CategoryController : Controller
    {
        private IStatsRepository _statsRepository;
        private ISoldOutRepository _repository;

        public CategoryController()
        {
            _statsRepository = new StatsRepository();
            _repository = new SoldOutRepository();
        }

        [Route("Category/{categoryId?}")]
        public ActionResult Category(int? categoryId)
        {
            int catId;

            catId = 2;

            if (categoryId != null) catId = categoryId.Value;
            
            var categoryInfo = _repository.GetCategoryByID(catId);

            //var category = new Category()
            //{
            //    Name = categoryInfo.Name,
            //    CategoryID = categoryInfo.CategoryID,
            //    ChildCategories = categoryInfo.Children
            //};

            return View("Category", categoryInfo);
        }
    }
}