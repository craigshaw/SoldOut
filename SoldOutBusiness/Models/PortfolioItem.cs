﻿using System;
using System.Collections.Generic;

namespace SoldOutBusiness.Models
{
    public class PortfolioItem
    {
        public int PortfolioItemId { get; set; }
        public int PortfolioId { get; set; }

        public ICollection<Product> Products { get; set; }

        public int Quantity { get; set; }

        public DateTime DatePurchased { get; set; }

        public Condition ItemCondition { get; set; }

        public Price PricePaid { get; set; }

        public PortfolioItem()
        {
            Products = new List<Product>();
        }
    }
}