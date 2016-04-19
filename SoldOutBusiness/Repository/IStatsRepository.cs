using SoldOutBusiness.Models;
using System;
using System.Collections.Generic;

namespace SoldOutBusiness.Repository
{
    public interface IStatsRepository : IDisposable
    {
        IEnumerable<ProductItemCount> MostPopularProducts(int conditionId, int numberToReturn, int daysToLookBack);
        IEnumerable<ProductItemCount> MostExpensiveProducts(int conditionId, int numberToReturn = 10, int daysToLookBack = 7);
    }

    public class ProductItemCount
    {
        public int ProductId { get; set; }
        public int ItemCount { get; set; }
        public double? AveragePrice { get; set; }
        public string ManufacturerCode { get; set; }
        public string Name { get; set; }
    }
}
