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
            int interval = 30;
            return View();
        }
    }
}