using System.Linq;
using Windows.UI.Xaml.Controls;
using Fluent_Media_Player_Dev.Settings;
using NavigationViewItem = Microsoft.UI.Xaml.Controls.NavigationViewItem;
using NavigationViewItemBase = Microsoft.UI.Xaml.Controls.NavigationViewItemBase;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Fluent_Media_Player_Dev.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();
            SettingsNav.SelectedItem = SettingsNav.MenuItems[0];
            SettingsFrame.Navigate(typeof(FoldersPage));
        }

        #region Navigation
        private void SettingsNav_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            string navTo = args.InvokedItemContainer.Tag.ToString();
            if (navTo != null)
            {
                switch (navTo)
                {
                    case "FoldersPage":
                        SettingsFrame.Navigate(typeof(FoldersPage));
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
                    break;
                }
            }
        }
        #endregion
    }
}
