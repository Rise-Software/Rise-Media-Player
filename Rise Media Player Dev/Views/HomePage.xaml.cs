using RMP.App.Common;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace RMP.App.Views
{
    public sealed partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        private async void FeedbackButton_Click(object sender, RoutedEventArgs e)
        {
            _ = await Methods.LaunchURI(URLs.Feedback);
        }

        private async void ChangelogButton_Click(object sender, RoutedEventArgs e)
        {
            _ = await Methods.LaunchURI(URLs.Changes);
        }

        private async void InsiderButton_Click(object sender, RoutedEventArgs e)
        {
            _ = await Methods.LaunchURI(URLs.Insider);
        }

        private async void ContributeButton_Click(object sender, RoutedEventArgs e)
        {
            _ = await Methods.LaunchURI(URLs.GitHub);
        }
    }
}