using Rise.App.ViewModels;
using System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Views
{
    public sealed partial class PlaylistPropertiesPage : Page
    {
        private PlaylistViewModel Playlist;
        private bool _saveChanges = false;

        public PlaylistPropertiesPage()
        {
            InitializeComponent();
            TitleBar.SetTitleBarForCurrentView();

            ApplicationView.GetForCurrentView().Consolidated += PlaylistPropertiesPage_Consolidated;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Playlist = e.Parameter as PlaylistViewModel;
        }

        private async void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _saveChanges = false;
            _ = await ApplicationView.GetForCurrentView().TryConsolidateAsync();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            _saveChanges = true;
            _ = await ApplicationView.GetForCurrentView().TryConsolidateAsync();
        }

        private async void PlaylistPropertiesPage_Consolidated(ApplicationView sender, ApplicationViewConsolidatedEventArgs args)
        {
            if (_saveChanges)
            {
                await Playlist.SaveEditsAsync();
            }
            else
            {
                try
                {
                    await Playlist.CancelEditsAsync();
                }
                catch
                {

                }
            }
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
                        _ = PropsFrame.Navigate(typeof(PlaylistDetailsPropertiesPage), Playlist);
                        break;

                    case "SongsItem":
                        _ = PropsFrame.Navigate(typeof(PlaylistSongsPropertiesPage), Playlist);
                        break;

                    default:
                        break;
                }

            }
        }
    }
}
