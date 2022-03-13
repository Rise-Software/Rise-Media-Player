using Rise.Common.Helpers;
using System;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Settings
{
    public sealed partial class MediaSourcesPage : Page
    {
        private readonly NavigationHelper _navigationHelper;

        private StorageLibrary MusicLibrary => App.MusicLibrary;
        private StorageLibrary VideoLibrary => App.VideoLibrary;

        private string _currTag = "AllMedia";

        public MediaSourcesPage()
        {
            this.InitializeComponent();
            this._navigationHelper = new NavigationHelper(this);
        }

        private void Selection_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            var selectedItem = (Microsoft.UI.Xaml.Controls.NavigationViewItem)args.SelectedItem;
            _currTag = (string)selectedItem.Tag;

            if (_currTag == "Music")
            {
                this.MusicList.Visibility = Visibility.Visible;
                this.VideoList.Visibility = Visibility.Collapsed;
            }
            else if (_currTag == "Videos")
            {
                this.MusicList.Visibility = Visibility.Collapsed;
                this.VideoList.Visibility = Visibility.Visible;
            }
            else
            {
                this.MusicList.Visibility = Visibility.Visible;
                this.VideoList.Visibility = Visibility.Visible;
            }
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currTag == "Music")
            {
                _ = await this.MusicLibrary.RequestAddFolderAsync();
            }
            else if (_currTag == "Videos")
            {
                _ = await this.VideoLibrary.RequestAddFolderAsync();
            }
            else
            {
                this.AddFlyout.ShowAt(AddButton);
            }
        }

        private async void AddMusicFolder_Click(object sender, RoutedEventArgs e)
            => _ = await this.MusicLibrary.RequestAddFolderAsync();

        private async void AddVideoFolder_Click(object sender, RoutedEventArgs e)
            => _ = await this.VideoLibrary.RequestAddFolderAsync();

        private async void RemoveMusicFolder_Click(object sender, RoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is StorageFolder folder)
            {
                _ = await this.MusicLibrary.RequestRemoveFolderAsync(folder);
            }
        }

        private async void RemoveVideoFolder_Click(object sender, RoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is StorageFolder folder)
            {
                _ = await this.VideoLibrary.RequestRemoveFolderAsync(folder);
            }
        }

        private async void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is StorageFolder folder)
            {
                _ = await Launcher.LaunchFolderAsync(folder);
            }
        }
    }
}
