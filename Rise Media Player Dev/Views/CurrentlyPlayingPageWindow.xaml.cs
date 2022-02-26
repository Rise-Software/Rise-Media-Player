using Rise.App.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
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

        public CurrentlyPlayingPageWindow()
        {
            this.InitializeComponent();
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
