using MahApps.Metro.Controls;
using SoldOutBusiness.Models;
using SoldOutBusiness.Repository;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SoldOut
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
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
            Searches.ItemsSource = _repo.GetAllSearches().OrderByDescending(s => s.LastCleansed);

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
            var items = SearchResults.SelectedItems;

            // TODO: Now remove them via the repository
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
