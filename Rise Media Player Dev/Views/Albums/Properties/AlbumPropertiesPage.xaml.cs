using Rise.App.ViewModels;
using System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Views.Albums.Properties
{
    public sealed partial class AlbumPropertiesPage : Page
    {
        private AlbumViewModel Album;

        public AlbumPropertiesPage()
        {
            this.InitializeComponent();
            TitleBar.SetTitleBarForCurrentView();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Album = e.Parameter as AlbumViewModel;
        }

        private async void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _ = await ApplicationView.GetForCurrentView().TryConsolidateAsync();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
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
