﻿using SoldOutBusiness.DAL;
using SoldOutBusiness.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Data.SqlClient;

namespace SoldOutBusiness.Repository
{
    public class UncleansedCount
    {
        public long SearchId { get; set; }
        public int Count { get; set; }
    }

    public class SearchRepository : ISearchRepository
    {
        private SearchContext _context;

        public SearchRepository()
        {
            _context = new SearchContext();

#if DEBUG
            _context.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
#endif
        }

        public void AddSearchResult(long searchID, SearchResult result)
        {
            var search = GetSearchByID(searchID);

            search.SearchResults.Add(result);
            _context.SearchResults.Add(result);
        }

        public void AddSearchResults(long searchID, IEnumerable<SearchResult> results)
        {
            var search = GetSearchByID(searchID);

            foreach (var result in results)
            {
                search.SearchResults.Add(result);
                _context.SearchResults.Add(result);
            }
        }

        public IEnumerable<Search> GetAllSearches()
        {
            return _context.Searches.Select(s => s)
                .OrderBy(s => s.SearchId).ToList();
        }

        public IEnumerable<Search> GetAllSearchesWithResults()
        {
            return _context.Searches.Include(s => s.SearchResults)
                .OrderBy(s => s.SearchId).ToList();
        }

        public IEnumerable<Search> GetAllSearchesWithSearchCriteria()
        {
            return _context.Searches.Include(s => s.SearchCriteria)
                .OrderBy(s => s.SearchId).ToList();
        }

        public IEnumerable<SearchResult> GetSearchResults(long searchId)
        {
            return _context.SearchResults.Where(r => r.SearchID == searchId);
        }

        public IEnumerable<SearchCriteria> GetSearchCriteria()
        {
            return _context.SearchCriteria.Select(sc => sc).ToList();
        }

        public Search GetSearchByID(long searchID)
        {
            return _context.Searches.Where(s => s.SearchId == searchID).FirstOrDefault();
        }

        public IDictionary<long, int> GetUncleansedCounts()
        {
            var uncleansedCounts = new Dictionary<long, int>();

            var counts = _context.Database.SqlQuery<UncleansedCount>("select s.SearchId, Count(SearchResultID) as 'Count' from SearchResult sr" +
                                                                    " Join Search s on s.SearchId = sr.SearchID" +
                                                                    " where sr.dateofmatch > s.LastCleansed" +
                                                                    " group by s.SearchId");

            foreach (var counter in counts)
            {
                uncleansedCounts[counter.SearchId] = counter.Count;
            }

            return uncleansedCounts;
        }

        public Search GetNextSearch(long searchID)
        {
            long maxID = _context.Searches.Max(s => s.SearchId);

            var currentID = (maxID == searchID) ? 0 : searchID;

            return _context.Searches.Where(s => s.SearchId > currentID).Include(s => s.SearchCriteria).FirstOrDefault();
        }

        public bool ResetSuspiciousSearchResults(IEnumerable<SearchResult> results)
        {
            // For every remaining suspicious entry, reset the suspicious flag and commit back to the DB
            foreach (var result in results.Where(sr => sr.Suspicious == true))
            {
                result.Suspicious = false;
            }

            return SaveAll();
        }

        public void DeleteSearchResults(IEnumerable<SearchResult> results)
        {
            // NOTE - assumes the entities are attached, i.e. _context.Entry(results.FirstOrDefault()).State != EntityState.Detached;
            _context.SearchResults.RemoveRange(results);
        }

        public void UpdateSearchLastCleansedTime(long searchID, DateTime lastCleansed)
        {
            var search = GetSearchByID(searchID);
            search.LastCleansed = lastCleansed;
        }

        public bool SaveAll()
        {
            return _context.SaveChanges() > 0;
        }

        public int ResultCount(long searchID)
        {
            return _context.SearchResults.Where(s => s.SearchID == searchID).Count();
        }

        public PriceStats GetPriceStatsForSearch(long searchId)
        {
            return _context.Database.SqlQuery<PriceStats>("dbo.GetPriceStatsForSearch @SearchId", new SqlParameter("SearchId", searchId)).Single();
        }

        public IEnumerable<SuspiciousPhrase> GetBasicSuspiciousPhrases()
        {
            return _context.SuspiciousPhrases.Select(p => p);
        }

        public IEnumerable<SearchSuspiciousPhrase> GetSuspiciousPhrasesForSearch(long searchId)
        {
            return _context.SearchSuspiciousPhrases.Where(sp => sp.SearchId == searchId).Select(p => p);
        }

        public IList<Condition> GetConditions()
        {
            return _context.Conditions.Select(c => c).ToList();
        }

        #region IDisposable Support
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_context != null)
                {
                    _context.Dispose();
                    _context = null;
                }
            }
        }
        #endregion
    }
}
