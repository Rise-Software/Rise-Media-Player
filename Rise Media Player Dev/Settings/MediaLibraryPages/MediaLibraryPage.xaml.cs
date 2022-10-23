using Rise.App.ViewModels;
using Rise.Common;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Settings
{
    public sealed partial class MediaLibraryPage : Page
    {
        #region Variables
        private SettingsViewModel ViewModel => App.SViewModel;

        private readonly List<string> Deletion = new()
        {
            ResourceLoaders.MediaLibraryLoader.GetString("OnlyApp"),
            ResourceLoaders.MediaLibraryLoader.GetString("Device")
        };
        #endregion

        public MediaLibraryPage()
        {
            InitializeComponent();
        }

        private void GotoManage_Click(object sender, RoutedEventArgs e)
        {
            AllSettingsPage.Current.MainSettingsHeader.Text = "Manage local media folders";
            AllSettingsPage.Current.MainSettingsHeaderIcon.Glyph = "\uE838";
            AllSettingsPage.Current.SettingsMainFrame.Navigate(typeof(MediaSourcesPage));
        }
    }
}
