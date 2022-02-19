using Microsoft.UI.Xaml.Controls;
using Rise.App.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static Rise.App.Common.Enums;

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

            Loaded += (s, e) =>
            {
                _ = new ApplicationTitleBar(AppTitleBar);
                PeriodicScan.SelectedIndex = App.SViewModel.IndexingMode + 1;
            };
        }

        private void PeriodicScan_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch ((sender as RadioButtons).SelectedIndex)
            {
                case 0:
                    // Never index
                    App.SViewModel.IndexingMode = -1;
                    break;
                case 1:
                    // Index every 1 minute
                    App.SViewModel.IndexingMode = 0;
                    App.IndexingInterval = TimeSpan.FromMinutes(1);
                    break;
                case 2:
                    // Index every 5 minutes
                    App.SViewModel.IndexingMode = 1;
                    App.IndexingInterval = TimeSpan.FromMinutes(5);
                    break;
                case 3:
                    // Index every 10 minutes
                    App.SViewModel.IndexingMode = 2;
                    App.IndexingInterval = TimeSpan.FromMinutes(10);
                    break;
                case 4:
                    // Index every 30 minutes
                    App.SViewModel.IndexingMode = 3;
                    App.IndexingInterval = TimeSpan.FromMinutes(30);
                    break;
                case 5:
                    // Index every hour
                    App.SViewModel.IndexingMode = 4;
                    App.IndexingInterval = TimeSpan.FromHours(1);
                    break;
                default:
                    break;
            }
            App.StartIndexingTimer();
        }
    }
}
