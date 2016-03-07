﻿using SoldOutBusiness.Models;
using System;
using System.Collections.Generic;

namespace SoldOutBusiness.Repository
{
    public interface ISearchRepository : IDisposable
    {
        Search GetSearchByID(long searchID);
        Search GetNextSearch(long searchID);
        IEnumerable<Search> GetAllSearches();
        IEnumerable<Search> GetAllSearchesWithResults();
        IEnumerable<Search> GetAllSearchesWithSearchCriteria();
        IEnumerable<SearchResult> GetSearchResults(long searchId);
        IEnumerable<SearchResult> GetSearchResultsSince(long searchId, DateTime since);
        void AddSearchResult(long searchID, SearchResult result);
        void AddSearchResults(long searchID, IEnumerable<SearchResult> results);
        void DeleteSearchResults(IEnumerable<SearchResult> results);
        void UpdateSearchLastCleansedTime(long searchID, DateTime lastCleansed);
        bool SaveAll();
        int ResultCount(long searchID);
        IDictionary<long, int> GetUncleansedCounts();
        PriceStats GetPriceStatsForSearch(long searchId, int conditionId);
        IEnumerable<SuspiciousPhrase> GetBasicSuspiciousPhrases();
        IEnumerable<SearchSuspiciousPhrase> GetSuspiciousPhrasesForSearch(long searchId);
        bool ResetSuspiciousSearchResults(IEnumerable<SearchResult> results);
        IList<Condition> GetConditions();
    }
}
