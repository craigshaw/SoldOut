using SoldOutBusiness.Models;
using System.Collections.Generic;

namespace SoldOutBusiness.Repository
{
    public interface ISearchRepository
    {
        Search GetSearchByID(long searchID);
        Search GetNextSearch(long searchID);
        IEnumerable<Search> GetAllSearches();
        IEnumerable<Search> GetAllSearchesWithResults();
        IEnumerable<Search> GetAllSearchesWithSearchCriteria();
        IEnumerable<SearchResult> GetSearchResults(long searchId);
        void AddSearchResult(long searchID, SearchResult result);
        void AddSearchResults(long searchID, IEnumerable<SearchResult> results);
        bool SaveAll();
    }
}
