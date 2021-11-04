using RMP.App.Dialogs;
using RMP.App.Settings.ViewModels;
using RMP.App.Views;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace RMP.App.Settings
{
    public sealed partial class MediaLibraryPage : Page
    {
        #region Variables
        private SettingsViewModel ViewModel => App.SViewModel;

        public static MediaLibraryPage Current;
        private readonly FoldersDialog Dialog = new FoldersDialog();
        public ContentDialog dialog = new ContentDialog
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

            dialog.Closing += Dialog_Closing;
            dialog.Closed += Dialog_Closed;
            dialog.Content = Dialog;

            DataContext = ViewModel;
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        private async void Dialog_Closed(ContentDialog sender, ContentDialogClosedEventArgs args)
        {
            _ = await MainPage.Current.SDialog.ShowAsync();
        }

        private async void ChooseFolders_Click(object sender, RoutedEventArgs e)
        {
            MainPage.Current.SDialog.Hide();
            _ = await dialog.ShowAsync();
        }

        private void Dialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            if (args.Result == ContentDialogResult.Primary)
            {
                args.Cancel = true;
                AddFolder();
            }
        }

        public async void AddFolder()
        {
            _ = await FoldersDialog.Current.MusicLibrary.RequestAddFolderAsync();
        }
    }
}
