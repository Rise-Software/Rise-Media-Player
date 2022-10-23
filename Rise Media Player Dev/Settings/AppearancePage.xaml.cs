using Rise.App.Dialogs;
using Rise.App.ViewModels;
using Rise.Common;
using Rise.Common.Enums;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

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

            color.A = 25;
            ViewModel.GlazeColors = color;

            foreach (Border border1 in RiseColorsPanel.Children)
            {
                border1.BorderBrush = new SolidColorBrush();
                border1.BorderThickness = new Thickness(0);
            }

            border.BorderBrush = (Brush)Resources["SystemControlForegroundChromeWhiteBrush"];
            border.BorderThickness = new Thickness(3);
        }

        private void ColorThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.SelectedGlaze = (GlazeTypes)ColorThemeComboBox.SelectedIndex;

            string state = "CustomGlazeDisabled";
            switch (ViewModel.SelectedGlaze)
            {
                case GlazeTypes.None:
                    ViewModel.GlazeColors = Colors.Transparent;
                    break;

                case GlazeTypes.AccentColor:
                    var uiSettings = new UISettings();
                    var accent = uiSettings.GetColorValue(UIColorType.Accent);
                    accent.A = 25;

                    ViewModel.GlazeColors = accent;
                    break;

                case GlazeTypes.CustomColor:
                    state = "CustomGlazeEnabled";
                    foreach (Border border in RiseColorsPanel.Children)
                    {
                        border.BorderBrush = new SolidColorBrush();
                        border.BorderThickness = new Thickness(0);

                        var bg = border.Background as SolidColorBrush;
                        var color = bg.Color;
                        color.A = 25;

                        if (ViewModel.GlazeColors.Equals(color))
                        {
                            border.BorderBrush = (Brush)Resources["SystemControlForegroundChromeWhiteBrush"];
                            border.BorderThickness = new Thickness(3);
                            break;
                        }
                    }
                    break;
            }

            VisualStateManager.GoToState(this, state, false);
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
