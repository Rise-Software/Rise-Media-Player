using Rise.App.ViewModels;
using System;
using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Rise.App.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CurrentlyPlayingPageWindow : Page
    {
        private PlaybackViewModel ViewModel => App.PViewModel;
        public static CurrentlyPlayingPageWindow Current;

        public CurrentlyPlayingPageWindow()
        {
            this.InitializeComponent();
            Current = this;
            Loaded += CurrentlyPlayingPage_Loaded;
            NavigationCacheMode = NavigationCacheMode.Enabled;

            DataContext = ViewModel;



        }

        private void CurrentlyPlayingPage_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Queue_Click(object sender, RoutedEventArgs e)
            => _ = Frame.Navigate(typeof(QueuePage));

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            MainPage.Current.AppTitleBar.Visibility = Visibility.Visible;
            var preferences = ViewModePreferences.CreateDefault(ApplicationViewMode.CompactOverlay);
            preferences.CustomSize = new Size(600, 700);
            _ = await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.Default, preferences);
            MainPage.Current.AppTitleBar.Visibility = Visibility.Collapsed;
        }
    }
}
