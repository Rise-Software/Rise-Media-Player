using Rise.App.Common;
using Rise.App.Dialogs;
using Rise.App.ViewModels;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Settings
{
    public sealed partial class AppearancePage : Page
    {
        private SettingsViewModel ViewModel => App.SViewModel;

        private readonly List<string> Themes = new List<string>
        {
            ResourceLoaders.AppearanceLoader.GetString("Light"),
            ResourceLoaders.AppearanceLoader.GetString("Dark"),
            ResourceLoaders.AppearanceLoader.GetString("System")
        };

        private readonly List<string> ColorThemes = new List<string>
        {
            "No glaze",
            "Use system accent colour",
            "Use custom colour",
            "Use album art"
        };

        private readonly List<string> Startup = new List<string>
        {
            ResourceLoaders.AppearanceLoader.GetString("Home"),
            ResourceLoaders.AppearanceLoader.GetString("NowPlaying"),
            ResourceLoaders.AppearanceLoader.GetString("Playlists"),
            ResourceLoaders.AppearanceLoader.GetString("Songs"),
            ResourceLoaders.AppearanceLoader.GetString("Artists"),
            ResourceLoaders.AppearanceLoader.GetString("Albums"),
            ResourceLoaders.AppearanceLoader.GetString("Genres"),
            ResourceLoaders.AppearanceLoader.GetString("LocalVideos"),
        };

        public AppearancePage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;

            foreach (Border border in RiseColorsPanel.Children)
            {
                border.PointerPressed += ColorBorder_PointerPressed;
            }
        }

        private void ColorBorder_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Border border = (Border)sender;

            ViewModel.Color = RiseColorsPanel.Children.IndexOf(border);
            foreach (Border border1 in RiseColorsPanel.Children)
            {
                border1.BorderBrush = new SolidColorBrush();
                border1.BorderThickness = new Thickness(0);
            }
            border.BorderBrush = (Brush)Resources["SystemControlForegroundChromeWhiteBrush"];
            border.BorderThickness = new Thickness(3);
        }

        private void SidebarCustomize_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(NavigationPage));

            SettingsDialogContainer.Breadcrumbs.
                Add(ResourceLoaders.AppearanceLoader.GetString("Sidebar"));
        }

        private void ColorThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (ColorThemeComboBox.SelectedIndex)
            {
                case 0:
                    Nothing.Visibility = Visibility.Visible;
                    Therest.Visibility = Visibility.Collapsed;
                    TextforGlaze.Visibility = Visibility.Collapsed;
                    RiseColorsPanel.Visibility = Visibility.Collapsed;
                    ViewModel.Color = -1;
                    break;

                case 1:
                    Nothing.Visibility = Visibility.Visible;
                    Therest.Visibility = Visibility.Collapsed;
                    TextforGlaze.Visibility = Visibility.Collapsed;
                    RiseColorsPanel.Visibility = Visibility.Collapsed;
                    ViewModel.Color = -2;
                    break;

                case 2:
                    Nothing.Visibility = Visibility.Collapsed;
                    Therest.Visibility = Visibility.Visible;
                    TextforGlaze.Visibility = Visibility.Visible;
                    RiseColorsPanel.Visibility = Visibility.Visible;
                    if (ViewModel.Color < 0)
                    {
                        ViewModel.Color = 0;
                    }
                    foreach (Border border in RiseColorsPanel.Children)
                    {
                        border.BorderBrush = new SolidColorBrush();
                        border.BorderThickness = new Thickness(0);
                        if (ViewModel.Color == RiseColorsPanel.Children.IndexOf(border))
                        {
                            border.BorderBrush = (Brush)Resources["SystemControlForegroundChromeWhiteBrush"];
                            border.BorderThickness = new Thickness(3);
                        }
                    }
                    break;

                case 3:
                    Nothing.Visibility = Visibility.Visible;
                    Therest.Visibility = Visibility.Collapsed;
                    TextforGlaze.Visibility = Visibility.Collapsed;
                    RiseColorsPanel.Visibility = Visibility.Collapsed;
                    ViewModel.Color = -3;
                    break;
            }
        }


    }
}
