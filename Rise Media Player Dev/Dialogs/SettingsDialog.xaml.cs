using Microsoft.UI.Xaml.Controls;
using RMP.App.Common;
using RMP.App.Settings;
using RMP.App.Settings.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace RMP.App.Dialogs
{
    public sealed partial class SettingsDialog : ContentDialog
    {
        #region Variables
        public static SettingsDialog Current;
        private SettingsViewModel ViewModel => App.SViewModel;

        public ObservableCollection<string> Breadcrumbs =
            new ObservableCollection<string>();

        private IEnumerable<ToggleButton> Toggles { get; set; }

        private readonly ObservableCollection<FontIcon> FontIcons =
            new ObservableCollection<FontIcon>();

        private readonly ObservableCollection<ImageIcon> ImageIcons =
            new ObservableCollection<ImageIcon>();

        private double Breakpoint { get; set; }
        #endregion

        public SettingsDialog()
        {
            InitializeComponent();
            Current = this;

            Toggles = ItemGrid.GetChildren<ToggleButton>();

            foreach (ToggleButton toggle in Toggles)
            {
                DependencyObject content = toggle.Content as DependencyObject;

                FontIcons.Add(content.FindVisualChild<FontIcon>());
                ImageIcons.Add(content.FindVisualChild<ImageIcon>());
            }

            ChangeIcons(ViewModel.ColoredSettingsIcons);
            Library.IsChecked = true;

            // Calculate the breakpoints only on initial opening.
            Loaded += (s, e) => Opened += SettingsDialog_Opened;
        }

        private void SettingsDialog_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            FirstDefinition.Width = new GridLength(1, GridUnitType.Auto);

            Breakpoint = ItemGrid.DesiredSize.Width + SecondGrid.DesiredSize.Width;
            ResizeDialog(Window.Current.Bounds.Height, Window.Current.Bounds.Width);

            FirstDefinition.Width = new GridLength(1, GridUnitType.Star);

            Opened -= SettingsDialog_Opened;
            SizeChanged += ContentDialog_SizeChanged;

            // From now on, register the size changed event whenever the dialog gets opened.
            Opened += (s, e) => SizeChanged += ContentDialog_SizeChanged;
        }

        private void ContentDialog_SizeChanged(object sender, SizeChangedEventArgs e)
            => ResizeDialog(Window.Current.Bounds.Height, Window.Current.Bounds.Width);

        private void ResizeDialog(double height, double width)
        {
            SettingsFrame.Width = width < 800 ?
                width - 68 : 800 - 68;

            RootGrid.Height = height < 620 ?
                height - 64 : 620 - 64;

            if (width - 40 < Breakpoint)
            {
                foreach (ToggleButton button in Toggles)
                {
                    // Overflowing is needed.
                    button.MaxWidth = 38;
                }
            }
            else
            {
                foreach (ToggleButton button in Toggles)
                {
                    // Overflowing is not needed.
                    button.MaxWidth = double.PositiveInfinity;
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
            => Hide();

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            UncheckToggleButtons();

            ToggleButton clicked = (ToggleButton)sender;
            clicked.Checked -= ToggleButton_Checked;
            clicked.IsChecked = true;

            Breadcrumbs.Clear();
            switch (clicked.Tag.ToString())
            {
                case "LibraryItem":
                    _ = SettingsFrame.Navigate(typeof(MediaLibraryPage));
                    Breadcrumbs.Add(ResourceLoaders.SidebarLoader.GetString("Lib"));
                    break;

                case "PlaybackItem":
                    _ = SettingsFrame.Navigate(typeof(PlaybackPage));
                    Breadcrumbs.Add(ResourceLoaders.SidebarLoader.GetString("Play"));
                    break;

                case "AppearanceItem":
                    _ = SettingsFrame.Navigate(typeof(AppearancePage));
                    Breadcrumbs.Add(ResourceLoaders.SidebarLoader.GetString("Pers"));
                    break;

                case "AboutItem":
                    _ = SettingsFrame.Navigate(typeof(AboutPage));
                    Breadcrumbs.Add(ResourceLoaders.SidebarLoader.GetString("Abt"));
                    break;

                default:
                    break;
            }

            clicked.Checked += ToggleButton_Checked;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
            => await FileHelpers.LaunchURIAsync(URLs.Feedback);

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            UncheckToggleButtons();

            ToggleButton toggle = sender as ToggleButton;
            toggle.IsChecked = true;
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            UncheckToggleButtons();
            Breadcrumbs.Clear();

            MenuFlyoutItem item = sender as MenuFlyoutItem;
            string tag = item.Tag.ToString();
            switch (tag)
            {
                case "Langs":
                    SettingsFrame.Navigate(typeof(LanguagePage));
                    break;

                case "Ins":
                    SettingsFrame.Navigate(typeof(InsiderPage));
                    Breadcrumbs.Add(ResourceLoaders.SidebarLoader.GetString("Abt"));
                    break;
            }

            Breadcrumbs.Add(ResourceLoaders.SidebarLoader.GetString(tag));
        }

        private void UncheckToggleButtons()
        {
            foreach (ToggleButton button in Toggles)
            {
                button.Unchecked -= ToggleButton_Unchecked;
                button.IsChecked = false;
                button.Unchecked += ToggleButton_Unchecked;
            }

            About.Unchecked -= ToggleButton_Unchecked;
            About.IsChecked = false;
            About.Unchecked += ToggleButton_Unchecked;
        }

        private void IconFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            RadioMenuFlyoutItem item = sender as RadioMenuFlyoutItem;
            if (item.Tag.ToString() == "HC")
            {
                App.SViewModel.ColoredSettingsIcons = false;
                ChangeIcons(false);
            }
            else
            {
                App.SViewModel.ColoredSettingsIcons = true;
                ChangeIcons(true);
            }
        }

        private void ChangeIcons(bool coloredIconsVisible)
        {
            Visibility coloredIconVisibility = Visibility.Collapsed;
            Visibility monoIconVisibility = Visibility.Visible;

            if (coloredIconsVisible)
            {
                coloredIconVisibility = Visibility.Visible;
                monoIconVisibility = Visibility.Collapsed;
            }

            foreach (FontIcon icon in FontIcons)
            {
                icon.Visibility = monoIconVisibility;
            }

            foreach (ImageIcon icon in ImageIcons)
            {
                icon.Visibility = coloredIconVisibility;
            }
        }
    }
}
