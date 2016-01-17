using eBay.Services;
using eBay.Services.Finding;
using System;

namespace SoldOutBusiness.Services
{
    public interface IEbayFinder
    {
        IEbayFinder Configure(Action<ClientConfig> config);
        IEbayFinder SetRequestDefaults(Action<FindCompletedItemsRequest> setRequestDefaults);
        FindCompletedItemsResponse GetCompletedItems(string keywords, DateTime? since);
        FindCompletedItemsResponse GetCompletedItems(string keywords, DateTime? since, Action<FindCompletedItemsRequest> embellishRequest);
    }
}
