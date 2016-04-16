using SoldOutBusiness.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
            // Most popular new
            var mostPopularNewProducts = _statsRepository.MostPopularProducts(2, 10, 30);
            return View(mostPopularNewProducts);
        }
    }
}