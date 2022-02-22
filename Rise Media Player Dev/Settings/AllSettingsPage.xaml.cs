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
            this.InitializeComponent();
            _navigationHelper = new NavigationHelper(this);
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;

            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            // Hide default title bar.
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            UpdateTitleBarLayout(coreTitleBar);

            // Set XAML element as a draggable region.
            Window.Current.SetTitleBar(AppTitleBar);

            // Register a handler for when the size of the overlaid caption control changes.
            // For example, when the app moves to a screen with a different DPI.
            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;

            // Register a handler for when the title bar visibility changes.
            // For example, when the title bar is invoked in full screen mode.
            coreTitleBar.IsVisibleChanged += CoreTitleBar_IsVisibleChanged;

            //Register a handler for when the window changes focus
            Window.Current.Activated += Current_Activated;
        }

        private async Task<bool> OpenPageAsWindowAsync(Type t)
        {
            CoreApplicationView view = CoreApplication.CreateNewView();
            int id = 0;

            await view.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Frame frame = new();
                _ = frame.Navigate(t, null);
                Window.Current.Content = frame;
                Window.Current.Activate();
                id = ApplicationView.GetForCurrentView().Id;
            });

            return await ApplicationViewSwitcher.TryShowAsStandaloneAsync(id);
        }

        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            UpdateTitleBarLayout(sender);
        }

        private void UpdateTitleBarLayout(CoreApplicationViewTitleBar coreTitleBar)
        {
            // Update title bar control size as needed to account for system size changes.
            AppTitleBar.Height = coreTitleBar.Height;

            // Ensure the custom title bar does not overlap window caption controls
            Thickness currMargin = AppTitleBar.Margin;
            AppTitleBar.Margin = new Thickness(currMargin.Left, currMargin.Top, coreTitleBar.SystemOverlayRightInset, currMargin.Bottom);
        }

        private void CoreTitleBar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
        {
            if (sender.IsVisible)
            {
                AppTitleBar.Visibility = Visibility.Visible;
            }
            else
            {
                AppTitleBar.Visibility = Visibility.Collapsed;
            }
        }

        // Update the TitleBar based on the inactive/active state of the app
        private void Current_Activated(object sender, Windows.UI.Core.WindowActivatedEventArgs e)
        {
            SolidColorBrush defaultForegroundBrush = (SolidColorBrush)Application.Current.Resources["TextFillColorPrimaryBrush"];
            SolidColorBrush inactiveForegroundBrush = (SolidColorBrush)Application.Current.Resources["TextFillColorDisabledBrush"];

            if (e.WindowActivationState == Windows.UI.Core.CoreWindowActivationState.Deactivated)
            {
                AppTitle.Foreground = inactiveForegroundBrush;
            }
            else
            {
                AppTitle.Foreground = defaultForegroundBrush;
            }
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
            _ = await OpenPageAsWindowAsync(typeof(Web.FeedbackPage));
        }
    }
}
