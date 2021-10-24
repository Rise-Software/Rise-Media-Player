using System;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is StorageFolder folder)
            {
                _ = await MusicLibrary.RequestRemoveFolderAsync(folder);
            }
        }
    }
}
