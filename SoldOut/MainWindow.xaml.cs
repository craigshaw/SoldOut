using log4net;
using MahApps.Metro.Controls;
using SoldOutBusiness.Models;
using SoldOutBusiness.Repository;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace SoldOut
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Application));
        private ISearchRepository _repo;

        public MainWindow()
        {
            InitializeComponent();

            _repo = new SearchRepository();

            InitialiseSearchGrid();
        }

        private void InitialiseSearchGrid()
        {
            // Bind data to the search grid
            Searches.ItemsSource = _repo.GetAllSearches().OrderByDescending(s => s.LastRun).ThenBy(s => s.LastCleansed);

            // Select the first item
            Searches.SelectedIndex = 0;
        }

        private void HandleListingClick(object sender, RoutedEventArgs e)
        {
            Hyperlink link = (Hyperlink)e.OriginalSource;
            Process.Start(link.NavigateUri.AbsoluteUri);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected items
            var items = SearchResults.SelectedItems.Cast<SearchResult>();

            // Now remove them via the repository
            _repo.DeleteSearchResults(items);

            // Update the last cleansed time
            _repo.UpdateSearchLastCleansedTime(((Search)Searches.SelectedItem).SearchId, DateTime.Now);

            // Commit
            _repo.SaveAll();

            // Reload the content
            SearchResults.ItemsSource = _repo.GetSearchResults(((Search)Searches.SelectedItem).SearchId).ToList();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Clean up the repo
            if(_repo != null)
            {
                _repo.Dispose();
                _repo = null;
            }
        }

        private void Searches_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Now load the results for that search
            SearchResults.ItemsSource = _repo.GetSearchResults(((Search)Searches.SelectedItem).SearchId).ToList();
        }
    }
}
