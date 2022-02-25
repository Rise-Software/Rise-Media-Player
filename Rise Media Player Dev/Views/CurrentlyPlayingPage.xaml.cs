using Rise.App.ViewModels;
using System;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
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