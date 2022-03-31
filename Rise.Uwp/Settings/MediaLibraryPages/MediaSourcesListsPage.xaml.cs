using System;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Settings
{
    public sealed partial class MediaSourcesListsPage : Page
    {
        private string _currTag = "AllMedia";

        private StorageLibrary MusicLibrary => App.MusicLibrary;
        private StorageLibrary VideoLibrary => App.VideoLibrary;

        public MediaSourcesListsPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is string param)
            {
                this._currTag = param;
                if (param == "Music")
                {
                    this.MusicList.Visibility = Visibility.Visible;
                    this.VideoList.Visibility = Visibility.Collapsed;
                }
                else if (param == "Videos")
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
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (this._currTag == "Music")
            {
                _ = await this.MusicLibrary.RequestAddFolderAsync();
            }
            else if (this._currTag == "Videos")
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
