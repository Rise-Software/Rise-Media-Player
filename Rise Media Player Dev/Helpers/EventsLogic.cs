using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.UI;
using Rise.App.ViewModels;
using Rise.Common.Extensions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;

namespace Rise.App.Views
{
    /// <summary>
    /// This contains common logic for event handlers that can
    /// be shared between pages/controls.
    /// </summary>
    public static partial class EventsLogic
    {
        private static AdvancedCollectionView Songs => App.MViewModel.FilteredSongs;
        private static AdvancedCollectionView Videos => App.MViewModel.FilteredVideos;

        /// <inheritdoc cref="MainViewModel.SelectedSong"/>
        private static SongViewModel SelectedSong
        {
            get => App.MViewModel.SelectedSong;
            set => App.MViewModel.SelectedSong = value;
        }

        /// <inheritdoc cref="MainViewModel.SelectedVideo"/>
        private static VideoViewModel SelectedVideo
        {
            get => App.MViewModel.SelectedVideo;
            set => App.MViewModel.SelectedVideo = value;
        }
    }

    // Playback related
    public static partial class EventsLogic
    {
        public static async Task StartMusicPlaybackAsync(int index = 0, bool shuffle = false)
        {
            try
            {
                App.PViewModel.PlaybackList.ShuffleEnabled = shuffle;

                if (SelectedSong != null && index == 0)
                {
                    index = Songs.IndexOf(SelectedSong);
                    SelectedSong = null;
                }

                var songs = Songs.CloneList<object, SongViewModel>();

                if (shuffle)
                {
                    Random rnd = new();
                    index = rnd.Next(0, Songs.Count);
                }

                await App.PViewModel.StartMusicPlaybackAsync(songs.GetEnumerator(), index, songs.Count, shuffle);
            }
            catch
            {

            }
        }

        public static async Task StartVideoPlaybackAsync(int index = 0, bool shuffle = false)
        {
            try
            {
                App.PViewModel.PlaybackList.ShuffleEnabled = shuffle;

                if (SelectedVideo != null && index == 0)
                {
                    index = Videos.IndexOf(SelectedVideo);
                    SelectedVideo = null;
                }

                var videos = Videos.CloneList<object, VideoViewModel>();

                if (shuffle)
                {
                    Random rnd = new();
                    index = rnd.Next(0, Videos.Count);
                }

                await App.PViewModel.StartVideoPlaybackAsync(videos.GetEnumerator(), index, videos.Count, shuffle);
            }
            catch
            {

            }
        }
    }

    // Focus/unfocus
    public static partial class EventsLogic
    {
        public static void FocusSong(ref SongViewModel song, PointerRoutedEventArgs e)
        {
            if (song == null)
            {
                if ((e.OriginalSource as FrameworkElement).DataContext is SongViewModel s)
                {
                    song = s;
                }
            }

            song.IsFocused = true;
        }

        public static void UnfocusSong(ref SongViewModel song, PointerRoutedEventArgs e)
        {
            if (song != null)
            {
                song.IsFocused = false;
                song = null;
            }
        }
    }

    // Hyperlink handling
    public static partial class EventsLogic
    {
        public static void GoToAlbum(Hyperlink sender)
        {
            var run = sender.Inlines.FirstOrDefault() as Run;

            _ = MainPage.Current.ContentFrame.
                    Navigate(typeof(AlbumSongsPage), run.Text);
        }

        public static void GoToArtist(Hyperlink sender)
        {
            var run = sender.Inlines.FirstOrDefault() as Run;

            _ = MainPage.Current.ContentFrame.
                    Navigate(typeof(ArtistSongsPage), run.Text);
        }
    }
}
