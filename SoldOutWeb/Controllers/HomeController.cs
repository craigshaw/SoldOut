using SoldOutBusiness.Repository;
using SoldOutWeb.Models;
using System;
using System.Linq;
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
            var results = _repository.GetSearchResults(id).ToList();

            var avgs = from item in results
                       group item by new { item.EndTime.Value.Month, item.EndTime.Value.Year } into grp
                       orderby grp.Key.Year, grp.Key.Month
                       select new PriceHistory (){ PricePeriod = new DateTime(grp.Key.Year, grp.Key.Month, 1), AveragePrice = (double)(grp.Average(it => it.Price)) };

            SearchSummary summary = new SearchSummary()
            {
                Name = search.Name,
                Description = search.Description,
                LastRun = search.LastRun,
                PriceHistory = avgs,
                TotalResults = results.Count
            };

            return View(summary);
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