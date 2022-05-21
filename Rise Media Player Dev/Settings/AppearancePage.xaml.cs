using System;
using System.Collections.Generic;
using Rise.App.Dialogs;
using Rise.App.ViewModels;
using Rise.Common;
using Rise.Common.Enums;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Settings
{
    public sealed partial class AppearancePage : Page
    {
        private SettingsViewModel ViewModel => App.SViewModel;

        private readonly List<string> Themes = new()
        {
            ResourceLoaders.AppearanceLoader.GetString("Light"),
            ResourceLoaders.AppearanceLoader.GetString("Dark"),
            ResourceLoaders.AppearanceLoader.GetString("System")
        };

        private readonly List<string> ColorThemes = new()
        {
            "No glaze",
            "Use system accent colour",
            "Use custom colour",
            "Use album art"
        };

        public AppearancePage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            ChangeThemeTip.IsOpen = false;

            foreach (Border border in RiseColorsPanel.Children)
            {
                border.PointerPressed += ColorBorder_PointerPressed;
            }
        }

        private void ColorBorder_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Border border = (Border)sender;

            var bg = border.Background as SolidColorBrush;
            var color = bg.Color;

            ViewModel.GlazeColors = new byte[4] { 25, color.R, color.G, color.B };

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

                    ViewModel.SelectedGlaze = GlazeTypes.None;
                    break;

                case 1:
                    Nothing.Visibility = Visibility.Visible;
                    Therest.Visibility = Visibility.Collapsed;
                    TextforGlaze.Visibility = Visibility.Collapsed;
                    RiseColorsPanel.Visibility = Visibility.Collapsed;

                    ViewModel.SelectedGlaze = GlazeTypes.AccentColor;
                    break;

                case 2:
                    Nothing.Visibility = Visibility.Collapsed;
                    Therest.Visibility = Visibility.Visible;
                    TextforGlaze.Visibility = Visibility.Visible;
                    RiseColorsPanel.Visibility = Visibility.Visible;

                    ViewModel.SelectedGlaze = GlazeTypes.CustomColor;
                    foreach (Border border in RiseColorsPanel.Children)
                    {
                        border.BorderBrush = new SolidColorBrush();
                        border.BorderThickness = new Thickness(0);

                        var bg = border.Background as SolidColorBrush;
                        var color = bg.Color;

                        var bytes = new byte[4] { 25, color.R, color.G, color.B };
                        var glaze = ViewModel.GlazeColors;

                        bool sameColor = true;
                        for (int i = 0; i < 4; i++)
                        {
                            if (bytes[i] != glaze[i])
                            {
                                sameColor = false;
                                break;
                            }
                        }

                        if (sameColor)
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

                    ViewModel.SelectedGlaze = GlazeTypes.MediaThumbnail;
                    break;
            }
        }

        private async void ChangeThemeTip_ActionButtonClick(Microsoft.UI.Xaml.Controls.TeachingTip sender, object args)
        {
            await CoreApplication.RequestRestartAsync("Theme changed");
        }

        private void ThemeChange_DropDownClosed(object sender, object e)
        {
            ChangeThemeTip.IsOpen = true;
        }
    }
}
