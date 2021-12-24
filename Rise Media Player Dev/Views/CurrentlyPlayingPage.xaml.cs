using Rise.App.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Views
{
    public sealed partial class CurrentlyPlayingPage : Page
    {
        /// <summary>
        /// Gets the app-wide NPViewModel instance.
        /// </summary>
        private PlaybackViewModel ViewModel => App.PViewModel;

        public CurrentlyPlayingPage()
        {
            InitializeComponent();
            Loaded += CurrentlyPlayingPage_Loaded;
            NavigationCacheMode = NavigationCacheMode.Enabled;

            DataContext = ViewModel;
        }

        private void CurrentlyPlayingPage_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Queue_Click(object sender, RoutedEventArgs e)
        {
            _ = Frame.Navigate(typeof(QueuePage));
        }
    }
}
