using Rise.App.Dialogs;
using Rise.App.Views;
using Rise.Common.Extensions;
using Rise.Common.Helpers;
using System;
using System.Linq;
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

        public AllSettingsPage()
        {
            InitializeComponent();
            _navigationHelper = new NavigationHelper(this);
            Current = this;

            TitleBar.SetTitleBarForCurrentView();

            SettingsMainFrame.Navigate(typeof(AppearanceBasePage));
            FinishNavigation();
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

        private async void ClassicDialog_Click(object sender, RoutedEventArgs e)
        {
            GoToMainPage();

            var diag = new SettingsDialogContainer();
            diag.Content = new SettingsPage();

            _ = await diag.ShowAsync();
        }

        private void Language_Click(object sender, RoutedEventArgs e)
        {
            SettingsMainFrame.Navigate(typeof(LanguagePage));
            FinishNavigation();
        }

        private void SettingsSidebar_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            string tag = args?.InvokedItemContainer?.Tag?.ToString();
            if (tag != null)
            {
                Type page;
                switch (tag)
                {
                    case "Appearance":
                        page = typeof(AppearanceBasePage);
                        break;
                    case "MediaLibrary":
                        page = typeof(MediaLibraryBasePage);
                        break;
                    case "Navigation":
                        page = typeof(NavigationPage);
                        break;
                    case "Playback":
                        page = typeof(PlaybackPage);
                        break;
                    case "Sync":
                        page = typeof(ComingSoonPage);
                        break;
                    case "Behaviour":
                        page = typeof(WindowsBehavioursPage);
                        break;
                    case "Components":
                        page = typeof(ComingSoonPage);
                        break;
                    case "About":
                        page = typeof(AboutPage);
                        break;
                    default:
                        page = typeof(MediaSourcesPage);
                        break;
                }

                if (SettingsMainFrame.CurrentSourcePageType != page)
                {
                    SettingsMainFrame.Navigate(page, null, args.RecommendedNavigationTransitionInfo);
                    SettingsMainFrame.BackStack.Clear();

                    FinishNavigation();
                }
            }
        }

        private void FinishNavigation()
        {
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
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            GoToMainPage();
        }

        private void GoToMainPage()
        {
            if (Frame.CanGoBack)
                Frame.GoBack();
            else
                _ = Frame.Navigate(typeof(MainPage));
        }
    }
}