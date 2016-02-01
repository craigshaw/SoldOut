using SoldOutBusiness.Repository;
using System.Web.Mvc;

namespace SoldOutWeb.Controllers
{
    public class HomeController : Controller
    {
        private ISearchRepository _repository;

        public HomeController()
        {
            _repository = new SearchRepository();
        }

        public ActionResult Index()
        {
            var searches = _repository.GetAllSearches();
            return View(searches);
        }

        public ActionResult Search(int id)
        {
            var search = _repository.GetSearchByID(id);

            return View(id);
        }

        //public ActionResult About()
        //{
        //    ViewBag.Message = "Your application description page.";

        //    return View();
        //}

        //public ActionResult Contact()
        //{
        //    ViewBag.Message = "Your contact page.";

        //    return View();
        //}
    }
}