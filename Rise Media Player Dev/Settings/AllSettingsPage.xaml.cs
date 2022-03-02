﻿using Rise.App.Common;
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
            string selectedItemTag = selectedItem.Tag as string;

            switch (selectedItemTag)
            {
                case "Appearance":
                    MainSettingsHeaderIcon.Glyph = "\uE771";
                    MainSettingsHeader.Text = "Appearance";
                    SettingsMainFrame.Navigate(typeof(AppearanceBasePage));
                    break;
                case "MediaLibrary":
                    MainSettingsHeaderIcon.Glyph = "\uEA69";
                    MainSettingsHeader.Text = "Media library";
                    SettingsMainFrame.Navigate(typeof(MediaLibraryBasePage));
                    break;
                case "Navigation":
                    MainSettingsHeaderIcon.Glyph = "\uE8B0";
                    MainSettingsHeader.Text = "Navigation";
                    SettingsMainFrame.Navigate(typeof(NavigationPage));
                    break;
                case "Playback":
                    MainSettingsHeaderIcon.Glyph = "\uF4C3";
                    MainSettingsHeader.Text = "Playback & sound";
                    SettingsMainFrame.Navigate(typeof(PlaybackPage));
                    break;
                case "Sync":
                    MainSettingsHeaderIcon.Glyph = "\uE117";
                    MainSettingsHeader.Text = "Syncing";
                    SettingsMainFrame.Navigate(typeof(ComingSoonPage));
                    break;
                case "Behaviour":
                    MainSettingsHeaderIcon.Glyph = "\uE7C4";
                    MainSettingsHeader.Text = "Windows behaviours";
                    SettingsMainFrame.Navigate(typeof(WindowsStartupPage));
                    break;
                case "Components":
                    MainSettingsHeaderIcon.Glyph = "\uEA86";
                    MainSettingsHeader.Text = "Manage components";
                    SettingsMainFrame.Navigate(typeof(ComingSoonPage));
                    break;
                case "About":
                    MainSettingsHeaderIcon.Glyph = "\uE946";
                    MainSettingsHeader.Text = "About";
                    SettingsMainFrame.Navigate(typeof(AboutPage));
                    break;
                default:
                    MainSettingsHeaderIcon.Glyph = "\uE115";
                    MainSettingsHeader.Text = "No page selected";
                    SettingsMainFrame.Navigate(typeof(MediaSourcesPage));
                    break;
            }
        }

        private async void FeedbackSettings_Click(object sender, RoutedEventArgs e)
        {
            _ = await typeof(Web.FeedbackPage).PlaceInWindowAsync(ApplicationViewMode.Default, 375, 600, true);
        }

        private void Insider_Click(object sender, RoutedEventArgs e)
        {
            MainSettingsHeaderIcon.Glyph = "\uF1AD";
            MainSettingsHeader.Text = "Insider Hub";
            SettingsMainFrame.Navigate(typeof(InsiderPage));
        }

        private void Language_Click(object sender, RoutedEventArgs e)
        {
            MainSettingsHeaderIcon.Glyph = "\uE12B";
            MainSettingsHeader.Text = "Language";
            SettingsMainFrame.Navigate(typeof(LanguagePage));
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
            => _ = await MainPage.Current.SDialog.ShowAsync();

        private void BacktoMediaLibrary_Click(object sender, RoutedEventArgs e)
        {

        }
    }
    }

