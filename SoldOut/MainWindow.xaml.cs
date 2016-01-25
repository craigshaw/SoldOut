﻿using MahApps.Metro.Controls;
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
        public MainWindow()
        {
            InitializeComponent();

            InitialiseSearchGrid();
        }

        private void InitialiseSearchGrid()
        {
            using (var repo = new SearchRepository())
            {
                // Bind data to the search grid
                Searches.ItemsSource = repo.GetAllSearches().OrderByDescending(s => s.LastCleansed);

                // Select the first item
                Searches.SelectedIndex = 0;

                // Now load the results for that search
                SearchResults.SelectionMode = DataGridSelectionMode.Extended;
                SearchResults.ItemsSource = repo.GetSearchResults(((Search)Searches.SelectedItem).SearchId).ToList();
            }
        }

        private void HandleListingClick(object sender, RoutedEventArgs e)
        {
            Hyperlink link = (Hyperlink)e.OriginalSource;
            Process.Start(link.NavigateUri.AbsoluteUri);
        }
    }
}
