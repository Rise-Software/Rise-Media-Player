using RMP.App.Common;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace RMP.App.Views
{
    public sealed partial class HomePage : Page
    {
        private readonly NavigationHelper navigationHelper;
        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return navigationHelper; }
        }

        public HomePage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;

            navigationHelper = new NavigationHelper(this);
        }

        private async void FeedbackButton_Click(object sender, RoutedEventArgs e)
            => _ = await URLs.Feedback.LaunchAsync();

        private async void ChangelogButton_Click(object sender, RoutedEventArgs e)
            => _ = await URLs.Changes.LaunchAsync();

        private async void InsiderButton_Click(object sender, RoutedEventArgs e)
            => _ = await URLs.Insider.LaunchAsync();

        private async void ContributeButton_Click(object sender, RoutedEventArgs e)
            => _ = await URLs.GitHub.LaunchAsync();

        #region NavigationHelper registration
        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
            => navigationHelper.OnNavigatedTo(e);

        protected override void OnNavigatedFrom(NavigationEventArgs e)
            => navigationHelper.OnNavigatedFrom(e);
        #endregion
    }
}