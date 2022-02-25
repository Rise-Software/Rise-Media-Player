using Rise.App.ViewModels;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Views
{
    public sealed partial class NowPlaying : Page
    {
        /// <summary>
        /// Gets the app-wide NPViewModel instance.
        /// </summary>
        private PlaybackViewModel ViewModel => App.PViewModel;
        private bool IsInCurrentlyPlayingPage = false;

        


        public NowPlaying()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
            MainPage.Current.AppTitleBar.Visibility = Visibility.Collapsed;
            _ = new ApplicationTitleBar(TitleBar);

            //Player.SetMediaPlayer(ViewModel.Player);
            ApplicationView.GetForCurrentView().TitleBar.ButtonBackgroundColor = Colors.Transparent;
            ApplicationView.GetForCurrentView().TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            DataContext = ViewModel;
            _ = PlayFrame.Navigate(typeof(CurrentlyPlayingPage));
            //int testvar = 3;
            //while (testvar==3)
            //{
            //    MainPage.Current.AppTitleBar.Visibility = Visibility.Collapsed;
            //}
        }

        private void Page_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (IsInCurrentlyPlayingPage)
            {
                PlayingAnimationIn.Begin();
                PlayFrame.Visibility = Visibility.Visible;
                Player.Visibility = Visibility.Visible;
                ImageBrushAlbumCover.Opacity = 0.5;
                BlurBrush.Amount = 10;
            }
            MainPage.Current.AppTitleBar.Visibility = Visibility.Collapsed;
        }

        private void Page_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (IsInCurrentlyPlayingPage)
            {
                PlayingAnimationOut.Begin();
                PlayFrame.Visibility = Visibility.Collapsed;
                Player.Visibility = Visibility.Collapsed;
                ImageBrushAlbumCover.Opacity = 1;
                BlurBrush.Amount = 0;
            }
            MainPage.Current.AppTitleBar.Visibility = Visibility.Collapsed;
        }

        private void PlayFrame_Navigated(object sender, NavigationEventArgs e)
        {
            IsInCurrentlyPlayingPage = !IsInCurrentlyPlayingPage;
            BackForPlay.Visibility = IsInCurrentlyPlayingPage ? Visibility.Collapsed : Visibility.Visible;
            MainPage.Current.AppTitleBar.Visibility = Visibility.Collapsed;
        }

        private void Page_PointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            MainPage.Current.AppTitleBar.Visibility = Visibility.Collapsed;
        }
    }
}
