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
        public AllSettingsPage()
        {
            InitializeComponent();
            _navigationHelper = new NavigationHelper(this);

            _ = new ApplicationTitleBar(TitleBar);
        }

        private void SettingsSidebar_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            var selectedItem = (Microsoft.UI.Xaml.Controls.NavigationViewItem)args.SelectedItem;
            string selectedItemTag = ((string)selectedItem.Tag);

            if (selectedItemTag == "Appearance")
            {
                MainSettingsHeaderIcon.Glyph = "\uE771";
                MainSettingsHeader.Text = "Appearance";
                SettingsMainFrame.Navigate(typeof(Settings.AppearancePage));
            }
            else if (selectedItemTag == "MediaLibrary")
            {
                MainSettingsHeaderIcon.Glyph = "\uEA69";
                MainSettingsHeader.Text = "Media library";
                SettingsMainFrame.Navigate(typeof(Settings.MediaLibraryPage));
            }
            else if (selectedItemTag == "Navigation")
            {
                MainSettingsHeaderIcon.Glyph = "\uE8B0";
                MainSettingsHeader.Text = "Navigation";
                SettingsMainFrame.Navigate(typeof(Settings.NavigationPage));
            }
            else if (selectedItemTag == "Playback")
            {
                MainSettingsHeaderIcon.Glyph = "\uF4C3";
                MainSettingsHeader.Text = "Playback & sound";
                SettingsMainFrame.Navigate(typeof(Settings.PlaybackPage));
            }
            else if (selectedItemTag == "Sync")
            {
                MainSettingsHeaderIcon.Glyph = "\uE117";
                MainSettingsHeader.Text = "Syncing";
                SettingsMainFrame.Navigate(typeof(Settings.AboutPage));
            }
            else if (selectedItemTag == "Behaviour")
            {
                MainSettingsHeaderIcon.Glyph = "\uE7C4";
                MainSettingsHeader.Text = "Windows behaviours";
                SettingsMainFrame.Navigate(typeof(Settings.AboutPage));
            }
            else if (selectedItemTag == "Components")
            {
                MainSettingsHeaderIcon.Glyph = "\uEA86";
                MainSettingsHeader.Text = "Manage components";
                SettingsMainFrame.Navigate(typeof(Settings.AboutPage));
            }

            else if (selectedItemTag == "About")
            {
                MainSettingsHeaderIcon.Glyph = "\uE946";
                MainSettingsHeader.Text = "About";
                SettingsMainFrame.Navigate(typeof(Settings.AboutPage));
            }

            else
            {
                MainSettingsHeaderIcon.Glyph = "\uE115";
                MainSettingsHeader.Text = "No page selected";
                SettingsMainFrame.Navigate(typeof(Settings.MediaSourcesPage));
            }
        }

        private async void FeedbackSettings_Click(object sender, RoutedEventArgs e)
        {
            _ = await typeof(Web.FeedbackPage).PlaceInWindowAsync(ApplicationViewMode.Default, 500, 600, true);
        }
    }
}
