using RMP.App.Common;
using RMP.App.Settings;
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
        public ObservableCollection<string> Breadcrumbs =
            new ObservableCollection<string>();

        private IEnumerable<ToggleButton> Toggles { get; set; }
        #endregion

        public SettingsDialog()
        {
            InitializeComponent();
            Current = this;

            Toggles = ItemGrid.GetChildren<ToggleButton>();
            Library.IsChecked = true;

            Opened += SettingsDialog_Opened;
        }

        private void SettingsDialog_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
            => ResizeDialog();

        private void ContentDialog_SizeChanged(object sender, SizeChangedEventArgs e)
            => ResizeDialog();

        private void ResizeDialog()
        {
            double windowWidth = Window.Current.Bounds.Width;
            double windowHeight = Window.Current.Bounds.Height;

            SettingsFrame.Width = windowWidth < 800 ?
                windowWidth - 68 : 800 - 68;

            RootGrid.Height = windowHeight < 620 ?
                windowHeight - 64 : 620 - 64;

            /*double gridWidth = ItemGrid.DesiredSize.Width;
            double itemsWidth = 0;

            foreach (ToggleButton button in Toggles)
            {
                itemsWidth += button.DesiredSize.Width;

                if (gridWidth < itemsWidth + 6)
                {
                    // Overflowing is needed.
                    Debug.WriteLine("Overflow!");
                    button.Height = 0;
                }
                else
                {
                    // Overflowing is not needed.
                    Debug.WriteLine("Not overflow!");
                    button.Height = double.NaN;
                }
                
                if (windowWidth == 500)
                {
                    button.Height = 0;
                }
            }*/
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
            => Hide();

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            foreach (ToggleButton button in Toggles)
            {
                button.Unchecked -= ToggleButton_Unchecked;
                button.IsChecked = false;
                button.Unchecked += ToggleButton_Unchecked;
            }

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
            => await Methods.LaunchURIAsync(URLs.Feedback);

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (ToggleButton button in Toggles)
            {
                button.Unchecked -= ToggleButton_Unchecked;
                button.IsChecked = false;
                button.Unchecked += ToggleButton_Unchecked;
            }

            ToggleButton toggle = sender as ToggleButton;
            toggle.IsChecked = true;
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
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
    }
}
