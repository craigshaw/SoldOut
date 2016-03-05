using MahApps.Metro.Controls;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;

namespace SoldOutCleanser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // TODO: Move this to the ViewModel
        private void HandleListingClick(object sender, RoutedEventArgs e)
        {
            Hyperlink link = (Hyperlink)e.OriginalSource;
            Process.Start(link.NavigateUri.AbsoluteUri);
        }

        private void ShowSettings(object sender, RoutedEventArgs e)
        {
            flyout.IsOpen = !flyout.IsOpen;
        }
    }
}
