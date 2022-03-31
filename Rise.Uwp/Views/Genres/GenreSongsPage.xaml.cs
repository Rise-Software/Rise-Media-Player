using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Rise.App.ViewModels;
using Rise.Common.Extensions;
using Rise.Common.Helpers;
using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Views
{
    public sealed partial class GenreSongsPage : Page
    {
        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        private readonly NavigationHelper _navigationHelper;

        public GenreSongsPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;

            DataContext = this;
            _navigationHelper = new NavigationHelper(this);
            _navigationHelper.LoadState += NavigationHelper_LoadState;
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            if (e.NavigationParameter is Guid id)
            {
                this.SelectedGenre = this.MViewModel.Genres.
                    FirstOrDefault(g => g.Model.Id == id);

                this.Songs.Filter = s => ((SongViewModel)s).Genres.Contains(SelectedGenre.Name);
                this.Albums.Filter = a => ((AlbumViewModel)a).Genres.Contains(SelectedGenre.Name);
            }
            else if (e.NavigationParameter is string str)
            {
                this.SelectedGenre = this.MViewModel.Genres.
                    FirstOrDefault(g => g.Name == str);

                this.Songs.Filter = s => ((SongViewModel)s).Genres.Contains(str);
                this.Albums.Filter = a => ((AlbumViewModel)a).Genres.Contains(str);
            }

            Songs.SortDescriptions.Clear();
            Songs.SortDescriptions.Add(new SortDescription("Title", SortDirection.Ascending));

            Albums.SortDescriptions.Clear();
            Albums.SortDescriptions.Add(new SortDescription("Title", SortDirection.Ascending));

            foreach (ArtistViewModel artist in Artists)
            {
                AllArtistsInGenre.AddIfNotExists(artist);
            }

            foreach (AlbumViewModel album in Albums)
            {
                AllArtistsInGenre.Filter = a => album.Artist == ((ArtistViewModel)a).Name;
            }
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            this.Frame.SetListDataItemForNextConnectedAnimation(this.SelectedGenre);
        }

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
            => _navigationHelper.OnNavigatedTo(e);

        protected override void OnNavigatedFrom(NavigationEventArgs e)
            => _navigationHelper.OnNavigatedFrom(e);
        #endregion
    }

    public sealed partial class GenreSongsPage : Page
    {
        /// <summary>
        /// Gets the app-wide MViewModel instance.
        /// </summary>
        public MainViewModel MViewModel => App.MViewModel;

        /// <summary>
        /// Gets the app-wide PViewModel instance.
        /// </summary>
        private PlaybackViewModel PViewModel => App.PViewModel;

        private static readonly DependencyProperty SelectedGenreProperty =
            DependencyProperty.Register("SelectedGenre", typeof(GenreViewModel), typeof(GenreSongsPage), null);

        private GenreViewModel SelectedGenre
        {
            get => (GenreViewModel)GetValue(SelectedGenreProperty);
            set => SetValue(SelectedGenreProperty, value);
        }

        private SongViewModel _song;
        public SongViewModel SelectedSong
        {
            get => MViewModel.SelectedSong;
            set => MViewModel.SelectedSong = value;
        }

        private AdvancedCollectionView Songs => this.MViewModel.FilteredSongs;
        private AdvancedCollectionView Albums => this.MViewModel.FilteredAlbums;
        private AdvancedCollectionView Artists => this.MViewModel.FilteredArtists;

        private readonly AdvancedCollectionView AllArtistsInGenre = new();
        private readonly AdvancedCollectionView AllAlbumsInGenre = new();

        private string SortProperty = "Title";
        private SortDirection CurrentSort = SortDirection.Ascending;

        private RelayCommand _sortCommand;
        public RelayCommand SortCommand
        {
            get
            {
                if (_sortCommand == null)
                {
                    _sortCommand = new RelayCommand(SortItems);
                }

                return _sortCommand;
            }
            set => _sortCommand = value;
        }

        private RelayCommand _viewCommand;
        public RelayCommand ViewCommand
        {
            get
            {
                if (_viewCommand == null)
                {
                    _viewCommand = new RelayCommand(ChangeView);
                }

                return _viewCommand;
            }
            set => _viewCommand = value;
        }
    }
    
    // Event handlers
    public sealed partial class GenreSongsPage : Page
    {
        private async void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is SongViewModel song)
            {
                int index = this.MainList.Items.IndexOf(song);
                await EventsLogic.StartMusicPlaybackAsync(index);
                return;
            }

            await EventsLogic.StartMusicPlaybackAsync();
        }

        private async void ShuffleButton_Click(object sender, RoutedEventArgs e)
            => await EventsLogic.StartMusicPlaybackAsync(0, true);

        private void Grid_PointerEntered(object sender, PointerRoutedEventArgs e)
            => EventsLogic.FocusSong(ref _song, e);

        private void Grid_PointerExited(object sender, PointerRoutedEventArgs e)
            => EventsLogic.UnfocusSong(ref _song, e);

        private void Album_Click(Hyperlink sender, HyperlinkClickEventArgs args)
            => EventsLogic.GoToAlbum(sender);

        private void Artist_Click(Hyperlink sender, HyperlinkClickEventArgs args)
            => EventsLogic.GoToArtist(sender);

        private void AlbumGrid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is AlbumViewModel album)
            {
                this.AlbumFlyout.ShowAt(AlbumGrid, e.GetPosition(AlbumGrid));
            }
        }

        private void ArtistGrid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is ArtistViewModel artist)
            {
                this.ArtistFlyout.ShowAt(ArtistGrid, e.GetPosition(ArtistGrid));
            }
        }

        private async void PropsHover_Click(object sender, RoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is SongViewModel song)
            {
                SelectedSong = song;
                await SelectedSong.StartEditAsync();
            }
        }

        private void NavigationView_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            switch (args.InvokedItem)
            {
                case "Songs":
                    this.MainList.Visibility = Visibility.Visible;
                    this.AlbumGrid.Visibility = Visibility.Collapsed;
                    this.ArtistGrid.Visibility = Visibility.Collapsed;
                    break;

                case "Albums":
                    this.MainList.Visibility = Visibility.Collapsed;
                    this.AlbumGrid.Visibility = Visibility.Visible;
                    this.ArtistGrid.Visibility = Visibility.Collapsed;
                    break;

                case "Artists":
                    this.MainList.Visibility = Visibility.Collapsed;
                    this.AlbumGrid.Visibility = Visibility.Collapsed;
                    this.ArtistGrid.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void ArtistGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if ((e?.OriginalSource as FrameworkElement).DataContext is ArtistViewModel artist)
            {
                this.Frame.Navigate(typeof(ArtistSongsPage), artist.Model.Id);
            }
        }

        private void AlbumGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if ((e?.OriginalSource as FrameworkElement).DataContext is AlbumViewModel album)
            {
                this.Frame.Navigate(typeof(AlbumSongsPage), album.Model.Id);
            }
        }

        private async void MainList_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is SongViewModel song)
            {
                int index = this.MainList.Items.IndexOf(song);
                await EventsLogic.StartMusicPlaybackAsync(index);
            }
        }

        private void MainList_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is SongViewModel song)
            {
                this.SelectedSong = song;
                this.SongFlyout.ShowAt(MainList, e.GetPosition(MainList));
            }
        }

        private async void Props_Click(object sender, RoutedEventArgs e)
            => await SelectedSong.StartEditAsync();

        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            await SelectedSong.StartEditAsync();
            SelectedSong = null;
        }

        private void ChangeView_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem item = sender as MenuFlyoutItem;
            string tag = item.Tag.ToString();

            switch (tag)
            {
                default:
                    break;
            }
        }

        private void SortItems(object param)
        {
            Songs.SortDescriptions.Clear();
            string by = param.ToString();

            switch (by)
            {
                case "Ascending":
                    CurrentSort = SortDirection.Ascending;
                    break;

                case "Descending":
                    CurrentSort = SortDirection.Descending;
                    break;

                case "Track":
                    Songs.SortDescriptions.
                        Add(new SortDescription("Disc", CurrentSort));
                    SortProperty = by;
                    break;

                default:
                    SortProperty = by;
                    break;
            }

            Songs.SortDescriptions.
                Add(new SortDescription(SortProperty, CurrentSort));
        }

        private void ChangeView(object param)
        {
            string view = param.ToString();
            switch (view)
            {
                case "Default":
                    CurrentSort = SortDirection.Ascending;
                    break;

                case "Descending":
                    CurrentSort = SortDirection.Descending;
                    break;

                default:
                    SortProperty = view;
                    break;
            }
        }
    }

    [ContentProperty(Name = "GenreTemplate")]
    public class GenreContentTemplateSelector : DataTemplateSelector
    {
        public DataTemplate GenreTemplate { get; set; }
        public DataTemplate AlbumTemplate { get; set; }

        public DataTemplate ArtistTemplate { get; set; }
        // public DataTemplate SeparatorTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            return GenreTemplate;
        }
    }
}
