using SoldOut.Framework;
using SoldOut.Models;
using SoldOutBusiness.Models;
using SoldOutBusiness.Repository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SoldOut.ViewModels
{
    internal class CleanserViewModel : ViewModel
    {
        // Commands
        private DelegateCommand<IList> _deleteSearchResultsCommand;
        private DelegateCommand _windowClosingCommand;

        // Fields
        private ISearchRepository _repo;
        private SearchOverview _selectedSearchOverview;
        private bool _initialising;

        public CleanserViewModel()
        {
            _initialising = true;

            _deleteSearchResultsCommand = new DelegateCommand<IList>(
                DeleteSelectedSearchResults
                );

            _windowClosingCommand = new DelegateCommand(
                () =>
                {
                    // Clean up the repo
                    if (_repo != null)
                    {
                        _repo.Dispose();
                        _repo = null;
                    }
                }
                );

            _repo = new SearchRepository();
        }

        #region Commands
        public DelegateCommand WindowClosingCommand
        {
            get
            {
                return _windowClosingCommand;
            }
        }

        public DelegateCommand<IList> DeleteSearchResultsCommand
        {
            get
            {
                return _deleteSearchResultsCommand;
            }
        }
        #endregion

        #region Properties
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
        #endregion

        #region Utility Methods
        private void DeleteSelectedSearchResults(IList selectedItems)
        {
            // Get the selected items
            var items = selectedItems.Cast<SearchResult>();

            // Now remove them via the repository
            _repo.DeleteSearchResults(items);

            // Update the last cleansed time
            _repo.UpdateSearchLastCleansedTime(_selectedSearchOverview.SearchId, DateTime.Now);

            // Commit
            _repo.SaveAll();

            // Reload the content
            RaisePropertyChangedEvent("SearchResults");
        }
        #endregion
    }
}
