using System;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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
    }
}
