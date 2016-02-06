using SoldOutBusiness.Repository;
using SoldOutWeb.Models;
using System;
using System.Collections.Generic;
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
            var priceHistory = CreatePriceHistory(id);

            SearchSummary summary = new SearchSummary()
            {
                Name = search.Name,
                Description = search.Description,
                SearchID = id,
                LastRun = search.LastRun,
                PriceHistory = priceHistory,
                TotalResults = _repository.ResultCount(id),
                Link = search.Link
            };

            return View(summary);
        }

        public JsonResult SearchSummary(int id)
        {
            var priceHistory = CreatePriceHistory(id);

            return Json(priceHistory, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<PriceHistory> CreatePriceHistory(int searchId)
        {
            var results = _repository.GetSearchResults(searchId).ToList();

            return from item in results
                       group item by new { item.EndTime.Value.Month, item.EndTime.Value.Year } into grp
                       orderby grp.Key.Year, grp.Key.Month
                       select new PriceHistory() { PricePeriod = new DateTime(grp.Key.Year, grp.Key.Month, 1), AveragePrice = (double)(grp.Average(it => it.Price)) };
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