using RMP.App.Settings;
using System;
using System.Collections.ObjectModel;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace RMP.App.Dialogs
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button clicked = sender as Button;
            FutureAccess.Remove(clicked.Tag.ToString());
            Entries.Clear();
            FillList();
        }
    }
}
