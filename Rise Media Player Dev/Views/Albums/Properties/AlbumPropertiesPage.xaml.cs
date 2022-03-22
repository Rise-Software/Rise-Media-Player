using Rise.App.ViewModels;
using Rise.Common.Extensions;
using System;
using System.Collections.Generic;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Rise.App.Views.Albums.Properties
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AlbumPropertiesPage : Page
    {
        private AlbumViewModel Album;
        private bool _saveChanges = false;
        private IEnumerable<ToggleButton> Toggles { get; set; }
        public AlbumPropertiesPage()
        {
            this.InitializeComponent();

            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Album = e.Parameter as AlbumViewModel;

            base.OnNavigatedTo(e);
        }

        private async void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _saveChanges = false;
            _ = await ApplicationView.GetForCurrentView().TryConsolidateAsync();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _saveChanges = true;
            _ = await ApplicationView.GetForCurrentView().TryConsolidateAsync();
        }

        private async void PlaylistPropertiesPage_Consolidated(ApplicationView sender, ApplicationViewConsolidatedEventArgs args)
        {
            _ = await ApplicationView.GetForCurrentView().TryConsolidateAsync();
        }

        private void NavigationView_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            var selectedItem = (Microsoft.UI.Xaml.Controls.NavigationViewItem)args.SelectedItem;
            if (selectedItem != null)
            {
                string selectedItemTag = selectedItem.Tag as string;
                switch (selectedItemTag)
                {
                    case "DetailsItem":
                        _ = PropsFrame.Navigate(typeof(AlbumPropsDetailsPagexaml), Album);
                        break;

                    default:
                        break;
                }

            }
        }
    }
}
