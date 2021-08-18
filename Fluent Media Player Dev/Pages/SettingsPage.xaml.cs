using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Fluent_Media_Player_Dev.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();
            FillList();
        }

        private async void FillList()
        {
            Windows.Storage.AccessCache.StorageItemAccessList fa =
                Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList;
            foreach (Windows.Storage.AccessCache.AccessListEntry entry in fa.Entries)
            {
                // Get folder from future access list
                string faToken = entry.Token;
                StorageFolder fold = await fa.GetFolderAsync(faToken);
                FolderList.Items.Add(fold.Path);
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
                Windows.Storage.AccessCache.StorageItemAccessList fa =
                    Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList;
                foreach (Windows.Storage.AccessCache.AccessListEntry entry in fa.Entries)
                {
                    // Get folder from future access list
                    string faToken = entry.Token;
                    StorageFolder fold = await fa.GetFolderAsync(faToken);
                    if (folder.Path == fold.Path)
                    {
                        return;
                    }
                }

                string token = Guid.NewGuid().ToString();

                Windows.Storage.AccessCache.StorageApplicationPermissions.
                FutureAccessList.AddOrReplace(token, folder);

                FolderList.Items.Add(folder.Path);
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
        }
    }
}
