using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using RMP.App.Dialogs;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.Storage.AccessCache;
using static RMP.App.Dialogs.FoldersDialog;
using System.Collections.Generic;

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
        private FoldersDialog Dialog = new FoldersDialog();
        public ContentDialog dialog = new ContentDialog
        {
            Title = "Folders",
            PrimaryButtonText = "Add folder",
            SecondaryButtonText = "Done",
            DefaultButton = ContentDialogButton.Primary,
        };

        private List<string> Deletion { get; set; }
        private Windows.ApplicationModel.Resources.ResourceLoader resourceLoader =
            Windows.ApplicationModel.Resources.
                ResourceLoader.GetForCurrentView("MediaLibrary");
        #endregion

        public MediaLibraryPage()
        {
            this.InitializeComponent();
            Current = this;

            dialog.Closing += Dialog_Closing;
            dialog.Closed += Dialog_Closed;
            dialog.Content = Dialog;

            Deletion = new List<string>
            {
                resourceLoader.GetString("OnlyApp"),
                resourceLoader.GetString("Device")
            };
        }

        private async void Dialog_Closed(ContentDialog sender, ContentDialogClosedEventArgs args)
        {
            if (args.Result == ContentDialogResult.Secondary)
            {
                await MainPage.Current.Dialog.ShowAsync();
            }
        }

        #region Checkboxes
        private void SelectAll_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void SelectAll_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void SelectAll_Indeterminate(object sender, RoutedEventArgs e)
        {

        }

        private void Option_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Option_Unchecked(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        private async void ChooseFolders_Click(object sender, RoutedEventArgs e)
        {
            MainPage.Current.Dialog.Hide();
            await dialog.ShowAsync();
        }

        private void Dialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            if (args.Result == ContentDialogResult.Primary)
            {
                args.Cancel = true;
                AddFolder();
                return;
            }
            else if (args.Result == ContentDialogResult.Secondary)
            {
                dialog.Hide();
            }
        }

        public async void AddFolder()
        {
            FolderPicker folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.MusicLibrary;
            folderPicker.FileTypeFilter.Add(".mp3"); // meaningless, but you have to have something
            StorageFolder folder = await folderPicker.PickSingleFolderAsync();

            if (folder != null)
            {
                foreach (AccessListEntry entry in FoldersDialog.Current.
                    FutureAccess.Entries)
                {
                    // Get folder from future access list
                    string faToken = entry.Token;
                    StorageFolder fold = await FoldersDialog.Current.
                        FutureAccess.GetFolderAsync(faToken);
                    if (folder.Path == fold.Path)
                    {
                        return;
                    }
                }

                string token = Guid.NewGuid().ToString();
                StorageApplicationPermissions.FutureAccessList.AddOrReplace(token, folder);

                FoldersDialog.Current.
                    Entries.Add(new ListEntry
                    {
                        Path = folder.Path,
                        DisplayName = folder.DisplayName,
                        Token = token
                    });
            }
        }
    }
}
