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
        FindCompletedItemsResponse GetCompletedItems(Search search, int pageNumber);
        FindCompletedItemsResponse GetCompletedItems(Search search, int pageNumber, Action<FindCompletedItemsRequest> embellishRequest);
    }
}
