using Rise.App.Common;
using Rise.App.Dialogs;
using Rise.App.ViewModels;
using Rise.App.Views;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Settings
{
    public sealed partial class MediaLibraryPage : Page
    {
        #region Variables
        private SettingsViewModel ViewModel => App.SViewModel;

        public static MediaLibraryPage Current;
        private readonly FoldersDialog Dialog = new FoldersDialog();
        private readonly VFoldersDialog VDialog = new VFoldersDialog();

        public ContentDialog FolderDialog = new ContentDialog
        {
            Title = ResourceLoaders.MediaLibraryLoader.GetString("Folders"),
            PrimaryButtonText = ResourceLoaders.MediaLibraryLoader.GetString("Add"),
            SecondaryButtonText = ResourceLoaders.MediaLibraryLoader.GetString("Done"),
            DefaultButton = ContentDialogButton.Primary,
        };

        public ContentDialog VFolderDialog = new ContentDialog
        {
            Title = ResourceLoaders.MediaLibraryLoader.GetString("Folders"),
            PrimaryButtonText = ResourceLoaders.MediaLibraryLoader.GetString("Add"),
            SecondaryButtonText = ResourceLoaders.MediaLibraryLoader.GetString("Done"),
            DefaultButton = ContentDialogButton.Primary,
        };

        private readonly List<string> Deletion = new List<string>
        {
            ResourceLoaders.MediaLibraryLoader.GetString("OnlyApp"),
            ResourceLoaders.MediaLibraryLoader.GetString("Device")
        };
        #endregion

        public MediaLibraryPage()
        {
            InitializeComponent();
            Current = this;

            FolderDialog.Closing += Dialog_Closing;
            FolderDialog.Closed += Dialog_Closed;
            FolderDialog.Content = Dialog;

            VFolderDialog.Closing += VDialog_Closing;
            VFolderDialog.Closed += Dialog_Closed;
            VFolderDialog.Content = VDialog;

            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        private async void Dialog_Closed(ContentDialog sender, ContentDialogClosedEventArgs args)
            => _ = await MainPage.Current.SDialog.ShowAsync();

        private async void ChooseFolders_Click(object sender, RoutedEventArgs e)
        {
            MainPage.Current.SDialog.Hide();
            _ = await FolderDialog.ShowAsync();
        }

        private async void VChooseFolders_Click(object sender, RoutedEventArgs e)
        {
            MainPage.Current.SDialog.Hide();
            _ = await VFolderDialog.ShowAsync();
        }

        private async void Dialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            if (args.Result == ContentDialogResult.Primary)
            {
                args.Cancel = true;
                _ = await AddFolderAsync();
            }
        }

        private async void VDialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            if (args.Result == ContentDialogResult.Primary)
            {
                args.Cancel = true;
                _ = await AddVideoFolderAsync();
            }
        }

        private async Task<StorageFolder> AddFolderAsync()
            => await App.MusicLibrary.RequestAddFolderAsync();

        private async Task<StorageFolder> AddVideoFolderAsync()
            => await App.VideoLibrary.RequestAddFolderAsync();
    }
}
