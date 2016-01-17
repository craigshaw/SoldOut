using System;
using System.Collections.Generic;
using eBay.Services;
using eBay.Services.Finding;

namespace SoldOutBusiness.Services
{
    public class EbayFinder : IEbayFinder
    {
        private FindingServicePortTypeClient _client;
        private ClientConfig _clientConfig;
        private Action<FindCompletedItemsRequest> _setRequestDefaults;

        public EbayFinder()
        {
            _clientConfig = new ClientConfig();
        }

        public IEbayFinder Configure(Action<ClientConfig> config)
        {
            // Configure
            config(_clientConfig);

            // Create a service client
            _client = FindingServiceClientFactory.getServiceClient(_clientConfig);

            return this;
        }

        public FindCompletedItemsResponse GetCompletedItems(string keywords, DateTime? since)
        {
            // Create a request to get our completed items
            var request = CreateCompletedItemsRequest(keywords, since); 

            return _client.findCompletedItems(request);
        }

        public FindCompletedItemsResponse GetCompletedItems(string keywords, DateTime? since, Action<FindCompletedItemsRequest> embellishRequest)
        {
            // Create a request to get our completed items
            var request = CreateCompletedItemsRequest(keywords, since);

            embellishRequest(request);

            return _client.findCompletedItems(request);
        }

        private FindCompletedItemsRequest CreateCompletedItemsRequest(string keywords, DateTime? since)
        {
            // Completed items, new only
            var request = new FindCompletedItemsRequest();

            // Search term
            request.keywords = keywords;

            // Filters to specify new, sold and GBP
            List<ItemFilter> filters = new List<ItemFilter>()
            {
                    new ItemFilter() { name = ItemFilterType.Condition, value = new string[] { "New", "1000" } },
                    new ItemFilter() { name = ItemFilterType.SoldItemsOnly, value = new string[] { "True" } },
                    new ItemFilter() { name = ItemFilterType.Currency, value = new string[] { "GBP" } }
            };

            // Have we recently updated ... if so, ask for items since we last updated
            if (since != null)
            {
                filters.Add(
                    new ItemFilter() { name = ItemFilterType.EndTimeFrom, value = new string[] { since.Value.ToString("s") } }
                    );
            }

            request.itemFilter = filters.ToArray();
            request.sortOrder = SortOrderType.EndTimeSoonest;

            return request;
        }

        public IEbayFinder SetRequestDefaults(Action<FindCompletedItemsRequest> setRequestDefaults)
        {
            _setRequestDefaults = setRequestDefaults;

            return this;
        }
    }
}
