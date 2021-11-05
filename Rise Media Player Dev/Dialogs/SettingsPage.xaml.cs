using Microsoft.UI.Xaml.Controls;
using RMP.App.Common;
using RMP.App.Settings;
using RMP.App.Settings.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;

namespace RMP.App.Dialogs
{
    public sealed partial class SettingsPage : Page
    {
        #region Variables
        private SettingsViewModel ViewModel => App.SViewModel;

        public ObservableCollection<string> Breadcrumbs =>
            SettingsDialogContainer.Breadcrumbs;

        private IEnumerable<ToggleButton> Toggles { get; set; }

        private readonly ObservableCollection<FontIcon> FontIcons =
            new ObservableCollection<FontIcon>();

        private readonly ObservableCollection<ImageIcon> ImageIcons =
            new ObservableCollection<ImageIcon>();

        private double Breakpoint { get; set; }
        #endregion

        public SettingsPage()
        {
            InitializeComponent();
            Toggles = ItemGrid.GetChildren<ToggleButton>();

            foreach (ToggleButton toggle in Toggles)
            {
                DependencyObject content = toggle.Content as DependencyObject;

                FontIcons.Add(content.FindVisualChild<FontIcon>());
                ImageIcons.Add(content.FindVisualChild<ImageIcon>());
            }

            ChangeIcons(ViewModel.ColoredSettingsIcons);
            Library.IsChecked = true;

            Loaded += SettingsDialog_Loaded;
            SizeChanged += (s, a) => ResizeDialog(Window.Current.Bounds.Height, Window.Current.Bounds.Width);

            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        private void SettingsDialog_Loaded(object sender, RoutedEventArgs e)
        {
            FirstDefinition.Width = new GridLength(1, GridUnitType.Auto);

            Breakpoint = ItemGrid.DesiredSize.Width + SecondGrid.DesiredSize.Width;
            ResizeDialog(Window.Current.Bounds.Height, Window.Current.Bounds.Width);
            FirstDefinition.Width = new GridLength(1, GridUnitType.Star);
        }

        private void ResizeDialog(double height, double width)
        {
            RootGrid.Width = width < 800 ?
                width - 12 : 800 - 12;

            RootGrid.Height = height < 620 ?
                height - 68 : 620 - 68;

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
            => SettingsDialogContainer.Current.Hide();

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
            => _ = await URLs.Feedback.LaunchAsync();

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
