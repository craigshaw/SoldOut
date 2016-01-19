using System.Collections.Generic;

namespace EFSQLTest.Models
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
