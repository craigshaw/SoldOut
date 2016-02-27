using SoldOutBusiness.Repository;
using SoldOutWeb.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System;

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

        private void AddSMA(PriceHistory[] basicPriceHistory, int interval)
        {
            for (int i = 0; i < basicPriceHistory.Length; i++)
            {
                double simpleMovingAverage = 0.00d;

                if (i <= interval - 2)
                {
                    basicPriceHistory[i].SMA = null;
                    continue;
                }

                for (int j = (i - (interval - 1)); j < i; j++)
                {
                    simpleMovingAverage += basicPriceHistory[j].AveragePrice;
                }

                simpleMovingAverage += basicPriceHistory[i].AveragePrice; // Add ourselves
                basicPriceHistory[i].SMA = simpleMovingAverage / interval;
            }
        }

        private IEnumerable<PriceHistory> CreatePriceHistory(int searchId)
        {
            var search = _repository.GetSearchByID(searchId);
            var results = _repository.GetSearchResults(searchId).ToList();

            int interval = 5;

            var basicPriceHistory = (from item in results
                        group item by new { item.EndTime.Value.Day, item.EndTime.Value.Month, item.EndTime.Value.Year } into grp
                        orderby grp.Key.Year, grp.Key.Month, grp.Key.Day
                        select new PriceHistory()
                        {
                            PricePeriod = System.DateTime.Parse($"{grp.Key.Day}/{grp.Key.Month:D2}/{grp.Key.Year}"),
                            AveragePrice = (double)(grp.Average(it => it.Price)),
                            MinPrice = (double)(grp.Min(it => it.Price)),
                            MaxPrice = (double)(grp.Max(it => it.Price)),
                        }).ToArray();


            AddSMA(basicPriceHistory, interval);

            AddEMA(basicPriceHistory, interval);

            return basicPriceHistory;
        }

        private void AddEMA(PriceHistory[] basicPriceHistory, int interval)
        {
            double multiplier = 1;
            double priorPrices = 0.00;

            multiplier = 2 / (interval + 1);

            //for (int j = 0; j < interval - 1; j++)
            //{
            //    priorPrices += basicPriceHistory[j].AveragePrice;
            //}

            //priorPrices = priorPrices / interval;

            for (int i = 0; i < basicPriceHistory.Length; i++)
            {
                if (i <= interval - 2)
                {
                    if (i == 0)
                    {
                        basicPriceHistory[i].EMA = null;
                    }
                    else
                    {
                        basicPriceHistory[i].EMA = basicPriceHistory[i - 1].AveragePrice;
                    }
                    continue;
                }

                basicPriceHistory[i].EMA = basicPriceHistory[i].AveragePrice * multiplier + basicPriceHistory[i-1].EMA * (1 - multiplier);
            }
        }
    }
}