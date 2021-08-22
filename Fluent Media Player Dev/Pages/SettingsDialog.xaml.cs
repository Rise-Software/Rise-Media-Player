using System.Linq;
using Windows.UI.Xaml.Controls;
using Fluent_Media_Player_Dev.Settings;
using NavigationViewItem = Microsoft.UI.Xaml.Controls.NavigationViewItem;
using NavigationViewItemBase = Microsoft.UI.Xaml.Controls.NavigationViewItemBase;
using Windows.UI.Xaml;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Fluent_Media_Player_Dev.Pages
{
    public sealed partial class SettingsDialog : ContentDialog
    {
        public SettingsDialog()
        {
            this.InitializeComponent();
            SettingsNav.SelectedItem = SettingsNav.MenuItems[0];
            SettingsFrame.Navigate(typeof(AppearancePage));
            ContentDialog_SizeChanged(null, null);
        }

        #region Navigation
        private void SettingsNav_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            if (args.InvokedItemContainer.Content.ToString() == Header.Text)
            {
                FinishNavigation();
                return;
            }

            string navTo = args.InvokedItemContainer.Tag.ToString();
            if (navTo != null)
            {
                switch (navTo)
                {
                    case "MediaLibraryPage":
                        SettingsFrame.Navigate(typeof(MediaLibraryPage));
                        break;

                    case "AppearancePage":
                        SettingsFrame.Navigate(typeof(AppearancePage));
                        break;

                    case "SidebarPage":
                        SettingsFrame.Navigate(typeof(SidebarPage));
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
                    Header.Text = item.Content.ToString();
                    break;
                }
            }

            foreach (NavigationViewItemBase item in SettingsNav.FooterMenuItems)
            {
                if (item is NavigationViewItem && item.Tag.ToString() == tag)
                {
                    SettingsNav.SelectedItem = item;
                    Header.Text = item.Content.ToString();
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
            if (Window.Current.Bounds.Width <= 700)
            {
                SettingsNav.PaneDisplayMode = Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.LeftCompact;
                SettingsFrame.Width = 380;
            }
            else if (Window.Current.Bounds.Width <= 800)
            {
                SettingsNav.PaneDisplayMode = Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.LeftCompact;
                SettingsFrame.Width = 460;
            }
            else
            {
                SettingsNav.PaneDisplayMode = Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.Left;
                SettingsFrame.Width = 460;
            }

            if (Window.Current.Bounds.Height <= 600)
            {
                SettingsGrid.Height = Window.Current.Bounds.Height - 108;
            }
            else
            {
                SettingsGrid.Height = 460;
            }
        }
    }
}
