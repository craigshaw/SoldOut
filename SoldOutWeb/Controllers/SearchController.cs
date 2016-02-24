using SoldOutBusiness.Repository;
using SoldOutWeb.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SoldOutWeb.Controllers
{
    public class SearchController : Controller
    {
        private ISearchRepository _repository;

        public SearchController()
        {
            _repository = new SearchRepository();
        }

        public ActionResult All()
        {
            var searches = _repository.GetAllSearchesWithResults();
            return View(searches);
        }

        public ActionResult Summary(int? id)
        {
            int searchId = 1;

            if (id.HasValue)
                searchId = id.Value;

            var search = _repository.GetSearchByID(searchId);
            var priceHistory = CreatePriceHistory(searchId);

            SearchSummary summary = new SearchSummary()
            {
                Name = search.Name,
                Description = search.Description,
                SearchID = searchId,
                LastRun = search.LastRun,
                PriceHistory = priceHistory,
                TotalResults = _repository.ResultCount(searchId),
                Link = search.Link
            };

            return View(summary);
        }

        public JsonResult PriceHistory(int id)
        {
            var priceHistory = CreatePriceHistory(id);

            return Json(priceHistory, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<PriceHistory> CreatePriceHistory(int searchId)
        {
            var results = _repository.GetSearchResults(searchId).ToList();

            return from item in results
                        group item by new { item.EndTime.Value.Day, item.EndTime.Value.Month, item.EndTime.Value.Year } into grp
                        orderby grp.Key.Year, grp.Key.Month, grp.Key.Day
                        select new PriceHistory()
                        {
                            PricePeriod = System.DateTime.Parse($"{grp.Key.Day}/{grp.Key.Month:D2}/{grp.Key.Year}"),
                            AveragePrice = (double)(grp.Average(it => it.Price)),
                            MinPrice = (double)(grp.Min(it => it.Price)),
                            MaxPrice = (double)(grp.Max(it => it.Price)),
                        };
        }
    }
}