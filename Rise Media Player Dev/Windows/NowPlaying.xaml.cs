using Rise.App.ViewModels;
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

            //Player.SetMediaPlayer(ViewModel.Player);

            DataContext = ViewModel;
            _ = PlayFrame.Navigate(typeof(CurrentlyPlayingPage));
        }

        private void Page_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (IsInCurrentlyPlayingPage)
            {
                PlayingAnimationIn.Begin();
                PlayFrame.Visibility = Visibility.Visible;
                Player.Visibility = Visibility.Visible;
                if (Application.Current.RequestedTheme == ApplicationTheme.Dark)
                {
                    ImageBrushAlbumCover.Opacity = 0.25;
                }
                else
                {
                    ImageBrushAlbumCover.Opacity = 0.5;
                }

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
        }

        private void PlayFrame_Navigated(object sender, NavigationEventArgs e)
        {
            IsInCurrentlyPlayingPage = !IsInCurrentlyPlayingPage;
            BackForPlay.Visibility = IsInCurrentlyPlayingPage ? Visibility.Collapsed : Visibility.Visible;
        }

        private void Page_PointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            MainPage.Current.AppTitleBar.Visibility = Visibility.Collapsed;
        }
    }
}
