using SoldOutBusiness.Models;
using System;
using System.Collections.Generic;

namespace SoldOutBusiness.Repository
{
    public interface IStatsRepository : IDisposable
    {
        IEnumerable<ProductItemCount> MostPopularProductsByCondition(int conditionId, int numberToReturn, int daysToLookBack);

        IEnumerable<ProductItemCount> MostExpensiveProducts(int conditionId, int numberToReturn = 10, int daysToLookBack = 7);

        IEnumerable<ProductSaleSummary> TopSellingProducts(int categoryId, int numberToReturn, int daysToLookBack);
    }

    public class ProductItemCount
    {
        private int conditionId;

        public int ProductId { get; set; }
        public int ItemCount { get; set; }
        public double? AveragePrice { get; set; }
        public string ManufacturerCode { get; set; }
        public string Name { get; set; }

        public string Condition
        {
            get
            {
                if (this.conditionId == 1)
                    return "New";
                else
                {
                    return "Used";
                }
            }

            set
            {
                this.conditionId = Int32.Parse(value);
            }
        }
    }

    public class ProductSaleSummary
    {

        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Condition { get; set; }
        public string ManufacturerCode { get; set; }
        public string Category { get; set; }
        public int NumberSold { get; set; }

        public double AvgPrice { get; set; }

        public int Ranking { get; set; }

        public string AsOfDate { get; set; }
    }
}
