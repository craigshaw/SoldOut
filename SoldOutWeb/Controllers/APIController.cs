﻿using SoldOutBusiness.Repository;
using SoldOutWeb.Models;
using SoldOutWeb.Services;
using System.Collections.Generic;
using System.Web.Mvc;
using System;
using SoldOutBusiness.Models;

namespace SoldOutWeb.Controllers
{
    class ProductPriceStats
    {
        public PriceStats PriceStats { get; set; }
        public int ProductId { get; set; }
    }

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

        [Route("Api/PriceHistory/{manufacturerCode}/{conditionId?}")]
        public ActionResult PriceHistory(string manufacturerCode, int? conditionId)
        {
            var product = _repository.GetProductByManufacturerCode(manufacturerCode);

            if( product == null)
                return new HttpNotFoundResult();

            var search = _repository.GetSearchByProductID(product.ProductId);
            var priceHistory = _repository.GetPriceStatsForSearchMonkeySuspiciousItemReviewer(search.SearchId, conditionId ?? 2);

            var stats = new ProductPriceStats()
            {
                PriceStats = priceHistory,
                ProductId = product.ProductId
            };

            return Json(stats, JsonRequestBehavior.AllowGet);
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

        [Route("Api/MACData/{productId?}/{conditionId?}/{shortinterval?}/{longinterval?}/{daysToLookBack?}")]
        public JsonResult GetMACDChartDataForProduct(int? productId, int? conditionId, int? shortInterval, int? longInterval, int? daysToLookBack)
        {
            var chartData = _statsRepository.GetTimeSeriesMACDDataForProduct(productId ?? 1, conditionId ?? 2, shortInterval ?? 20, longInterval ?? 50, daysToLookBack ?? 60);

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
            var scatterGraphData = _statsRepository.GetScatterGraphDataForProduct(productId, 30);

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
        public JsonResult GetMoversAndLosersByCategoryAndCondition(int categoryId, int? conditionId)
        {
            var mostExpensiveProducts = _statsRepository.GetMoversAndLosersByCategoryAndCondition(categoryId, conditionId ?? 2, 30, 10);
            return Json(mostExpensiveProducts, JsonRequestBehavior.AllowGet);
        }

        [Route("Api/ProductsInCategory/{categoryId}")]
        public JsonResult GetProductsForCategory(int categoryId)
        {
            var productsInCategory = _statsRepository.GetProductsInCategory(categoryId);
            return Json(productsInCategory, JsonRequestBehavior.AllowGet);
        }

        [Route("Api/TopSellersInCategoryByNumberOfBuyers/{categoryId?}/{interval?}")]
        public JsonResult TopSellersInCategoryByNumberOfBuyers(int? categoryId, int? interval)
        {
            var _categorySales = _statsRepository.GetTopSellingProductsForCategoryByNumberOfBuyers(categoryId ?? 2, 7);

            return Json(_categorySales, JsonRequestBehavior.AllowGet);
        }

        [Route("Api/Menu")]
        public JsonResult GetMenu()
        {
            var _menuitems = _statsRepository.GetCategories();
            return Json(_menuitems, JsonRequestBehavior.AllowGet);
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
        [Route("Api/WeekdaySalesData/{categoryId?}/{conditionId}")]
        public JsonResult GetWeeklySalesData(int? categoryId, int conditionId)
        {
            var salesData = _statsRepository.GetWeeklySalesDataByCategory(categoryId ?? 2, conditionId);
            return Json(salesData, JsonRequestBehavior.AllowGet);
        }

        [Route("Api/WeekdaySalesDataForProduct/{productId}/{conditionId}")]
        public JsonResult GetWeeklySalesDataforProduct(int productId, int conditionId)
        {
            var salesData = _statsRepository.GetWeeklySalesDataByProduct(productId, conditionId);
            return Json(salesData, JsonRequestBehavior.AllowGet);
        }
    }
}