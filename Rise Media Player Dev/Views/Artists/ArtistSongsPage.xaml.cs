﻿using Microsoft.Toolkit.Uwp.UI;
using Rise.App.Common;
using Rise.App.ViewModels;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Views
{
    public sealed partial class ArtistSongsPage : Page
    {
        #region Variables
        /// <summary>
        /// Gets the app-wide MViewModel instance.
        /// </summary>
        public MainViewModel MViewModel => App.MViewModel;

        /// <summary>
        /// Gets the app-wide PViewModel instance.
        /// </summary>
        private PlaybackViewModel PViewModel => App.PViewModel;

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        private readonly NavigationHelper _navigationHelper;

        private static readonly DependencyProperty SelectedArtistProperty =
            DependencyProperty.Register("SelectedArtist", typeof(ArtistViewModel), typeof(ArtistSongsPage), null);

        private ArtistViewModel SelectedArtist
        {
            get => (ArtistViewModel)GetValue(SelectedArtistProperty);
            set => SetValue(SelectedArtistProperty, value);
        }

        private SongViewModel _song;
        public SongViewModel SelectedSong
        {
            get => MViewModel.SelectedSong;
            set => MViewModel.SelectedSong = value;
        }

        private AdvancedCollectionView Songs => MViewModel.FilteredSongs;
        private AdvancedCollectionView Albums => MViewModel.FilteredAlbums;

        private string SortProperty = "Title";
        private SortDirection CurrentSort = SortDirection.Ascending;
        #endregion

        public ArtistSongsPage()
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

            if (e.NavigationParameter is AlbumViewModel album)
            {
                Songs.Filter = s => ((SongViewModel)s).Album == album.Title;

                // TODO: Get "more album from this artist" to work.
                Albums.Filter = a => ((AlbumViewModel)a).Artist == album.Artist;
                Albums.SortDescriptions.Clear();
                Albums.SortDescriptions.Add(new SortDescription("Year", SortDirection.Ascending));
            }
            else
            {

                // TODO: Get "more album from this artist" to work.
                Albums.SortDescriptions.Clear();
                Albums.SortDescriptions.Add(new SortDescription("Year", SortDirection.Ascending));
            }

            if (e.NavigationParameter is ArtistViewModel artist)
            {
                SelectedArtist = artist;
                Songs.Filter = s => ((SongViewModel)s).Artist == artist.Name;
                Albums.SortDescriptions.Clear();
                Albums.SortDescriptions.Add(new SortDescription("Year", SortDirection.Ascending));
            }
            else if (e.NavigationParameter is string str)
            {
                SelectedArtist = App.MViewModel.Artists.First(a => a.Name == str);
                Songs.Filter = s => ((SongViewModel)s).Artist == str
                    || ((SongViewModel)s).AlbumArtist == str;
            
            }

            

            Songs.SortDescriptions.Clear();
            Songs.SortDescriptions.Add(new SortDescription("Title", SortDirection.Ascending));
        }

        #region Event handlers
        private async void MainList_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is SongViewModel song)
            {
                int index = MainList.Items.IndexOf(song);
                await EventsLogic.StartMusicPlaybackAsync(index);
            }
        }

        private void AskDiscy_Click(object sender, RoutedEventArgs e)
        {
            DiscyOnSong.IsOpen = true;
        }

        private void MainList_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is SongViewModel song)
            {
                SelectedSong = song;
                SongFlyout.ShowAt(MainList, e.GetPosition(MainList));
            }
        }

        private async void Props_Click(object sender, RoutedEventArgs e)
            => await SelectedSong.StartEdit();

        private void ShowAlbum_Click(object sender, RoutedEventArgs e)
            => _ = Frame.Navigate(typeof(AlbumSongsPage), SelectedSong.Album);

        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            await SelectedSong.StartEdit();
            SelectedSong = null;
        }

        private void SortFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem item = sender as MenuFlyoutItem;
            Songs.SortDescriptions.Clear();

            string tag = item.Tag.ToString();
            switch (tag)
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
                    SortProperty = tag;
                    break;

                default:
                    SortProperty = tag;
                    break;
            }

            Songs.SortDescriptions.
                Add(new SortDescription(SortProperty, CurrentSort));
        }
        #endregion

        #region Common handlers
        private async void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is SongViewModel song)
            {
                int index = MainList.Items.IndexOf(song);
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

        private void MainList_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            // Check if the list view height is larger than this page's height, if this is true,
            // try to handle scrolling
            if (MainList.Height > Height)
            {
                (MainList.HeaderTemplate.GetChildren<Border>().First().Background as ImageBrush).Opacity = MainList.ActualOffset.Y;
            }
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
            => _navigationHelper.OnNavigatedTo(e);

        protected override void OnNavigatedFrom(NavigationEventArgs e)
            => _navigationHelper.OnNavigatedFrom(e);
        #endregion
    }
}
