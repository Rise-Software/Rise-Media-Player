using System;
using System.Collections.ObjectModel;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Fluent_Media_Player_Dev.Settings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MediaLibraryPage : Page
    {
        #region Variables
        public ObservableCollection<ListEntry> _Entries = new ObservableCollection<ListEntry>();
        public ObservableCollection<ListEntry> Entries { get { return _Entries; } }
        public StorageItemAccessList FutureAccess { get; set; }
        #endregion
        #region Classes
        public class ListEntry
        {
            public string Path { get; set; }
            public string DisplayName {  get; set; }
            public string Token { get; set; }
        }
        #endregion
        public MediaLibraryPage()
        {
            this.InitializeComponent();
            FutureAccess = StorageApplicationPermissions.FutureAccessList;
            FillList();
        }

        private async void FillList()
        {
            foreach (AccessListEntry entry in FutureAccess.Entries)
            {
                // Get folder from future access list
                string faToken = entry.Token;
                StorageFolder folder = await FutureAccess.GetFolderAsync(faToken);

                _Entries.Add(new ListEntry
                {
                    Path = folder.Path,
                    DisplayName = folder.DisplayName,
                    Token = faToken
                });
            }
        }

        private async void PickFolder_Click(object sender, RoutedEventArgs e)
        {
            FolderPicker folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.MusicLibrary;
            folderPicker.FileTypeFilter.Add(".mp3"); // meaningless, but you have to have something
            StorageFolder folder = await folderPicker.PickSingleFolderAsync();

            if (folder != null)
            {
                foreach (AccessListEntry entry in FutureAccess.Entries)
                {
                    // Get folder from future access list
                    string faToken = entry.Token;
                    StorageFolder fold = await FutureAccess.GetFolderAsync(faToken);
                    if (folder.Path == fold.Path)
                    {
                        return;
                    }
                }

                string token = Guid.NewGuid().ToString();
                StorageApplicationPermissions.FutureAccessList.AddOrReplace(token, folder);

                _Entries.Add(new ListEntry
                {
                    Path = folder.Path,
                    DisplayName = folder.DisplayName,
                    Token = token
                });
            }
        }

        private async void FolderList_ItemClick(object sender, ItemClickEventArgs e)
        {
            ContentDialog removeFolder = new ContentDialog
            {
                Title = "Remove this folder?",
                Content = "You're about to remove this folder from your library. The folder won't be removed, but you won't see its contents in your library.",
                CloseButtonText = "Cancel",
                PrimaryButtonText = "Remove"
            };

            ContentDialogResult result = await removeFolder.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                ListEntry clickedEntry = e.ClickedItem as ListEntry;
                FutureAccess.Remove(clickedEntry.Token);
                _Entries.Remove(clickedEntry);
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
    }
}
