using eBay.Services;
using eBay.Services.Finding;
using SoldOutBusiness.Models;
using System;

namespace SoldOutBusiness.Services
{
    public interface IEbayFinder
    {
        IEbayFinder Configure(Action<ClientConfig> config);
        IEbayFinder SetRequestDefaults(Action<FindCompletedItemsRequest> setRequestDefaults);
        FindCompletedItemsResponse GetCompletedItems(Search search);
        FindCompletedItemsResponse GetCompletedItems(Search search, Action<FindCompletedItemsRequest> embellishRequest);
    }
}
