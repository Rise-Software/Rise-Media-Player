using Rise.App.Common;
using Rise.App.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Views
{
    public sealed partial class VideoPlaybackPage : Page
    {
        private readonly NavigationHelper navigationHelper;
        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper => navigationHelper;

        private PlaybackViewModel ViewModel => App.PViewModel;

        public VideoPlaybackPage()
        {
            InitializeComponent();
            PlayerElement.SetMediaPlayer(ViewModel.Player);

            NavigationCacheMode = NavigationCacheMode.Required;
            navigationHelper = new NavigationHelper(this);

            Loaded += (s, e) => _ = new ApplicationTitleBar(AppTitleBar);
            _ = new ApplicationTitleBar(AppTitleBar);
        }

        private void Page_PointerExited(object sender, PointerRoutedEventArgs e)
            => TopGrid.Visibility = Visibility.Collapsed;

        private void Page_PointerEntered(object sender, PointerRoutedEventArgs e)
            => TopGrid.Visibility = Visibility.Visible;
    }
}
