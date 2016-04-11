using SoldOutBusiness.Repository;
using SoldOutWeb.Models;
using System.Collections.Generic;
using System.Web.Mvc;
using SoldOutWeb.Services;

namespace SoldOutWeb.Controllers
{
    public class SearchController : Controller
    {
        private ISoldOutRepository _repository;
        private IPriceHistoryService _priceHistoryService;

        public SearchController()
        {
            _repository = new SoldOutRepository();

            _priceHistoryService = new PriceHistoryService(_repository);
        }

        [Route("Searches/All", Name = "Searches")]
        public ActionResult All()
        {
            var searches = _repository.GetAllSearchesWithResults();

            //var categoryParents = _repository.GetParentCategories();

            //var categorySummary = _repository.GetCategorySummaryByCategoryId(categoryParents);

            return View(searches);
        }

        [Route("Summmary/{id?}")]
        public ActionResult Summary(int? id)
        {
            int searchId = 1;

            if (id.HasValue)
                searchId = id.Value;

            var search = _repository.GetSearchByID(searchId);

            SearchSummary summary = new SearchSummary()
            {
                Name = search.Name,
                Description = search.Description,
                SearchID = searchId,
                LastRun = search.LastRun,
                TotalResults = _repository.ResultCount(searchId),
                Link = search.Link
            };

            return View(summary);
        }

        [Route("PriceHistory/{id}")]
        public JsonResult PriceHistory(int id)
        {
            var priceHistory = CreatePriceHistory(id);

            return Json(priceHistory, JsonRequestBehavior.AllowGet);
        }

        [Route("PriceHistory/{id}/{conditionId}")]
        public JsonResult PriceHistoryByCondition(int id, int conditionId)
        {
            var priceHistory = CreatePriceHistory(id, conditionId);

            return Json(priceHistory, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<PriceHistory> CreatePriceHistory(int searchId)
        {
            int interval = 5;

            var basicPriceHistory = _priceHistoryService.CreateBasicPriceHistory(searchId, 2, AggregationPeriod.Monthly); // TODO: Use the condition resolver here?
            _priceHistoryService.AddSimpleMovingAverage(basicPriceHistory, interval);
            _priceHistoryService.AddExponentialMovingAverage(basicPriceHistory, interval);

            return basicPriceHistory;
        }

        private IEnumerable<PriceHistory> CreatePriceHistory(int searchId, int conditionId)
        {
            var allPriceHistory = _priceHistoryService.CreateBasicPriceHistory(searchId, conditionId, AggregationPeriod.Monthly);

            //_priceHistoryService.AddSimpleMovingAverage(basicPriceHistory, interval);
            //_priceHistoryService.AddExponentialMovingAverage(basicPriceHistory, interval);

            return allPriceHistory;
        }
    }
}