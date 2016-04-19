using SoldOutBusiness.DAL;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

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

        public IEnumerable<ProductItemCount> MostPopularProducts(int conditionId, int numberToReturn = 10, int daysToLookBack = 7)
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
