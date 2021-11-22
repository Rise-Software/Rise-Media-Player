using Microsoft.Toolkit.Uwp.UI;
using Rise.App.Common;
using Rise.App.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Views
{
    public sealed partial class ArtistsPage : Page
    {
        /// <summary>
        /// Gets the app-wide MViewModel instance.
        /// </summary>
        private MainViewModel MViewModel => App.MViewModel;

        private AdvancedCollectionView Artists => MViewModel.FilteredArtists;

        private static readonly DependencyProperty SelectedArtistProperty =
            DependencyProperty.Register("SelectedArtist", typeof(ArtistViewModel), typeof(ArtistsPage), null);

        private ArtistViewModel SelectedArtist
        {
            get => (ArtistViewModel)GetValue(SelectedArtistProperty);
            set => SetValue(SelectedArtistProperty, value);
        }

        private readonly NavigationHelper navigationHelper;
        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper => navigationHelper;

        public ArtistsPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;

            navigationHelper = new NavigationHelper(this);
        }

        #region Event handlers
        private void GridView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is ArtistViewModel artist)
            {
                _ = Frame.Navigate(typeof(ArtistSongsPage), artist);
            }

            SelectedArtist = null;
        }
        #endregion

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
