using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace RMP.App.Dialogs
{
    public sealed partial class FoldersDialog : Page
    {
        #region Variables
        public static FoldersDialog Current;
        public StorageLibrary MusicLibrary => App.MusicLibrary;
        #endregion

        public FoldersDialog()
        {
            InitializeComponent();
            Current = this;
        }

        private async void FolderList_ItemClick(object sender, ItemClickEventArgs e)
        {
            _ = await MusicLibrary.RequestRemoveFolderAsync((StorageFolder)e.ClickedItem);
        }
    }
}
