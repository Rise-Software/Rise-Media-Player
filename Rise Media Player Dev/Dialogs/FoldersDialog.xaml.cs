using Rise.App.Common;
using Rise.App.Views;
using Rise.Common.Enums;
using System;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Dialogs
{
    public sealed partial class FoldersDialog : Page
    {
        #region Variables
        public static FoldersDialog Current;
        private StorageLibrary MusicLibrary => App.MusicLibrary;
        #endregion

        public FoldersDialog()
        {
            InitializeComponent();
            Current = this;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is StorageFolder folder)
            {
                _ = await MusicLibrary.RequestRemoveFolderAsync(folder);
            }
        }

        private async void Done_Click(object sender, RoutedEventArgs e)
            => _ = await MainPage.Current.SDialog.ShowAsync(ExistingDialogOptions.CloseExisting);

        private async void Add_Click(object sender, RoutedEventArgs e)
            => await App.MusicLibrary.RequestAddFolderAsync();
    }
}
