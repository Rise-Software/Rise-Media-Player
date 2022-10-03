using Microsoft.UI.Xaml.Controls;
using Rise.App.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        private SettingsViewModel ViewModel => App.SViewModel;

        private readonly uint[] MinuteIntervals = new uint[] { 1, 5, 10, 30, 60 };
        private readonly List<string> Intervals = new()
        {
            "1 minute",
            "5 minutes",
            "10 minutes",
            "30 minutes",
            "1 hour"
        };

        public ScanningPage()
        {
            InitializeComponent();
        }

        private void PeriodicScan_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = (sender as RadioButtons).SelectedIndex;
            if (index >= 0)
            {
                ViewModel.IndexingTimerInterval = MinuteIntervals[index];
                App.RestartIndexingTimer();
            }
        }

        private void PeriodicSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            App.RestartIndexingTimer();
        }

        private async void ManualScanButton_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(async () => await App.MViewModel.StartFullCrawlAsync());
        }
    }
}
