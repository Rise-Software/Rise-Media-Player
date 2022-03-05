using Rise.App.Common;
using Rise.App.ViewModels;
using Windows.UI;
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
    public sealed partial class FullNowPlayingPage : Page
    {
        private PlaybackViewModel ViewModel => App.PViewModel;
        private readonly NavigationHelper _navigationHelper;
        public static FullNowPlayingPage Current;
        private bool IsInCurrentlyPlayingPage = false;


        public FullNowPlayingPage()
        {
            this.InitializeComponent();

            NavigationCacheMode = NavigationCacheMode.Required;
            _navigationHelper = new NavigationHelper(this);
            Current = this;
            MainPage.Current.AppTitleBar.Visibility = Visibility.Collapsed;
            _ = new ApplicationTitleBar(TitleBar);
            MainPage.Current.AppTitleBar.Visibility = Visibility.Collapsed;
            MainPage.Current.AppTitleBar.Visibility = Visibility.Collapsed;
            PlayingAnimationIn.Begin();
            MainPage.Current.AppTitleBar.Visibility = Visibility.Collapsed;
            PlayFrame.Visibility = Visibility.Visible;
            MainPage.Current.AppTitleBar.Visibility = Visibility.Collapsed;
            Player.Visibility = Visibility.Visible;
            MainPage.Current.AppTitleBar.Visibility = Visibility.Collapsed;

            BlurBrush.Amount = 15;
            MainPage.Current.AppTitleBar.Visibility = Visibility.Collapsed;

            //Player.SetMediaPlayer(ViewModel.Player);
            ApplicationView.GetForCurrentView().TitleBar.ButtonBackgroundColor = Colors.Transparent;
            ApplicationView.GetForCurrentView().TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            DataContext = ViewModel;
            _ = PlayFrame.Navigate(typeof(CurrentlyPlayingPageWindow));
            //int testvar = 3;
            //while (testvar==3)
            //{
            //    MainPage.Current.AppTitleBar.Visibility = Visibility.Collapsed;
            //}
        }

        private void FullNowPlayingPage_Loaded(object sender, RoutedEventArgs e)
        {
            _ = new ApplicationTitleBar(TitleBar);
        }

        private void Page_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            _ = new ApplicationTitleBar(TitleBar);
            MainPage.Current.AppTitleBar.Visibility = Visibility.Collapsed;
            if (IsInCurrentlyPlayingPage)
            {
                MainPage.Current.AppTitleBar.Visibility = Visibility.Collapsed;
                PlayingAnimationIn.Begin();
                MainPage.Current.AppTitleBar.Visibility = Visibility.Collapsed;
                PlayFrame.Visibility = Visibility.Visible;
                MainPage.Current.AppTitleBar.Visibility = Visibility.Collapsed;
                Player.Visibility = Visibility.Visible;
                MainPage.Current.AppTitleBar.Visibility = Visibility.Collapsed;

                BlurBrush.Amount = 15;
                MainPage.Current.AppTitleBar.Visibility = Visibility.Collapsed;
            }
        }

        private async void Page_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            MainPage.Current.AppTitleBar.Visibility = Visibility.Collapsed;
            if (IsInCurrentlyPlayingPage)
            {
                MainPage.Current.AppTitleBar.Visibility = Visibility.Collapsed;
                PlayingAnimationOut.Begin();
                PlayFrame.Visibility = Visibility.Collapsed;
                Player.Visibility = Visibility.Collapsed;
                BlurBrush.Amount = 0;
                MainPage.Current.AppTitleBar.Visibility = Visibility.Collapsed;
            }
        }

        private void PlayFrame_Navigated(object sender, NavigationEventArgs e)
        {
            MainPage.Current.AppTitleBar.Visibility = Visibility.Collapsed;
            IsInCurrentlyPlayingPage = !IsInCurrentlyPlayingPage;
            BackForPlay.Visibility = IsInCurrentlyPlayingPage ? Visibility.Collapsed : Visibility.Visible;
            MainPage.Current.AppTitleBar.Visibility = Visibility.Collapsed;
        }

        private void Page_PointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            MainPage.Current.AppTitleBar.Visibility = Visibility.Collapsed;
        }

        private async void PopOutWindow_Click(object sender, RoutedEventArgs e)
        {

        }

        private void FullNPGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width >= 900)
            {
                CurrentlyPlayingPage.Current.ArtInfo.Visibility = Visibility.Visible;
                ArtInfo.Visibility = Visibility.Visible;
            }
            else if (e.NewSize.Width >= 600)
            {
                CurrentlyPlayingPage.Current.ArtInfo.Visibility = Visibility.Visible;
                ArtInfo.Visibility = Visibility.Visible;
            }
            else if (e.NewSize.Width >= 480)
            {
                CurrentlyPlayingPage.Current.ArtInfo.Visibility = Visibility.Collapsed;
                ArtInfo.Visibility = Visibility.Collapsed;
            }
            else if (e.NewSize.Width >= 400)
            {
                CurrentlyPlayingPage.Current.ArtInfo.Visibility = Visibility.Collapsed;
                ArtInfo.Visibility = Visibility.Collapsed;
            }
            else
            {
                CurrentlyPlayingPage.Current.ArtInfo.Visibility = Visibility.Collapsed;
                ArtInfo.Visibility = Visibility.Collapsed;
            }
        }

    }
}
