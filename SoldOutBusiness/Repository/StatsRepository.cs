using SoldOutBusiness.DAL;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Data.SqlClient;

namespace SoldOutBusiness.Repository
{
    public class StatsRepository : IStatsRepository
    {
        public SoldOutContext _context;

        public SoldOutContext SoldOutContext { set { _context = value; } }

        public StatsRepository()
        {
            _context = new SoldOutContext();

#if DEBUG
            _context.Database.Log = s => Debug.WriteLine(s);
#endif
        }

        public IEnumerable<ProductSaleSummary> TopSellingProducts(int categoryId, int numberToReturn, int daysToLookBack)
        {
            var data = _context.Database.SqlQuery<ProductSaleSummary>("exec GetTopSellingProductsByCategoryAndNumberItemsSold " + categoryId.ToString() + "," + daysToLookBack.ToString()).ToList();

            return data;
        }

        public IEnumerable<ProductSaleSummary> TopSellingCategories(int? categoryId, int daysToLookBack)
        {            
            var data = _context.Database.SqlQuery<ProductSaleSummary>("exec GetTopSellingCategoriesByNumberItemsSold " + categoryId.ToString() + "," + daysToLookBack.ToString()).ToList();

            return data;
        }

        public IEnumerable<ProductTimeSeriesData> GetTimeSeriesDataForProduct(int? productId, int? conditionId)
        {
            var condition = conditionId == null ? 2 : conditionId;

            var timeData = _context.Database.SqlQuery<ProductTimeSeriesData>("exec GetTimeSeriesDataByProductID " + productId.ToString() + "," + condition.ToString());
            
            return timeData.Skip(Math.Max(0, timeData.Count() - 60));
        }

        public IEnumerable<ProductTimeSeriesData> GetTimeSeriesMACDDataForProduct(int? productId, int? conditionId, int? shortInterval, int? longInterval, int? daysToLookBack = 100)
        {
            var condition = conditionId == null ? 2 : conditionId;
            var sInterval = shortInterval == null ? 20 : shortInterval.Value;
            var lInterval = longInterval == null ? 50 : longInterval.Value;

            var timeData = _context.Database.SqlQuery<ProductTimeSeriesData>("exec GetTimeSeriesMACDDataByProductID " + productId.ToString() + "," + condition.ToString() + "," + sInterval.ToString() + "," + lInterval.ToString());
            
            if (daysToLookBack.HasValue)
                return timeData.Skip(Math.Max(0, timeData.Count() - daysToLookBack.Value));
            else
                return timeData;
        }

        public IEnumerable<Categories> GetCategories()
        {            
                return _context.Database.SqlQuery<Categories>("exec GetCategoryList");         
        }
        

        //public IEnumerable<Pro>

        //public IEnumerable<ProductItemCount> MostPopularProducts(int categoryId, int numberToReturn = 10, int daysToLookBack = 7)
        //{
        //    return (from product in _context.Products
        //            join results in _context.SearchResults on product.ProductId equals results.ProductId into productResults
        //            from productResult in productResults
        //            where DbFunctions.AddDays(productResult.DateOfMatch, daysToLookBack) > DateTime.Now
        //            group productResult by new
        //            {
        //                ProductId = product.ProductId,
        //                ManufacturerCode = product.ManufacturerCode,
        //                Name = product.Name,
        //                ConditionId = productResult.ConditionId
        //            } into grouped
        //            orderby grouped.Count() descending
        //            select new ProductItemCount()
        //            {
        //                ProductId = grouped.Key.ProductId,
        //                ItemCount = grouped.Count(),
        //                AveragePrice = grouped.Average(sr => sr.Price),
        //                ManufacturerCode = grouped.Key.ManufacturerCode,
        //                Name = grouped.Key.Name,
        //                Condition = grouped.Key.ConditionId.ToString()
        //            }).Take(numberToReturn);
        //}


        public IEnumerable<ProductItemCount> MostPopularProductsByCondition(int conditionId, int numberToReturn = 10, int daysToLookBack = 7)
        {
            return (from product in _context.Products
                    join results in _context.SearchResults on product.ProductId equals results.ProductId into productResults                    
                    from productResult in productResults
                    where DbFunctions.AddDays(productResult.DateOfMatch, daysToLookBack) > DateTime.Now
                          && productResult.ConditionId == conditionId
                    group productResult by new { ProductId = product.ProductId,
                                       ManufacturerCode = product.ManufacturerCode,
                                       Name = product.Name }  into grouped
                    orderby grouped.Count() descending
                    select new ProductItemCount() { ProductId = grouped.Key.ProductId, ItemCount = grouped.Count(),
                        AveragePrice = grouped.Average(sr => sr.Price), ManufacturerCode = grouped.Key.ManufacturerCode,
                        Name = grouped.Key.Name}).Take(numberToReturn);
        }

        public IEnumerable<ProductItemCount> MostPopularProductsByCategoryAndCondition(int categoryId, int conditionId, int daysToLookBack = 7, int numberToTake = 10)
        {
           return _context.Database.SqlQuery<ProductItemCount>("exec GetMostPopularProductsByCategory " + categoryId.ToString() + "," + conditionId.ToString() + "," + daysToLookBack.ToString()).ToList().Take(numberToTake);
        }

        public IEnumerable<ProductItemCount> MostExpensiveProductsByCategoryAndCondition(int categoryId, int conditionId, int daysToLookBack = 7, int numberToTake = 10)
        {
            var tableData = _context.Database.SqlQuery<ProductItemCount>("exec GetMostExpensiveProductsByCategory " + categoryId.ToString() + "," + conditionId.ToString() + "," + daysToLookBack.ToString()).ToList();
            return tableData.Take(numberToTake);
        }

        public IEnumerable<CategoryMoversAndLosersData> GetMoversAndLosersByCategoryAndCondition(int categoryId, int conditionId, int daysToLookBack = 7, int numberToTake = 10)
        {
            
            var _priceChangeData = _context.Database.SqlQuery<CategoryMoversAndLosersData>("exec GetPriceChangesOverPeriodByCategoryAndCondition " + categoryId.ToString() + "," + conditionId.ToString() + "," + daysToLookBack.ToString());

            // return the top n movers and bottom n losers
            if (_priceChangeData.Count() < (numberToTake * 2))
                return _priceChangeData;
            else            
                return _priceChangeData.Take(numberToTake).ToList().Union(_priceChangeData.Skip(Math.Max(0, _priceChangeData.Count() - numberToTake)));           
        }

        public IEnumerable<CategoryProducts> GetProductsInCategory(int categoryId)
        {            
            return _context.Database.SqlQuery<CategoryProducts>("exec GetProductsForCategory " + categoryId.ToString());  
        }

        public IEnumerable<CategorySales> GetTopSellingProductsForCategoryByNumberOfBuyers(int categoryId, int daysToLookBack)
        {
            return _context.Database.SqlQuery<CategorySales>("exec GetTopSellingProductsForCategoryByNumberOfBuyers " + categoryId.ToString() + "," + daysToLookBack.ToString());
        }


        public IEnumerable<ProductItemCount> MostExpensiveProducts(int conditionId, int numberToReturn = 10, int daysToLookBack = 7)
        {
            // TODO: Confirm this one ... it's grouping by product, should it? It's also looking at DateOfMatch, should probably use item end time
            return (from product in _context.Products
                    join results in _context.SearchResults on product.ProductId equals results.ProductId into productResults
                    from productResult in productResults
                    where DbFunctions.AddDays(productResult.DateOfMatch, daysToLookBack) > DateTime.Now
                          //&& productResult.ConditionId == conditionId
                    group productResult by new
                    {
                        ProductId = product.ProductId,
                        ManufacturerCode = product.ManufacturerCode,
                        Name = product.Name
                    } into grouped
                    orderby grouped.Max(sr => sr.Price) descending
                    select new ProductItemCount
                    {
                        ProductId = grouped.Key.ProductId,
                        ItemCount = 0,
                        AveragePrice = grouped.Max(sr => sr.Price),
                        ManufacturerCode = grouped.Key.ManufacturerCode,
                        Name = grouped.Key.Name
                    }).Take(numberToReturn);


            //(from sr in _context.SearchResults
            //        where DbFunctions.AddDays(sr.DateOfMatch, daysToLookBack) > DateTime.Now
            //              //&& sr.ConditionId == conditionId
            //        group sr by sr.ProductId into grouped
            //        from g in grouped
            //        orderby g.Price descending
            //        select g).Take(numberToReturn);
        }

        public IEnumerable<ProductPriceScatterGraphData> GetScatterGraphDataForProduct(int productId, int interval)
        {
            var scatterGraphData = _context.Database.SqlQuery<ProductPriceScatterGraphData>("exec GetSalesDataByPeriodAndIntervalForProductScatterGraph " + productId.ToString() + "," + interval.ToString()).ToList();

            return scatterGraphData;
        }

        public IEnumerable<WeekdaySalesData> GetWeeklySalesDataByCategory(int? categoryId, int daysToLookBack = 7)
        {   
            var _salesData = _context.Database.SqlQuery<WeekdaySalesData>("exec GetPriceStatisticsForCategoryByDayOfWeek " + categoryId.ToString() + ",7," + daysToLookBack.ToString()).ToList();

            return _salesData;
        }

        #region IDisposable Support
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_context != null)
                {
                    _context.Dispose();
                    _context = null;
                }
            }
        }
        #endregion
    }
}
