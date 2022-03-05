using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Rise.App.Settings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppearanceBasePage : Page
    {
        public AppearanceBasePage()
        {
            this.InitializeComponent();
        }

        private void AppearanceNav_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            var selectedItem = (Microsoft.UI.Xaml.Controls.NavigationViewItem)args.SelectedItem;
            string selectedItemTag = selectedItem.Tag as string;

            switch (selectedItemTag)
            {
                case "Window":
                    AppearanceFrame.Navigate(typeof(AppearancePage));
                    break;
                default:
                    AppearanceFrame.Navigate(typeof(ComingSoonPage));
                    break;

            }
        }
    }
}
