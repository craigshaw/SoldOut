using SoldOutBusiness.DAL;
using SoldOutBusiness.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace SoldOutBusiness.Repository
{
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

        public Search GetSearchByID(long searchID)
        {
            return _context.Searches.Where(s => s.SearchId == searchID).FirstOrDefault();
        }

        public Search GetNextSearch(long searchID)
        {
            long maxID = _context.Searches.Max(s => s.SearchId);

            var currentID = (maxID == searchID) ? 0 : searchID;

            return _context.Searches.Where(s => s.SearchId > currentID).Include(s => s.SearchCriteria).FirstOrDefault();
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
