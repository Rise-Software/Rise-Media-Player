using System.Linq;
using Windows.UI.Xaml.Controls;
using Fluent_Media_Player_Dev.Settings;
using NavigationViewItem = Microsoft.UI.Xaml.Controls.NavigationViewItem;
using NavigationViewItemBase = Microsoft.UI.Xaml.Controls.NavigationViewItemBase;
using Windows.UI.Xaml;
using System.Collections.ObjectModel;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Fluent_Media_Player_Dev.Dialogs
{
    public sealed partial class SettingsDialog : ContentDialog
    {
        #region Variables
        private ObservableCollection<string> Breadcrumbs =
            new ObservableCollection<string>();
        #endregion
        public SettingsDialog()
        {
            this.InitializeComponent();
            SettingsNav.SelectedItem = SettingsNav.MenuItems[0];
            SettingsFrame.Navigate(typeof(AppearancePage));
            FinishNavigation();
            ContentDialog_SizeChanged(null, null);
        }

        #region Navigation
        private void SettingsNav_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            if (args.InvokedItemContainer.Content.ToString() == Breadcrumbs.Last())
            {
                FinishNavigation();
                return;
            }

            string navTo = args.InvokedItemContainer.Tag.ToString();
            if (navTo != null)
            {
                switch (navTo)
                {
                    case "AppearancePage":
                        SettingsFrame.Navigate(typeof(AppearancePage));
                        break;

                    case "MediaLibraryPage":
                        SettingsFrame.Navigate(typeof(MediaLibraryPage));
                        break;

                    case "PlaybackPage":
                        SettingsFrame.Navigate(typeof(PlaybackPage));
                        break;

                    case "NavigationPage":
                        SettingsFrame.Navigate(typeof(NavigationPage));
                        break;

                    case "ExperimentalPage":
                        SettingsFrame.Navigate(typeof(ExperimentalPage));
                        break;

                    case "AboutPage":
                        SettingsFrame.Navigate(typeof(AboutPage));
                        break;

                    default:
                        break;
                }
            }

            FinishNavigation();
        }

        private void FinishNavigation()
        {
            string type = SettingsFrame.CurrentSourcePageType.ToString();
            string tag = type.Split('.').Last();

            foreach (NavigationViewItemBase item in SettingsNav.MenuItems)
            {
                if (item is NavigationViewItem && item.Tag.ToString() == tag)
                {
                    SettingsNav.SelectedItem = item;
                    Breadcrumbs.Clear();
                    Breadcrumbs.Add(item.Content.ToString());
                    break;
                }
            }

            foreach (NavigationViewItemBase item in SettingsNav.FooterMenuItems)
            {
                if (item is NavigationViewItem && item.Tag.ToString() == tag)
                {
                    SettingsNav.SelectedItem = item;
                    Breadcrumbs.Clear();
                    Breadcrumbs.Add(item.Content.ToString());
                    break;
                }
            }
        }
        #endregion

        private void CloseButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Hide();
        }

        private void ContentDialog_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            if (Window.Current.Bounds.Width <= 600)
            {
                SettingsNav.PaneDisplayMode = Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.LeftCompact;
                SettingsFrame.Width = double.NaN;
                DualTone.Width = 96;
            }
            else if (Window.Current.Bounds.Width <= 800)
            {
                SettingsNav.PaneDisplayMode = Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.Left;
                SettingsFrame.Width = double.NaN;
                DualTone.Width = 248;
            }
            else
            {
                SettingsNav.PaneDisplayMode = Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.Left;
                SettingsFrame.Width = 500;
                DualTone.Width = 248;
            }

            if (Window.Current.Bounds.Height <= 670)
            {
                SettingsGrid.Height = Window.Current.Bounds.Height - 100;
            }
            else
            {
                SettingsGrid.Height = 530;
            }
        }
    }
}
