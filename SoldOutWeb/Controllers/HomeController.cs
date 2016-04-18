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

        [Route("Api/Popular/{conditionId}")]
        public JsonResult Popular(int conditionId)
        {
            var mostPopularProducts = _statsRepository.MostPopularProducts(conditionId, 10, 30);
            return Json(mostPopularProducts, JsonRequestBehavior.AllowGet);
        }
    }
}