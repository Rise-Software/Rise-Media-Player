using Rise.App.Common;
using Rise.App.ViewModels;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Views
{
    public sealed partial class SearchResultsPage : Page
    {
        private string _searchText = string.Empty;
        private readonly List<ArtistViewModel> suitableArtists = new();
        private readonly List<SongViewModel> suitableSongs = new();
        private readonly List<AlbumViewModel> suitableAlbums = new();
        private string[] splitText;
        private bool songFound, albumFound, artistFound;

        private async void SongsGrid_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is SongViewModel song)
            {
                await EventsLogic.StartMusicPlaybackAsync(App.MViewModel.Songs.IndexOf(song), false);
            }
        }

        private void ArtistsGrid_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is ArtistViewModel artist)
            {
                _ = MainPage.Current.ContentFrame.Navigate(typeof(ArtistSongsPage), artist);
            }
        }

        private void AlbumsGrid_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is AlbumViewModel album)
            {
                _ = MainPage.Current.ContentFrame.Navigate(typeof(AlbumSongsPage), album.Model.Id);
            }
        }

        public SearchResultsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _searchText = e.Parameter as string;
            splitText = _searchText.ToLower().Split(" ");

            foreach (AlbumViewModel album in App.MViewModel.FilteredAlbums)
            {
                albumFound = splitText.All((key) =>
                {
                    return album.Title.ToLower().Contains(key);
                });

                if (albumFound)
                {
                    suitableAlbums.Add(album);
                }
            }

            foreach (SongViewModel song in App.MViewModel.FilteredSongs)
            {
                songFound = splitText.All((key) =>
                {
                    return song.Title.ToLower().Contains(key);
                });

                if (songFound)
                {
                    suitableSongs.Add(song);
                }
            }

            foreach (ArtistViewModel artist in App.MViewModel.FilteredArtists)
            {
                artistFound = splitText.All((key) =>
                {
                    return artist.Name.ToLower().Contains(key);
                });

                if (artistFound)
                {
                    suitableArtists.Add(artist);
                }
            }

            if (suitableSongs.Count == 0)
            {
                songFound = false;
            }

            if (suitableArtists.Count == 0)
            {
                artistFound = false;
            }

            if (suitableAlbums.Count == 0)
            {
                albumFound = false;
            }

            if (suitableSongs.Count > 0)
            {
                songFound = true;
            }

            if (suitableArtists.Count > 0)
            {
                artistFound = true;
            }

            if (suitableAlbums.Count > 0)
            {
                albumFound = true;
            }
        }
    }
}
