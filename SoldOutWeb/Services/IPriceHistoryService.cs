﻿using SoldOutWeb.Models;
using System.Collections.Generic;

namespace SoldOutWeb.Services
{
    public interface IPriceHistoryService
    {
        IList<PriceHistory> CreateBasicPriceHistory(int searchId);
        void AddSimpleMovingAverage(IList<PriceHistory> prices, int interval);
        void AddExponentialMovingAverage(IList<PriceHistory> prices, int interval);
    }
}
