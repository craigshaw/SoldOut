using SoldOutBusiness.Models;
using System.Collections.Generic;

namespace SoldOutBusiness.Repository
{
    public interface ISearchRepository
    {
        Search GetSearchByID(long searchID);
        IEnumerable<Search> GetAllSearches();
        IEnumerable<Search> GetAllSearchesWithResults();
        IEnumerable<SearchResult> GetResultsForSearch(long searchId);
        void AddSearchResult(long searchID, SearchResult result);
        bool SaveAll();
    }
}
