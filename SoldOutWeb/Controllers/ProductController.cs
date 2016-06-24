using SoldOutBusiness.Repository;
using SoldOutWeb.Models;
using System.Web.Mvc;
using SoldOutWeb.Services;

namespace SoldOutWeb.Controllers
{
    public class ProductController : Controller
    {
        private ISoldOutRepository _repository;
        private IPriceHistoryService _priceHistoryService;
        private IStatsRepository _statsRepository;

        public ProductController()
        {
            _repository = new SoldOutRepository();

            _statsRepository = new StatsRepository();

            _priceHistoryService = new PriceHistoryService(_repository);
        }


        [Route("Product/{id?}")]
        public ActionResult Product(int? id)
        {
            int productId = 1;

            if (id.HasValue)
                productId = id.Value;

            var search = _repository.GetSearchByProductID(productId);

            if (search == null)
                return new HttpNotFoundResult();

            SearchSummary summary = new SearchSummary()
            {
                Name = search.Name,
                Description = search.Description,
                SearchID = search.SearchId,
                LastRun = search.LastRun,
                TotalResults = _repository.ResultCount(search.SearchId),
                Link = search.Link,
                ProductID = productId
            };

            return View(summary);
        }

        [Route("Product/{productId?}/{categoryId?}")]
        public ActionResult Product(int? productId, int? conditionId)
        {
            int pId = 1;
            int cId = 0;

            string newTabContentClassString;
            string usedTabContentClassString;

            if (cId == 2)
            {
                newTabContentClassString = "tab-pane fade in active";
                usedTabContentClassString = "tab-pane fade";
            }
            else
            {
                newTabContentClassString = "tab-pane fade";
                usedTabContentClassString = "tab-pane fade in active";
            }


            if (productId.HasValue)
                pId = productId.Value;

            if (conditionId.HasValue)
                cId = conditionId.Value;

            var search = _repository.GetSearchByProductID(pId);

            SearchSummary summary = new SearchSummary()
            {
                Name = search.Name,
                Description = search.Description,
                SearchID = search.SearchId,
                LastRun = search.LastRun,
                TotalResults = _repository.ResultCount(search.SearchId),
                Link = search.Link,
                ProductID = pId,
                ConditionID = cId
            };

            return View(summary);
        }
    }
}