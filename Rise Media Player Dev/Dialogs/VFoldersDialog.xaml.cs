using Rise.App.Common;
using Rise.App.Views;
using Rise.Common.Enums;
using System;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Dialogs
{
    public sealed partial class VFoldersDialog : Page
    {
        #region Variables
        public static VFoldersDialog Current;
        private StorageLibrary VideoLibrary => App.VideoLibrary;
        private StorageLibrary MusicLibrary => App.MusicLibrary;
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

        private async void AddVideo_Click(object sender, RoutedEventArgs e)
            => await App.VideoLibrary.RequestAddFolderAsync();

        private async void AddMusic_Click(object sender, RoutedEventArgs e)
            => await App.MusicLibrary.RequestAddFolderAsync();

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is StorageFolder folder)
            {
                _ = await MusicLibrary.RequestRemoveFolderAsync(folder);
            }
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is StorageFolder folder)
            {
                await Launcher.LaunchFolderAsync(folder);
            }
        }

        private void Border_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {

        }

        private void Border_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {

        }

        private void Border_PointerEntered_1(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {

        }

        private void Border_PointerExited_1(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {

        }
    }
}
