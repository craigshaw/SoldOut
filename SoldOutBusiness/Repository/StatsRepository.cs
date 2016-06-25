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
            var data = _context.Database.SqlQuery<ProductSaleSummary>("exec GetTopSellingProductsByCategoryAndNumberItemsSold @CategoryId, @DaysToLookBack",
                new SqlParameter("@CategoryId", categoryId),
                new SqlParameter("@DaysToLookBack", daysToLookBack))
                .ToList(); // Force immediate execution

            return data;
        }

        public IEnumerable<ProductSaleSummary> TopSellingCategories(int categoryId, int daysToLookBack)
        {            
            var data = _context.Database.SqlQuery<ProductSaleSummary>("exec GetTopSellingCategoriesByNumberItemsSold @CategoryId, @DaysToLookBack",
                new SqlParameter("@CategoryId", categoryId),
                new SqlParameter("@DaysToLookBack", daysToLookBack))
                .ToList();

            return data;
        }

        public IEnumerable<ProductTimeSeriesData> GetTimeSeriesDataForProduct(int productId, int conditionId)
        {
            var timeData = _context.Database.SqlQuery<ProductTimeSeriesData>("exec GetTimeSeriesDataByProductID @ProductId, @ConditionId",
                new SqlParameter("@ProductId", productId),
                new SqlParameter("@ConditionId", conditionId))
                .ToList();
            
            return timeData.Skip(Math.Max(0, timeData.Count() - 60));
        }

        public IEnumerable<ProductTimeSeriesData> GetTimeSeriesMACDDataForProduct(int productId, int conditionId, int shortInterval, int longInterval, int daysToLookBack)
        {
            var _priceData =  _context.Database.SqlQuery<ProductTimeSeriesData>("exec GetTimeSeriesMACDDataByProductID @ProductId, @ConditionId, @ShortInterval, @LongInterval",
                new SqlParameter("@ProductId", productId),
                new SqlParameter("@ConditionId", conditionId),
                new SqlParameter("@ShortInterval", shortInterval),
                new SqlParameter("@LongInterval", longInterval))
                .ToList();

            return _priceData.Skip(Math.Max(0, _priceData.Count() - daysToLookBack));
        }

        public IEnumerable<Categories> GetCategories()
        {            
                return _context.Database.SqlQuery<Categories>("exec GetCategoryList").ToList();         
        }

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
           return _context.Database.SqlQuery<ProductItemCount>("exec GetMostPopularProductsByCategory @CategoryId, @ConditionId, @DaysToLookBack",
                new SqlParameter("@CategoryId", categoryId),
                new SqlParameter("@ConditionId", conditionId),
                new SqlParameter("@DaysToLookBack", daysToLookBack))
                .ToList()
               .Take(numberToTake);
        }

        public IEnumerable<ProductItemCount> MostExpensiveProductsByCategoryAndCondition(int categoryId, int conditionId, int daysToLookBack = 7, int numberToTake = 10)
        {
            return _context.Database.SqlQuery<ProductItemCount>("exec GetMostExpensiveProductsByCategory @CategoryId, @ConditionId, @DaysToLookBack",
                new SqlParameter("@CategoryId", categoryId),
                new SqlParameter("@ConditionId", conditionId),
                new SqlParameter("@DaysToLookBack", daysToLookBack))
                .ToList()
                .Take(numberToTake);
        }

        public IEnumerable<CategoryMoversAndLosersData> GetMoversAndLosersByCategoryAndCondition(int categoryId, int conditionId, int daysToLookBack = 7, int numberToTake = 10)
        {            
            var priceChangeData = _context.Database.SqlQuery<CategoryMoversAndLosersData>("exec GetPriceChangesOverPeriodByCategoryAndCondition @CategoryId, @ConditionId, @DaysToLookBack",
                new SqlParameter("@CategoryId", categoryId),
                new SqlParameter("@ConditionId", conditionId),
                new SqlParameter("@DaysToLookBack", daysToLookBack))
                .ToList();

            // return the top n movers and bottom n losers
            if (priceChangeData.Count() < (numberToTake * 2))
                return priceChangeData;
            else            
                return priceChangeData.Take(numberToTake).ToList().Union(priceChangeData.Skip(Math.Max(0, priceChangeData.Count() - numberToTake)));           
        }

        public IEnumerable<CategoryProducts> GetProductsInCategory(int categoryId)
        {            
            return _context.Database.SqlQuery<CategoryProducts>("exec GetProductsForCategory @CategoryId",
                new SqlParameter("@CategoryId", categoryId))
                .ToList();  
        }

        public IEnumerable<CategorySales> GetTopSellingProductsForCategoryByNumberOfBuyers(int categoryId, int daysToLookBack)
        {
            return _context.Database.SqlQuery<CategorySales>("exec GetTopSellingProductsForCategoryByNumberOfBuyers @CategoryId, @DaysToLookBack",
                new SqlParameter("@CategoryId", categoryId),
                new SqlParameter("@DaysToLookBack", daysToLookBack))
                .ToList();
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
        }

        public IEnumerable<ProductPriceScatterGraphData> GetScatterGraphDataForProduct(int productId, int interval)
        {
            return _context.Database.SqlQuery<ProductPriceScatterGraphData>("exec GetSalesDataByPeriodAndIntervalForProductScatterGraph @ProductId, @Interval",
                new SqlParameter("@ProductId", productId),
                new SqlParameter("@Interval", interval))
                .ToList();
        }

        public IEnumerable<WeekdaySalesData> GetWeeklySalesDataByCategory(int categoryId, int daysToLookBack = 7)
        {   
            return _context.Database.SqlQuery<WeekdaySalesData>("exec GetPriceStatisticsForCategoryByDayOfWeek @CategoryId, 7, @DaysToLookBack",
                new SqlParameter("@CategoryId", categoryId),
                new SqlParameter("@DaysToLookBack", daysToLookBack))
                .ToList();
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
