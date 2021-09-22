using RMP.App.Dialogs;
using RMP.App.Views;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace RMP.App.Settings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MediaLibraryPage : Page
    {
        #region Variables
        public static MediaLibraryPage Current;
        private readonly FoldersDialog Dialog = new FoldersDialog();
        public ContentDialog dialog = new ContentDialog
        {
            Title = ResourceLoaders.MediaLibraryLoader.GetString("Folders"),
            PrimaryButtonText = ResourceLoaders.MediaLibraryLoader.GetString("Add"),
            SecondaryButtonText = ResourceLoaders.MediaLibraryLoader.GetString("Done"),
            DefaultButton = ContentDialogButton.Primary,
        };

        private List<string> Deletion { get; set; }
        #endregion

        public MediaLibraryPage()
        {
            InitializeComponent();
            Current = this;

            dialog.Closing += Dialog_Closing;
            dialog.Closed += Dialog_Closed;
            dialog.Content = Dialog;

            Deletion = new List<string>
            {
                ResourceLoaders.MediaLibraryLoader.GetString("OnlyApp"),
                ResourceLoaders.MediaLibraryLoader.GetString("Device")
            };
        }

        private async void Dialog_Closed(ContentDialog sender, ContentDialogClosedEventArgs args)
        {
            _ = await MainPage.Current.Dialog.ShowAsync();
        }

        private async void ChooseFolders_Click(object sender, RoutedEventArgs e)
        {
            MainPage.Current.Dialog.Hide();
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
