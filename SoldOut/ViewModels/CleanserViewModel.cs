using SoldOut.Framework;
using SoldOut.Models;
using SoldOutBusiness.Models;
using SoldOutBusiness.Repository;
using System.Collections.Generic;
using System.Linq;

namespace SoldOut.ViewModels
{
    internal class CleanserViewModel : ViewModel
    {
        private ISearchRepository _repo;
        private SearchOverview _selectedSearchOverview;
        private bool _initialising;

        public CleanserViewModel()
        {
            _initialising = true;

            _repo = new SearchRepository();
        }

        public IEnumerable<SearchOverview> Searches
        {
            get
            {
                var searchOverviews = new List<SearchOverview>();
                var searches = _repo.GetAllSearches();
                var counts = _repo.GetUncleansedCounts();

                foreach (var search in searches)
                {
                    searchOverviews.Add(new SearchOverview()
                    {
                        SearchId = search.SearchId,
                        Description = search.Description,
                        LastCleansed = search.LastCleansed,
                        LastRun = search.LastRun,
                        UncleansedCount = counts.ContainsKey(search.SearchId) ? counts[search.SearchId] : 0
                    });
                }

                var orderedSearches = searchOverviews.OrderByDescending(s => s.UncleansedCount);

                // TODO: Do something about this hack
                if (_initialising)
                {
                    SelectedSearchOverview = orderedSearches.First();
                    _initialising = false;
                }

                return orderedSearches;
            }
        }

        public IEnumerable<SearchResult> SearchResults
        {
            get
            {
                return _repo.GetSearchResults(_selectedSearchOverview.SearchId).ToList();
            }
        }

        public SearchOverview SelectedSearchOverview
        {
            get
            {
                return _selectedSearchOverview;
            }

            set
            {
                _selectedSearchOverview = value;
                RaisePropertyChangedEvent("SelectedSearchOverview"); // Select the item
                RaisePropertyChangedEvent("SearchResults"); // Refresh the results view
            }
        }
    }
}
