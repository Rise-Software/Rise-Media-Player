using Rise.App.ViewModels;
using System;
using System.Numerics;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Views
{
    public sealed partial class NowPlaying : Page
    {
        /// <summary>
        /// Gets the app-wide NPViewModel instance.
        /// </summary>
        private PlaybackViewModel ViewModel => App.PViewModel;

        private bool IsInCurrentlyPlayingPage = true;

        public NowPlaying()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;

            Player.SetMediaPlayer(ViewModel.Player);
            ApplicationView.GetForCurrentView().TitleBar.ButtonBackgroundColor = Colors.Transparent;
            ApplicationView.GetForCurrentView().TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            DataContext = ViewModel;
            _ = PlayFrame.Navigate(typeof(CurrentlyPlayingPage));
        }
    }
}
