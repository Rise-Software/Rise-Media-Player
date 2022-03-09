using Rise.App.ViewModels;
using Rise.App.Views.Playlists.Properties;
using Rise.Common.Extensions;
using System;
using System.Collections.Generic;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Rise.App.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PlaylistPropertiesPage : Page
    {
        private PlaylistViewModel Playlist;
        private bool _saveChanges = false;

        private Uri _imagePath = new("ms-appx:///Assets/NavigationView/PlaylistsPage/blankplaylist.png");
        private IEnumerable<ToggleButton> Toggles { get; set; }

        public PlaylistPropertiesPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;

            Toggles = ItemGrid.GetChildren<ToggleButton>();
            ApplicationView.GetForCurrentView().Consolidated += PlaylistPropertiesPage_Consolidated;
            //Loaded += (s, e) =>
            //{
            //    _ = new ApplicationTitleBar(TitleBar);
            //};
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Playlist = e.Parameter as PlaylistViewModel;

            Details.IsChecked = true;
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            Details.IsChecked = false;
            Songs.IsChecked = false;

            ToggleButton clicked = (ToggleButton)sender;
            clicked.Checked -= ToggleButton_Checked;
            clicked.IsChecked = true;

            switch (clicked.Tag.ToString())
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

            clicked.Checked += ToggleButton_Checked;
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
    }
}
