using Fluent_Media_Player_Dev.Settings;
using System;
using System.Collections.ObjectModel;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Fluent_Media_Player_Dev.Dialogs
{
    public sealed partial class FoldersDialog : Page
    {
        #region Variables
        public static FoldersDialog Current;
        public ObservableCollection<ListEntry> Entries = new ObservableCollection<ListEntry>();
        public StorageItemAccessList FutureAccess { get; set; }
        #endregion
        #region Classes
        public class ListEntry
        {
            public string Path { get; set; }
            public string DisplayName { get; set; }
            public string Token { get; set; }
        }
        #endregion
        public FoldersDialog()
        {
            FutureAccess = StorageApplicationPermissions.FutureAccessList;
            FillList();
            this.InitializeComponent();
            Current = this;
        }

        public async void FillList()
        {
            foreach (AccessListEntry entry in FutureAccess.Entries)
            {
                // Get folder from future access list
                string faToken = entry.Token;
                StorageFolder folder = await FutureAccess.GetFolderAsync(faToken);

                Entries.Add(new ListEntry
                {
                    Path = folder.Path,
                    DisplayName = folder.DisplayName,
                    Token = faToken
                });
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            MediaLibraryPage.Current.dialog.Hide();
            ContentDialog removeFolder = new ContentDialog
            {
                Title = "Remove this folder?",
                Content = "You're about to remove this folder from your library.",
                CloseButtonText = "Cancel",
                PrimaryButtonText = "Remove"
            };

            ContentDialogResult result = await removeFolder.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                var hi = sender as Button;
                FutureAccess.Remove(hi.Tag.ToString());
                Entries.Clear();
                FillList();
            }

            await MediaLibraryPage.Current.dialog.ShowAsync();
        }
    }
}
