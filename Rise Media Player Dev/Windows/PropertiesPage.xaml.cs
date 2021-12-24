using Rise.App.Common;
using Rise.App.Props;
using Rise.App.ViewModels;
using System;
using System.Collections.Generic;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Views
{
    public sealed partial class PropertiesPage : Page
    {
        private SongPropertiesViewModel Props { get; set; }
        private IEnumerable<ToggleButton> Toggles { get; set; }

        public PropertiesPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;

            Toggles = ItemGrid.GetChildren<ToggleButton>();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is SongPropertiesViewModel props)
            {
                Props = props;
            }

            Details.IsChecked = true;
            base.OnNavigatedTo(e);
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            Details.IsChecked = false;
            Lyrics.IsChecked = false;
            Profile.IsChecked = false;
            File.IsChecked = false;

            ToggleButton clicked = (ToggleButton)sender;
            clicked.Checked -= ToggleButton_Checked;
            clicked.IsChecked = true;

            switch (clicked.Tag.ToString())
            {
                case "DetailsItem":
                    _ = PropsFrame.Navigate(typeof(DetailsPage), Props);
                    break;

                case "FileItem":
                    _ = PropsFrame.Navigate(typeof(FilePage), Props);
                    break;

                default:
                    break;
            }

            clicked.Checked += ToggleButton_Checked;
        }

        private async void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _ = await ApplicationView.GetForCurrentView().TryConsolidateAsync();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            bool result = await Props.SaveChangesAsync();

            if (result)
            {
                _ = await ApplicationView.GetForCurrentView().TryConsolidateAsync();
                return;
            }

            ErrorTip.IsOpen = true;
        }
    }
}
