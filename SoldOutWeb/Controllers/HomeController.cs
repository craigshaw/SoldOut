using SoldOutBusiness.Repository;
using System.Web.Mvc;

namespace SoldOutWeb.Controllers
{
    [Route("{action=Home}", Name = "Home")]
    public class HomeController : Controller
    {
        private IStatsRepository _statsRepository;

        public HomeController()
        {
            _statsRepository = new StatsRepository();
        }

        // GET: Home
        public ActionResult Home()
        {
            return View();
        }

        [Route("Api/TopSellers/")]
        public JsonResult Popular()
        {
            var topSellers = _statsRepository.TopSellingProducts(2, 10, 30);
            return Json(topSellers, JsonRequestBehavior.AllowGet);
        }


        [Route("Api/Popular/{conditionId}")]
        public JsonResult Popular(int? conditionId)
        {
            int cId = conditionId.HasValue ? conditionId.Value : 2;
            var mostPopularProducts = _statsRepository.MostPopularProductsByCondition(cId, 10, 30);
            return Json(mostPopularProducts, JsonRequestBehavior.AllowGet);
        }

        [Route("Api/Expensive/")]
        public JsonResult Expensive()
        {
            var mostExpensiveProducts = _statsRepository.MostExpensiveProducts(2, 10, 30);
            return Json(mostExpensiveProducts, JsonRequestBehavior.AllowGet);
        }
    }
}