using Rise.App.ViewModels;
using Rise.Common.Enums;
using Rise.Common.Extensions.Markup;
using Rise.Models;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Settings
{
    public sealed partial class AppearancePage : Page
    {
        private SettingsViewModel ViewModel => App.SViewModel;

        private readonly List<string> Themes = new()
        {
            ResourceHelper.GetString("Light"),
            ResourceHelper.GetString("Dark"),
            ResourceHelper.GetString("UseSystemSetting")
        };

        private readonly List<string> ColorThemes = new()
        {
            "No glaze",
            "Use system accent colour",
            "Use custom colour",
            "Use album art"
        };

        private static List<NamedColor> _glazeColors;
        private List<NamedColor> GlazeColors => _glazeColors;

        public AppearancePage()
        {
            InitializeComponent();

            if (_glazeColors == null)
            {
                _glazeColors = new();
                PopulateColors();
            }
        }

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            ThemeChange.SelectionChanged += ThemeChange_SelectionChanged;
            ColorThemeComboBox.SelectionChanged += ColorThemeComboBox_SelectionChanged;

            ColorGrid.SelectionChanged += ColorGrid_SelectionChanged;
        }

        private void PopulateColors()
        {
            AddColor("Yellow gold", 255, 185, 0);
            AddColor("Gold", 255, 140, 0);
            AddColor("Bright orange", 247, 99, 12);
            AddColor("Dark orange", 202, 80, 16);
            AddColor("Rust", 218, 59, 1);
            AddColor("Pale rust", 239, 105, 80);
            AddColor("Brick red", 209, 52, 56);
            AddColor("Mod red", 255, 67, 67);
            AddColor("Pale red", 231, 72, 86);
            AddColor("Red", 232, 17, 35);
            AddColor("Rose bright", 234, 0, 94);
            AddColor("Rose", 195, 0, 82);
            AddColor("Plum light", 227, 0, 140);
            AddColor("Plum", 191, 0, 119);
            AddColor("Orchid light", 194, 57, 179);
            AddColor("Orchid", 154, 0, 137);
            AddColor("Blue", 0, 120, 212);
            AddColor("Navy blue", 0, 99, 177);
            AddColor("Purple shadow", 142, 140, 216);
            AddColor("Purple shadow dark", 107, 105, 214);
            AddColor("Iris pastel", 135, 100, 184);
            AddColor("Iris spring", 116, 77, 169);
            AddColor("Violet red light", 177, 70, 194);
            AddColor("Violet red", 136, 23, 152);
            AddColor("Cool blue bright", 0, 153, 188);
            AddColor("Cool blue", 45, 125, 154);
            AddColor("Seafoam", 0, 183, 195);
            AddColor("Seafoam teal", 3, 131, 135);
            AddColor("Mint light", 0, 178, 148);
            AddColor("Mint dark", 1, 133, 116);
            AddColor("Turf green", 0, 204, 106);
            AddColor("Sport green", 16, 137, 62);
            AddColor("Grey", 122, 117, 116);
            AddColor("Grey brown", 93, 90, 88);
            AddColor("Steel blue", 104, 118, 138);
            AddColor("Metal blue", 81, 92, 107);
            AddColor("Pale moss", 86, 124, 116);
            AddColor("Moss", 72, 104, 96);
            AddColor("Meadow green", 73, 130, 5);
            AddColor("Green", 16, 124, 16);
            AddColor("Overcast", 118, 118, 118);
            AddColor("Storm", 76, 74, 72);
            AddColor("Blue grey", 105, 121, 126);
            AddColor("Grey dark", 74, 84, 89);
            AddColor("Liddy green", 100, 124, 100);
            AddColor("Sage", 82, 94, 84);
            AddColor("Camouflage desert", 132, 117, 69);
            AddColor("Camouflage", 126, 115, 95);

            static void AddColor(string name, byte r, byte g, byte b)
                => _glazeColors.Add(new(name, r, g, b));
        }
    }

    // Event handlers
    public sealed partial class AppearancePage
    {
        private void ThemeChange_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ChangeThemeTip.IsOpen = true;
        }

        private void ColorThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var glaze = (GlazeTypes)ColorThemeComboBox.SelectedIndex;
            if (ViewModel.SelectedGlaze == glaze)
                return;

            ViewModel.SelectedGlaze = glaze;
            switch (glaze)
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
                    var color = (NamedColor)ColorGrid.SelectedItem;
                    var col = color.Color;

                    ViewModel.GlazeColors = Color.FromArgb(25, col.R, col.G, col.B);
                    break;
            }
        }

        private void ColorGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var color = (NamedColor)e.AddedItems[0];
            var col = color.Color;

            ViewModel.GlazeColors = Color.FromArgb(25, col.R, col.G, col.B);
        }

        private void ThemeChange_DropDownClosed(object sender, object e)
        {

        }

        private async void ChangeThemeTip_ActionButtonClick(Microsoft.UI.Xaml.Controls.TeachingTip sender, object args)
        {
            await CoreApplication.RequestRestartAsync("Theme changed");
        }
    }
}
