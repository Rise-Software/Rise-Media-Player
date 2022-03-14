using Rise.App.Dialogs;
using Rise.App.ViewModels;
using Rise.Common;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Settings
{
    public sealed partial class MediaLibraryPage : Page
    {
        #region Variables
        private SettingsViewModel ViewModel => App.SViewModel;

        internal static MediaLibraryPage Current;

        private readonly List<string> Deletion = new()
        {
            ResourceLoaders.MediaLibraryLoader.GetString("OnlyApp"),
            ResourceLoaders.MediaLibraryLoader.GetString("Device")
        };
        #endregion

        public MediaLibraryPage()
        {
            InitializeComponent();
            Current = this;

            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        private void GotoManage_Click(object sender, RoutedEventArgs e)
        {
            AllSettingsPage.Current.GOBACKPAGE.Visibility = Visibility.Visible;
            AllSettingsPage.Current.MainSettingsHeader.Text = "Manage local media folders";
            AllSettingsPage.Current.MainSettingsHeaderIcon.Glyph = "\uE838";
            AllSettingsPage.Current.SettingsMainFrame.Navigate(typeof(MediaSourcesPage));

            SettingsPage.Current.SettingsFrame.Navigate(typeof(MediaSourcesPage));
        }

        private void CommandBarButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            switch (button.Tag.ToString())
            {
                case "Insider":
                    _ = Frame.Navigate(typeof(InsiderPage));
                    SettingsDialogContainer.Breadcrumbs.Add
                        (ResourceLoaders.SidebarLoader.GetString("Ins"));
                    break;

                case "OnlineServices":
                    break;
                default:
                    break;
            }
        }



    }
}
