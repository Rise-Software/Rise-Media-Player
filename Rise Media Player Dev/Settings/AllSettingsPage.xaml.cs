using Rise.App.Views;
using Rise.Common.Extensions;
using Rise.Common.Helpers;
using System;
using System.Linq;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Rise.App.Settings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AllSettingsPage : Page
    {
        private readonly NavigationHelper _navigationHelper;
        public static AllSettingsPage Current;
        private int _selectedIndex;

        public AllSettingsPage()
        {
            InitializeComponent();
            _navigationHelper = new NavigationHelper(this);
            Current = this;
        }

        private void SettingsSidebar_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            var selectedItem = (Microsoft.UI.Xaml.Controls.NavigationViewItem)args.SelectedItem;
            if (selectedItem != null)
            {
                string selectedItemTag = selectedItem.Tag as string;

                switch (selectedItemTag)
                {
                    case "Appearance":
                        SettingsMainFrame.Navigate(typeof(AppearanceBasePage));
                        break;
                    case "MediaLibrary":
                        SettingsMainFrame.Navigate(typeof(MediaLibraryBasePage));
                        break;
                    case "Navigation":
                        SettingsMainFrame.Navigate(typeof(NavigationPage));
                        break;
                    case "Playback":
                        SettingsMainFrame.Navigate(typeof(PlaybackPage));
                        break;
                    case "Sync":
                        SettingsMainFrame.Navigate(typeof(ComingSoonPage));
                        break;
                    case "Behaviour":
                        SettingsMainFrame.Navigate(typeof(WindowsBehavioursPage));
                        break;
                    case "Components":
                        SettingsMainFrame.Navigate(typeof(ComingSoonPage));
                        break;
                    case "About":
                        SettingsMainFrame.Navigate(typeof(AboutPage));
                        break;
                    default:
                        SettingsMainFrame.Navigate(typeof(MediaSourcesPage));
                        break;
                }

                FinishNavigation();
            }
        }

        private async void FeedbackSettings_Click(object sender, RoutedEventArgs e)
        {
            _ = await typeof(Web.FeedbackPage).
                ShowInApplicationViewAsync(null, 375, 600, true);
        }

        private void Insider_Click(object sender, RoutedEventArgs e)
        {
            SettingsMainFrame.Navigate(typeof(InsiderPage));
            FinishNavigation();
        }

        private void Language_Click(object sender, RoutedEventArgs e)
        {
            SettingsMainFrame.Navigate(typeof(LanguagePage));
            FinishNavigation();
        }

        private void SettingsSidebar_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            //var selectedItem = (Microsoft.UI.Xaml.Controls.NavigationViewItem)args.InvokedItem;
            //string selectedItemTag = selectedItem.Tag as string;

            //switch (selectedItemTag)
            //{
            //    case "Appearance":
            //        MainSettingsHeaderIcon.Glyph = "\uE771";
            //        MainSettingsHeader.Text = "Appearance";
            //        SettingsMainFrame.Navigate(typeof(AppearanceBasePage));
            //        break;
            //    case "MediaLibrary":
            //        MainSettingsHeaderIcon.Glyph = "\uEA69";
            //        MainSettingsHeader.Text = "Media library";
            //        SettingsMainFrame.Navigate(typeof(MediaLibraryBasePage));
            //        break;
            //    case "Navigation":
            //        MainSettingsHeaderIcon.Glyph = "\uE8B0";
            //        MainSettingsHeader.Text = "Navigation";
            //        SettingsMainFrame.Navigate(typeof(NavigationPage));
            //        break;
            //    case "Playback":
            //        MainSettingsHeaderIcon.Glyph = "\uF4C3";
            //        MainSettingsHeader.Text = "Playback & sound";
            //        SettingsMainFrame.Navigate(typeof(PlaybackPage));
            //        break;
        }

        private async void ClassicDialog_Click(object sender, RoutedEventArgs e)
        {
            if (Window.Current.Content is Frame rootFrame && rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
            }
            _ = await MainPage.Current.SDialog.ShowAsync();
            _ = SettingsPage.Current.SettingsFrame.Navigate(typeof(MediaLibraryPage));
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Window.Current.Content is Frame rootFrame && rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
                MainPage.Current.AppTitleBar.Visibility = Visibility.Visible;
                MainPage.Current.AppTitleBar.SetupTitleBar();
            }
        }

        private void FinishNavigation()
        {
            AllSettingsPage.Current.GOBACKPAGE.Visibility = Visibility.Collapsed;
            string type = SettingsMainFrame.CurrentSourcePageType.ToString();
            string tag = type.Split('.').Last();

            switch (tag)
            {
                case "AppearanceBasePage":
                    MainSettingsHeaderIcon.Glyph = "\uE771";
                    MainSettingsHeader.Text = "Appearance";
                    break;
                case "MediaLibraryBasePage":
                    MainSettingsHeaderIcon.Glyph = "\uEA69";
                    MainSettingsHeader.Text = "Media library";
                    break;
                case "NavigationPage":
                    MainSettingsHeaderIcon.Glyph = "\uE8B0";
                    MainSettingsHeader.Text = "Navigation";
                    break;
                case "PlaybackPage":
                    MainSettingsHeaderIcon.Glyph = "\uF4C3";
                    MainSettingsHeader.Text = "Playback & sound";
                    break;
                case "ComingSoonPage":
                    MainSettingsHeaderIcon.Glyph = "\uE115";
                    MainSettingsHeader.Text = "Coming soon...";
                    break;
                case "WindowsBehavioursPage":
                    MainSettingsHeaderIcon.Glyph = "\uEC7A";
                    MainSettingsHeader.Text = "System behaviours";
                    break;
                case "AboutPage":
                    MainSettingsHeaderIcon.Glyph = "\uE946";
                    MainSettingsHeader.Text = "About";
                    break;
                case "MediaSourcesPage":
                    MainSettingsHeaderIcon.Glyph = "\uE115";
                    MainSettingsHeader.Text = "Media Sources";
                    break;
                case "InsiderPage":
                    MainSettingsHeaderIcon.Glyph = "\uF1AD";
                    MainSettingsHeader.Text = "Insider Hub";
                    break;
                case "LanguagePage":
                    MainSettingsHeaderIcon.Glyph = "\uE12B";
                    MainSettingsHeader.Text = "Language";
                    break;
            }
        }

        private void GoBackAPage_Click(object sender, RoutedEventArgs e)
        {
            if (SettingsMainFrame.CanGoBack)
            {
                SettingsMainFrame.GoBack();
                FinishNavigation();
                GOBACKPAGE.Visibility = Visibility.Collapsed;
            }
        }

        private void GoBackToMain_Click(object sender, RoutedEventArgs e)
        {
            if (Window.Current.Content is Frame rootFrame && rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
            }
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width <850 && e.NewSize.Width > 650)
            {
                SidebarContainer.Visibility = Visibility.Visible;
                SidebarContainer.Width = 114;
                SettingsSidebar.PaneDisplayMode = (Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode)NavigationViewPaneDisplayMode.LeftCompact;
                SettingsTitle.Visibility = Visibility.Collapsed;
                SmallSettingsButton.Visibility = Visibility.Visible;
            }
            else if (e.NewSize.Width < 650)
            {
                SidebarContainer.Visibility = Visibility.Collapsed;
                SettingsTitle.Visibility = Visibility.Collapsed;
                SmallSettingsButton.Visibility = Visibility.Visible;
            }
            else
            {
                SidebarContainer.Visibility = Visibility.Visible;
                SidebarContainer.Width = 301;
                SettingsSidebar.PaneDisplayMode = (Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode)NavigationViewPaneDisplayMode.Left;
                SettingsTitle.Visibility = Visibility.Visible;
                SmallSettingsButton.Visibility = Visibility.Collapsed;
            }
        }
    }
}