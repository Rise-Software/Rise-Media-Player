using Rise.App.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;
using Rise.App.Views;

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

            _ = new ApplicationTitleBar(TitleBar);
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
                        SettingsMainFrame.Navigate(typeof(ComingSoonPage));
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
            _ = await typeof(Web.FeedbackPage).PlaceInWindowAsync(ApplicationViewMode.Default, 375, 600, true);
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
            SettingsPage.Current.SettingsFrame.Navigate(typeof(MediaLibraryPage));
        }


        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Window.Current.Content is Frame rootFrame && rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
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
                    GOBACKPAGE.Visibility = Visibility.Collapsed;
                    break;
                case "MediaLibraryBasePage":
                    MainSettingsHeaderIcon.Glyph = "\uEA69";
                    MainSettingsHeader.Text = "Media library";
                    GOBACKPAGE.Visibility = Visibility.Collapsed;
                    break;
                case "NavigationPage":
                    MainSettingsHeaderIcon.Glyph = "\uE8B0";
                    MainSettingsHeader.Text = "Navigation";
                    GOBACKPAGE.Visibility = Visibility.Collapsed;
                    break;
                case "PlaybackPage":
                    MainSettingsHeaderIcon.Glyph = "\uF4C3";
                    MainSettingsHeader.Text = "Playback & sound";
                    GOBACKPAGE.Visibility = Visibility.Collapsed;
                    break;
                case "ComingSoonPage":
                    MainSettingsHeaderIcon.Glyph = "\uE115";
                    MainSettingsHeader.Text = "Coming soon...";
                    GOBACKPAGE.Visibility = Visibility.Collapsed;
                    break;
                case "AboutPage":
                    MainSettingsHeaderIcon.Glyph = "\uE946";
                    MainSettingsHeader.Text = "About";
                    GOBACKPAGE.Visibility = Visibility.Collapsed;
                    break;
                case "MediaSourcesPage":
                    MainSettingsHeaderIcon.Glyph = "\uE115";
                    MainSettingsHeader.Text = "Media Sources";
                    GOBACKPAGE.Visibility = Visibility.Collapsed;
                    break;
                case "InsiderPage":
                    MainSettingsHeaderIcon.Glyph = "\uECA7";
                    MainSettingsHeader.Text = "Insider Hub";
                    GOBACKPAGE.Visibility = Visibility.Visible;
                    break;
                case "InsiderWallpapers":
                    MainSettingsHeaderIcon.Glyph = "\uE8B9";
                    MainSettingsHeader.Text = "Wallpapers";
                    GOBACKPAGE.Visibility = Visibility.Visible;
                    break;
                case "LanguagePage":
                    MainSettingsHeaderIcon.Glyph = "\uE12B";
                    MainSettingsHeader.Text = "Language";
                    GOBACKPAGE.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void GoBackAPage_Click(object sender, RoutedEventArgs e)
        {
            if (SettingsMainFrame.CanGoBack)
            {
                SettingsMainFrame.GoBack();
                FinishNavigation();
            }
        }

        private void GoBackToMain_Click(object sender, RoutedEventArgs e)
        {
            if (Window.Current.Content is Frame rootFrame && rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
            }
        }
    }
}