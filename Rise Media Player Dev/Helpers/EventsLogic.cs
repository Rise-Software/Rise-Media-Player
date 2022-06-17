using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.UI;
using Rise.App.ViewModels;
using Rise.Common.Extensions;
using Rise.Data.ViewModels;
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
        private static MediaPlaybackViewModel MPViewModel => App.MPViewModel;

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
        /// <summary>
        /// Starts playback for the current list of filtered songs.
        /// </summary>
        /// <param name="index">The position of the song in
        /// <see cref="Songs"/> that should be played first.</param>
        /// <param name="shuffle">Whether shuffling should be enabled.
        /// This value is ignored if shuffling was enabled beforehand,
        /// as indicated by the value of
        /// <see cref="MediaPlaybackViewModel.ShuffleEnabled"/>.</param>
        public static async Task StartMusicPlaybackAsync(int index = 0, bool shuffle = false)
        {
            try
            {
                var songs = Songs.CloneList<object, SongViewModel>();
                if (SelectedSong != null && index == 0)
                {
                    index = Songs.IndexOf(SelectedSong);
                    SelectedSong = null;
                }
                songs.MoveRangeToEnd(0, index - 1);

                if (!MPViewModel.ShuffleEnabled)
                    MPViewModel.ShuffleEnabled = shuffle;

                await MPViewModel.PlayItemsAsync(songs);
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
}
