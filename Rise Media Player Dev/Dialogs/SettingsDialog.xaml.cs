using RMP.App.Common;
using RMP.App.Settings;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

            Library.IsChecked = true;
            Toggles = ItemGrid.GetChildren<ToggleButton>();
        }

        private void ContentDialog_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double windowWidth = Window.Current.Bounds.Width;
            double windowHeight = Window.Current.Bounds.Height;

            Debug.WriteLine(windowWidth);

            RootGrid.Width = windowWidth < 800 ?
                windowWidth - 64 : 800 - 64;

            RootGrid.Height = windowHeight < 620 ?
                windowHeight - 64 : 620 - 64;

            double gridWidth = ItemGrid.DesiredSize.Width;
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
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            Library.IsChecked = false;
            Playback.IsChecked = false;
            Appearance.IsChecked = false;
            About.IsChecked = false;

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
        {
            await Methods.LaunchURI(URLs.Feedback);
        }
    }
}
