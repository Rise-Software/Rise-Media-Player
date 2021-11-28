using Rise.App.Common;
using Rise.App.Views;
using System;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using static Rise.App.Common.Enums;

namespace Rise.App.Dialogs
{
    public sealed partial class VFoldersDialog : Page
    {
        #region Variables
        public static VFoldersDialog Current;
        private StorageLibrary VideoLibrary => App.VideoLibrary;
        #endregion

        public VFoldersDialog()
        {
            InitializeComponent();
            Current = this;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is StorageFolder folder)
            {
                _ = await VideoLibrary.RequestRemoveFolderAsync(folder);
            }
        }

        private async void Done_Click(object sender, RoutedEventArgs e)
            => _ = await MainPage.Current.SDialog.ShowAsync(ExistingDialogOptions.CloseExisting);

        private async void Add_Click(object sender, RoutedEventArgs e)
            => await App.VideoLibrary.RequestAddFolderAsync();
    }
}
