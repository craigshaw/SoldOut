using System;
using System.Collections.Generic;
using eBay.Services;
using eBay.Services.Finding;
using SoldOutBusiness.Models;

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

        public FindCompletedItemsResponse GetCompletedItems(SoldOutBusiness.Models.Search search)
        {
            // Create a request to get our completed items
            var request = CreateCompletedItemsRequest(search); 

            return _client.findCompletedItems(request);
        }

        public FindCompletedItemsResponse GetCompletedItems(SoldOutBusiness.Models.Search searchItem, Action<FindCompletedItemsRequest> embellishRequest)
        {
            // Create a request to get our completed items
            var request = CreateCompletedItemsRequest(searchItem);

            embellishRequest(request);

            return _client.findCompletedItems(request);
        }

        private FindCompletedItemsRequest CreateCompletedItemsRequest(Search searchItem)
        {
            // Completed items, new only
            var request = new FindCompletedItemsRequest();
            List<ItemFilter> filters = new List<ItemFilter>();

            // Search term
            foreach (SearchCriteria criteria in searchItem.SearchCriteria)
            {
                switch (criteria.Criteria)
                {
                    case "Title":
                        request.keywords = criteria.Value;
                        break;

                    case "Condition":
                        filters.Add(new ItemFilter() { name = ItemFilterType.Condition, value = new string[] { criteria.Value, "1000" } });
                        break;

                    case "SoldItemsOnly":
                        filters.Add(new ItemFilter() { name = ItemFilterType.SoldItemsOnly, value = new string[] { criteria.Value } });
                        break;

                    case "Currency":
                        filters.Add(new ItemFilter() { name = ItemFilterType.Currency, value = new string[] { criteria.Value } });
                        break;
                }
            }

            // Filters to specify new, sold and GBP
            // Wire up default values

            if (String.IsNullOrEmpty(request.keywords))
                request.keywords = searchItem.Name;

            if (filters.Find(f => f.name == ItemFilterType.Currency) == null)
                filters.Add(new ItemFilter() { name = ItemFilterType.Currency, value = new string[] { "GBP" } });

            if (filters.Find(f => f.name == ItemFilterType.Condition) == null)
                filters.Add(new ItemFilter() { name = ItemFilterType.Condition, value = new string[] { "New" , "1000"} });

            if (filters.Find(f => f.name == ItemFilterType.SoldItemsOnly) == null)
                filters.Add(new ItemFilter() { name = ItemFilterType.SoldItemsOnly, value = new string[] { "True" } });

            // Have we recently updated ... if so, ask for items since we last updated
            if (searchItem.LastRun != null)
            {
                filters.Add(
                    new ItemFilter() { name = ItemFilterType.EndTimeFrom, value = new string[] { searchItem.LastRun.ToString("s") } }
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
