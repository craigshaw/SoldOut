using SoldOutCleanser.Framework;
using SoldOutCleanser.Models;
using SoldOutBusiness.Models;
using SoldOutBusiness.Repository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SoldOutCleanser.ViewModels
{
    internal class CleanserViewModel : ViewModel
    {
        // Commands
        private DelegateCommand<IList> _deleteSearchResultsCommand;
        private DelegateCommand _windowClosingCommand;
        private DelegateCommand<IList> _markAsCleansedCommand;

        // Fields
        private ISoldOutRepository _repo;
        private SearchOverview _selectedSearchOverview;
        private IEnumerable<SearchOverview> _searches;

        public CleanserViewModel()
        {
            ShowOnlyNewResults = true;

            _repo = new SoldOutRepository();

            _deleteSearchResultsCommand = new DelegateCommand<IList>(
                DeleteSelectedSearchResults
                );

            _markAsCleansedCommand = new DelegateCommand<IList>(
                MarkSelectedSearchAsCleansed
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

            Searches = GetSearches();

            SelectedSearchOverview = _searches.First();
        }

        #region Commands
        public DelegateCommand WindowClosingCommand
        {
            get
            {
                return _windowClosingCommand;
            }
        }
        public DelegateCommand<IList> MarkAsCleansedCommand
        {
            get
            {
                return _markAsCleansedCommand;
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
        public bool ShowOnlyNewResults { get; set; }

        public string AppTitle
        {
            get
            {
                return "SoldOut Cleanser v" + Assembly.GetEntryAssembly().GetName().Version;
            }
        }

        public IEnumerable<SearchOverview> Searches
        {
            get
            {
                return _searches;
            }

            set
            {
                _searches = value;
                RaisePropertyChangedEvent("Searches");
            }
        }

        public IEnumerable<SearchResult> SearchResults
        {
            get
            {
                var results = ShowOnlyNewResults ? _repo.GetSearchResultsSince(_selectedSearchOverview.SearchId, _selectedSearchOverview.LastCleansed)
                                                 : _repo.GetSearchResults(_selectedSearchOverview.SearchId);

                return results.OrderByDescending(sr => sr.EndTime.Value.Date).ThenBy(sr => sr.EndTime.Value.TimeOfDay).ToList();
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

                if(_selectedSearchOverview != null)
                {
                    RaisePropertyChangedEvent("SelectedSearchOverview"); // Select the item
                    RaisePropertyChangedEvent("SearchResults"); // Refresh the results view
                }
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
            UpdateCleansedTimeOnSelectedSearch();

            // Commit
            _repo.SaveAll();

            // Reload the content
            RaisePropertyChangedEvent("SearchResults");
        }

        private void MarkSelectedSearchAsCleansed(IList results)
        {
            var searchResults = results.Cast<SearchResult>();

            // Remember which search was selected
            var selectedSearchId = _selectedSearchOverview.SearchId;

            // Mark any suspicious results as not
            _repo.ResetSuspiciousSearchResults(searchResults);

            // Update the last cleansed time
            UpdateCleansedTimeOnSelectedSearch();

            // Reload searches
            Searches = GetSearches();

            // Select the saved search
            SelectedSearchOverview = _searches.Where(so => so.SearchId == selectedSearchId).Single();
        }

        private void UpdateCleansedTimeOnSelectedSearch()
        {
            // Update the last cleansed time (remote)
            _repo.UpdateSearchLastCleansedTime(_selectedSearchOverview.SearchId, DateTime.Now);

            // Commit
            _repo.SaveAll();
        }

        private IEnumerable<SearchOverview> GetSearches()
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

            return searchOverviews.OrderByDescending(s => s.UncleansedCount);
        }
        #endregion
    }
}
