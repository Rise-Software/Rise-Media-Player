using RMP.App.Common;
using RMP.App.Dialogs;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace RMP.App.Settings
{
    public sealed partial class AboutPage : Page
    {
        public AboutPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        private async void NavigationExpander_Click(object sender, RoutedEventArgs e)
            => await Methods.LaunchURIAsync(URLs.License);

        private void CommandBarButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            switch (button.Tag.ToString())
            {
                case "Insider":
                    Frame.Navigate(typeof(InsiderPage));
                    SettingsDialog.Current.Breadcrumbs.Add
                        (ResourceLoaders.SidebarLoader.GetString("Ins"));
                    break;

                case "Version":

                    break;
            }


        }
    }
}
