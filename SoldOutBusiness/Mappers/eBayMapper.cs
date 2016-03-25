﻿using eBay.Services.Finding;
using SoldOutBusiness.Utilities.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoldOutBusiness.Mappers
{
    public static class eBayMapper
    {
        public static IEnumerable<Models.SearchResult> MapSearchItemsToSearchResults(SearchItem[] items, IConditionResolver resolver, int productId)
        {
            return items.Select(i => new Models.SearchResult()
            {
                DateOfMatch = DateTime.Now,
                Link = i.viewItemURL,
                Title = i.title,
                Price = i.sellingStatus.currentPrice.Value,
                ItemNumber = i.itemId,
                StartTime = i.listingInfo.startTime,
                EndTime = i.listingInfo.endTime,
                NumberOfBidders = i.listingInfo.listingType.ToLowerInvariant() == "auction" ? i.sellingStatus.bidCount : 0,
                ImageURL = i.galleryURL,
                Currency = i.sellingStatus.currentPrice.currencyId,
                Location = i.location,
                SiteID = i.globalId,
                Type = i.listingInfo.listingType,
                ShippingCost = (i.shippingInfo.shippingServiceCost != null) ? i.shippingInfo.shippingServiceCost.Value : 0.0f,
                ConditionId = resolver.ConditionIdFromEBayConditionId(i.condition.conditionId),
                ProductId = productId
            });
        }
    }
}
