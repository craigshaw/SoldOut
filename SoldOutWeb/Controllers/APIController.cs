using SoldOutBusiness.Repository;
using SoldOutWeb.Models;
using SoldOutWeb.Services;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SoldOutWeb.Controllers
{
    public class APIController : Controller
    {
        private IStatsRepository _statsRepository;
        private ISoldOutRepository _repository;
        private PriceHistoryService _priceHistoryService;

        public APIController()
        {
            _statsRepository = new StatsRepository();
            _repository = new SoldOutRepository();
            _priceHistoryService = new PriceHistoryService(_repository);
        }

        [Route("Api/TopSellingProducts/{categoryId?}")]        
        public JsonResult TopSellingProducts(int? categoryId)
        {
            var topSellers = _statsRepository.TopSellingProducts(categoryId ?? 2, 10, 30);
            return Json(topSellers, JsonRequestBehavior.AllowGet);
        }

        [Route("Api/TopSellingCategories/{categoryId?}")]
        public JsonResult TopSellingCategories(int? categoryId)
        {
            var topSellers = _statsRepository.TopSellingCategories(categoryId ?? 2, 30);
            return Json(topSellers, JsonRequestBehavior.AllowGet);
        }

        [Route("Api/CandlestickData/{productId?}/{conditionId?}")]
        public JsonResult GetCandleStickChartDataForProduct(int? productId, int? conditionId)
        {
            var chartData = _statsRepository.GetTimeSeriesDataForProduct(productId ?? 1, conditionId ?? 2 );
            return Json(chartData, JsonRequestBehavior.AllowGet);
        }

        [Route("Api/MACData/{productId?}/{conditionId?}/{shortinterval?}/{longinterval?}")]
        public JsonResult GetMACDChartDataForProduct(int? productId, int? conditionId, int? shortInterval, int? longInterval)
        {
            var chartData = _statsRepository.GetTimeSeriesMACDDataForProduct(productId, conditionId ?? 2, shortInterval, longInterval);
            return Json(chartData, JsonRequestBehavior.AllowGet);
        }

        [Route("Api/PriceHistory/{productId}/{conditionId}")]
        public JsonResult PriceHistoryByCondition(int productId, int conditionId)
        {
            var priceHistory = CreatePriceHistory(productId, conditionId);

            return Json(priceHistory, JsonRequestBehavior.AllowGet);
        }

        [Route("Api/Scattergraph/{productId}")]
        public JsonResult ScattergraphDataForProduct(int productId)
        {
            var scatterGraphData = _statsRepository.GetScatterGraphDataForProduct(productId, 7);

            return Json(scatterGraphData, JsonRequestBehavior.AllowGet);
        }


        [Route("Api/Popular/{categoryId}/{conditionId}")]
        public JsonResult GetMostPopularProductsByCategoryAndCondition(int categoryId, int conditionId)
        {            
            var mostPopularProducts = _statsRepository.MostPopularProductsByCategoryAndCondition(categoryId, conditionId, 30);
            return Json(mostPopularProducts, JsonRequestBehavior.AllowGet);
        }

        [Route("Api/Expensive/{categoryId?}/{conditionId?}")]
        public JsonResult GetMostExpensiveProductsByCategoryAndCondition(int categoryId, int conditionId)
        {
            var mostExpensiveProducts = _statsRepository.MostExpensiveProductsByCategoryAndCondition(categoryId, conditionId, 30);
            return Json(mostExpensiveProducts, JsonRequestBehavior.AllowGet);
        }

        [Route("Api/MoversAndLosers/{categoryId}/{conditionId?}")]
        public JsonResult GetMoversAndLosersByCategoryAndCondition(int categoryId, int conditionId)
        {
            var mostExpensiveProducts = _statsRepository.GetMoversAndLosersByCategoryAndCondition(categoryId, conditionId, 30, 10);
            return Json(mostExpensiveProducts, JsonRequestBehavior.AllowGet);
        }

        [Route("Api/ProductsInCategory/{categoryId}")]
        public JsonResult GetProductsForCategory(int categoryId)
        {
            var productsInCategory = _statsRepository.GetProductsInCategory(categoryId);
            return Json(productsInCategory, JsonRequestBehavior.AllowGet);
        }

        [Route("Api/TopSellersInCategoryByNumberOfBuyers/{categoryId}")]
        public JsonResult TopSellersInCategoryByNumberOfBuyers(int? categoryId)
        {
            var _categorySales = _statsRepository.GetTopSellingProductsForCategoryByNumberOfBuyers(categoryId.HasValue ? categoryId.Value : 2, 7);
            return Json(_categorySales, JsonRequestBehavior.AllowGet);
        }


        private IEnumerable<PriceHistory> CreatePriceHistory(int productId)
        {
            int interval = 5;

            var basicPriceHistory = _priceHistoryService.CreateBasicPriceHistory(productId, 2, AggregationPeriod.Monthly); // TODO: Use the condition resolver here?
            _priceHistoryService.AddSimpleMovingAverage(basicPriceHistory, interval);
            _priceHistoryService.AddExponentialMovingAverage(basicPriceHistory, interval);

            return basicPriceHistory;
        }

        private IEnumerable<PriceHistory> CreatePriceHistory(int productId, int conditionId)
        {
            var allPriceHistory = _priceHistoryService.CreateBasicPriceHistory(productId, conditionId, AggregationPeriod.Monthly);

            return allPriceHistory;
        }
        [Route("Api/WeekdaySalesData/{categoryId?}")]
        public JsonResult GetWeeklySalesData(int? categoryId)
        {
            var salesData = _statsRepository.GetWeeklySalesDataByCategory(categoryId.HasValue ? categoryId.Value : 2);
            return Json(salesData, JsonRequestBehavior.AllowGet);
        }
    }
}