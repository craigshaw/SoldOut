using SoldOutBusiness.Models;
using System;
using System.Collections.Generic;

namespace SoldOutBusiness.Repository
{
    public interface IStatsRepository : IDisposable
    {
        IEnumerable<ProductItemCount> MostPopularProductsByCondition(int conditionId, int numberToReturn, int daysToLookBack);

        IEnumerable<ProductItemCount> MostPopularProductsByCategoryAndCondition(int categoryId, int conditionId, int daysToLookBack, int numberToTake = 10);

        IEnumerable<ProductItemCount> MostExpensiveProductsByCategoryAndCondition(int categoryId, int conditionId, int daysToLookBack, int numberToTake = 10);

        IEnumerable<ProductItemCount> MostExpensiveProducts(int conditionId, int numberToReturn = 10, int daysToLookBack = 7);

        IEnumerable<ProductSaleSummary> TopSellingProducts(int categoryId, int numberToReturn, int daysToLookBack);

        //IEnumerable<ProductSaleSummary> TopSellingProducts(int categoryId, int conditionId, int numberToReturn, int daysToLookBack);

        IEnumerable<ProductSaleSummary> TopSellingCategories(int categoryId, int daysToLookBack);

        IEnumerable<ProductTimeSeriesData> GetTimeSeriesDataForProduct(int productId, int conditionId);

        IEnumerable<ProductTimeSeriesData> GetTimeSeriesMACDDataForProduct(int? productId, int? conditionId, int? shortInterval, int? longInterval);

        IEnumerable<ProductPriceScatterGraphData> GetScatterGraphDataForProduct(int productId, int interval);

        IEnumerable<CategoryMoversAndLosersData> GetMoversAndLosersByCategoryAndCondition(int categoryId, int conditionId, int daysToLookBack = 7, int numberToTake = 10);

        IEnumerable<CategoryProducts> GetProductsInCategory(int categoryId);

        IEnumerable<CategorySales> GetTopSellingProductsForCategoryByNumberOfBuyers(int categoryId, int daysToLookBack = 7);

        IEnumerable<WeekdaySalesData> GetWeeklySalesDataByCategory(int? categoryId, int daysToLookBack = 7);

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

        public int CategoryId { get; set; }

        public int ConditionId { get; set; }

        public string Name { get; set; }
        public string Condition { get; set; }
        public string ManufacturerCode { get; set; }
        public string Category { get; set; }
        public int NumberSold { get; set; }

        public double AvgPrice { get; set; }

        public int Ranking { get; set; }

        public string AsOfDate { get; set; }
    }

    public class ProductTimeSeriesData
    {
        //public float serialDate { get; set; }
        public string RepresentativeDate { get; set; }

        public double? OpenPrice { get; set; }

        public double MinPrice { get; set; }

        public double MaxPrice { get; set; }

        public double? ClosePrice { get; set; }

        public double? ShortEMA { get; set; }
        public double? LongEMA { get; set; }
        public double? MACD { get; set; }
        public double? SignalLine { get; set; }
        public double? HistogramData { get; set; }
        public double? AvgPrice { get; set; }
    }

    public class ProductPriceScatterGraphData
    {
        public int ProductId { get; set; }
        public string Condition { get; set; }

        public string EndTime { get; set; }

        public double Price { get; set; }
    }

    public class CategoryMoversAndLosersData
    {
        public int ProductId { get; set; }

        public string Name { get; set; }

        public string ManufacturerCode { get; set; }


        public string Condition { get; set; }
        public DateTime MinSearchDate { get; set; }
        public double MinAvgPrice { get; set; }
        public DateTime MaxSearchDate { get; set; }
        public double MaxAvgPrice { get; set; }

        public double PercentPriceChange { get; set; }

    }

    public class CategoryProducts
    {

        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string CategoryName { get; set; }
        public string ManufacturerCode { get; set; }

        public string YearOfRelease { get; set; }
    }

    public class CategorySales
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string ManufacturerCode { get; set; }
        public string Category { get; set; }
        public int CategoryId { get; set; }
        public int NumberOfBidders { get; set; }
        public double AvgPrice { get; set; }
        public DateTime AsOfDate { get; set; }

    }

    public class WeekdaySalesData
    {
        public string DayName { get; set; }

        public int NumberOfBidders { get; set; }
        public int NumberOfItemsSold { get; set; }
    }
}
