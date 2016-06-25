using System.Web.Mvc;
using SoldOutWeb.Repository;

namespace SoldOutWeb.Controllers
{
    public class SearchController : Controller
    {
        private IWebSearchRepository _repository;

        public SearchController()
        {
            _repository = new WebSearchRespository();
        }

        [Route("Search/{searchText}")]
        public ActionResult Search(string searchText)
        {
            var products = _repository.SearchForProduct(searchText);

            return View(products);
        }
    }
}