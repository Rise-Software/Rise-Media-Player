using Microsoft.UI.Xaml.Controls;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Rise.App.Settings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ScanningPage : Page
    {
        public ScanningPage()
        {
            InitializeComponent();
        }

        private void PeriodicScan_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch ((sender as RadioButtons).SelectedIndex)
            {
                case 0:
                    // Index every 1 minute
                    App.SViewModel.IndexingMode = 0;
                    App.IndexingInterval = TimeSpan.FromMinutes(1);
                    break;
                case 1:
                    // Index every 5 minutes
                    App.SViewModel.IndexingMode = 1;
                    App.IndexingInterval = TimeSpan.FromMinutes(5);
                    break;
                case 2:
                    // Index every 10 minutes
                    App.SViewModel.IndexingMode = 2;
                    App.IndexingInterval = TimeSpan.FromMinutes(10);
                    break;
                case 3:
                    // Index every 30 minutes
                    App.SViewModel.IndexingMode = 3;
                    App.IndexingInterval = TimeSpan.FromMinutes(30);
                    break;
                case 4:
                    // Index every hour
                    App.SViewModel.IndexingMode = 4;
                    App.IndexingInterval = TimeSpan.FromHours(1);
                    break;
                default:
                    break;
            }
            App.StartIndexingTimer();
        }

        private void PeriodicSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            //if (PeriodicContent.Visibility == Visibility.Visible)
            //{
            //    PeriodicBorder.Height = 120;
            //}
            //else
            //{
            //    PeriodicBorder.Height = 64;
            //}
        }

        private async void ManualScanButton_Click(object sender, RoutedEventArgs e)
        {
            await App.MViewModel.StartFullCrawlAsync();
        }
    }
}
