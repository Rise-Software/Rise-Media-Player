using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using static RMP.App.Common.Enums;

namespace RMP.App.ViewModels
{
    public class PlaybackViewModel : BaseViewModel
    {
        /// <summary>
        /// Creates a new NowPlayingViewModel.
        /// </summary>
        public PlaybackViewModel()
        {
            Player.Source = PlaybackList;
            PlaybackList.CurrentItemChanged += PlaybackList_CurrentItemChanged;
        }

        #region Variables
        /// <summary>
        /// Gets or sets the list of songs that are currently queued.
        /// </summary>
        public ObservableCollection<SongViewModel> PlayingSongs { get; set; } =
            new ObservableCollection<SongViewModel>();

        private SongViewModel _currentSong;

        /// <summary>
        /// Gets the song that's currently playing.
        /// </summary>
        public SongViewModel CurrentSong
        {
            get => _currentSong;
            set => Set(ref _currentSong, value);
        }

        public MediaPlayer Player { get; }
            = new MediaPlayer();

        public MediaPlaybackList PlaybackList { get; set; }
            = new MediaPlaybackList();

        private CancellationTokenSource CTS { get; set; }
            = new CancellationTokenSource();

        public CancellationToken Token => CTS.Token;

        private bool CanContinue = true;
        #endregion

        public async Task StartShuffle(IEnumerable<SongViewModel> songs)
        {
            songs = App.MViewModel.SortSongs(songs, SortMethods.Random);

            CancelTask();
            await CreatePlaybackList(0, songs, Token);
        }

        public async Task StartPlayback(IEnumerable<SongViewModel> songs, int startIndex)
        {
            CancelTask();
            await CreatePlaybackList(startIndex, songs, Token);
        }

        public async Task CreatePlaybackList(int index, IEnumerable<SongViewModel> songs, CancellationToken token)
        {
            while (!CanContinue)
            {
                // Not so efficient, but it's legitimately the only thing I could
                // think of to prevent the tasks from overlapping
                await Task.Delay(30);
            }

            CanContinue = false;

            Debug.WriteLine("Starting with item #" + index);

            int addedSongs = 1;
            int itemCount = songs.Count();

            PlayingSongs.Clear();
            PlaybackList.Items.Clear();

            // Add initial item to avoid delays when starting playback
            MediaPlaybackItem item =
                await CreateMusicItem(songs.ElementAt(index));

            PlaybackList.Items.Add(item);
            PlayingSongs.Add(songs.ElementAt(index));

            // Not disposing the media player here is intentional, it gets
            // marshalled from a different thread when setting the media players
            // and running on the UI thread here isn't desirable.
            Player.Source = PlaybackList;
            Player.Play();

            SetCurrentSong(item);

            if (itemCount <= 1)
            {
                Debug.WriteLine("Added 1 song.");
                CanContinue = true;
                return;
            }

            // Needs to account for the selected index offset.
            for (int i = index + 1; addedSongs < itemCount; i++)
            {
                if (token.IsCancellationRequested)
                {
                    Debug.WriteLine("Stop!");
                    CanContinue = true;
                    return;
                }

                if (i == itemCount)
                {
                    i = 0;
                }

                item = await CreateMusicItem(songs.ElementAt(i));
                PlaybackList.Items.Add(item);
                PlayingSongs.Add(songs.ElementAt(i));

                addedSongs++;
                if (i >= itemCount - 1)
                {
                    i = -1;
                }
            }

            Debug.WriteLine("Added " + addedSongs + " songs.");
            CanContinue = true;
            return;
        }

        /// <summary>
        /// Creates a MediaPlaybackItem from a SongViewModel.
        /// </summary>
        /// <param name="model">Song to convert.</param>
        /// <returns>A MediaPlaybackItem based on the song.</returns>
        private async Task<MediaPlaybackItem> CreateMusicItem(SongViewModel model)
        {
            StorageFile file = await StorageFile.GetFileFromPathAsync(model.Location);

            MediaSource source = MediaSource.CreateFromStorageFile(file);
            MediaPlaybackItem media = new MediaPlaybackItem(source);

            MediaItemDisplayProperties props = media.GetDisplayProperties();
            props.Type = MediaPlaybackType.Music;

            props.MusicProperties.Title = model.Title;
            props.MusicProperties.Artist = model.Artist;
            props.MusicProperties.AlbumTitle = model.Album;
            props.MusicProperties.AlbumArtist = model.AlbumArtist;
            props.MusicProperties.TrackNumber = model.Track;

            if (model.Thumbnail != null)
            {
                props.Thumbnail = RandomAccessStreamReference.CreateFromUri(new Uri(model.Thumbnail));
            }

            media.ApplyDisplayProperties(props);
            return media;
        }

        public void SetCurrentSong(MediaPlaybackItem item)
        {
            if (item == null)
            {
                return;
            }

            MusicDisplayProperties props = item.GetDisplayProperties().MusicProperties;
            SongViewModel song = new SongViewModel
            {
                Title = props.Title,
                Artist = props.Artist,
                Album = props.AlbumTitle,
                AlbumArtist = props.AlbumArtist,
                Track = props.TrackNumber
            };

            CurrentSong = song;
        }

        public void CancelTask()
        {
            CTS.Cancel();
            CTS = new CancellationTokenSource();
        }

        private async void PlaybackList_CurrentItemChanged(MediaPlaybackList sender, CurrentMediaPlaybackItemChangedEventArgs args)
        {
            Debug.WriteLine("Item changed!");
            if (args.Reason == MediaPlaybackItemChangedReason.Error)
            {
                Debug.WriteLine("Can't do much with this really.");
                return;
            }

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                SetCurrentSong(sender.CurrentItem);
            });
        }
    }
}
