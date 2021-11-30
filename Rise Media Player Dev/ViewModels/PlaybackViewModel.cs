using Rise.App.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.UI.Core;

namespace Rise.App.ViewModels
{
    public class PlaybackViewModel : ViewModel, ICancellableTask
    {
        /// <summary>
        /// Creates a new <see cref="PlaybackViewModel"/>.
        /// </summary>
        public PlaybackViewModel()
        {
            Player.Source = PlaybackList;
            PlaybackList.AutoRepeatEnabled = true;
            PlaybackList.CurrentItemChanged += PlaybackList_CurrentItemChanged;
        }

        #region Variables
        /// <summary>
        /// Gets or sets the list of songs that are currently queued.
        /// </summary>
        public ObservableCollection<SongViewModel> PlayingSongs { get; set; } =
            new ObservableCollection<SongViewModel>();

        /// <summary>
        /// Gets or sets the list of videos that are currently queued.
        /// </summary>
        public ObservableCollection<VideoViewModel> PlayingVideos { get; set; } =
            new ObservableCollection<VideoViewModel>();

        private SongViewModel _currentSong;
        /// <summary>
        /// Gets the song that's currently playing.
        /// </summary>
        public SongViewModel CurrentSong
        {
            get => _currentSong;
            set => Set(ref _currentSong, value);
        }

        private VideoViewModel _currentVideo;
        /// <summary>
        /// Gets the video that's currently playing.
        /// </summary>
        public VideoViewModel CurrentVideo
        {
            get => _currentVideo;
            set => Set(ref _currentVideo, value);
        }

        public MediaPlayer Player { get; }
            = new MediaPlayer();

        public MediaPlaybackList PlaybackList { get; set; }
            = new MediaPlaybackList();

        public CancellationTokenSource CTS { get; set; }
            = new CancellationTokenSource();

        public CancellationToken Token => CTS.Token;

        public bool CanContinue { get; set; }
            = true;
        #endregion

        public async Task PlaySongAsync(SongViewModel song)
        {
            CancelTask();

            PlaybackList.ShuffleEnabled = false;
            PlaybackList.Items.Clear();

            PlaybackList.Items.Add(await song.AsPlaybackItemAsync());
            Player.Play();
        }

        public async Task PlayVideoAsync(VideoViewModel video)
        {
            CancelTask();

            PlaybackList.ShuffleEnabled = false;
            PlaybackList.Items.Clear();

            PlaybackList.Items.Add(await video.AsPlaybackItemAsync());
            Player.Play();
        }

        public async Task StartMusicPlaybackAsync(IEnumerator<object> songs, int startIndex, int count, bool shuffle = false)
        {
            CancelTask();
            PlaybackList.ShuffleEnabled = shuffle;

            List<SongViewModel> list = new List<SongViewModel>();
            while (songs.MoveNext())
            {
                list.Add(new SongViewModel
                    (await (songs.Current as StorageFile).AsSongModelAsync()));
            }

            songs.Dispose();
            await CreatePlaybackListAsync(startIndex, count, list.GetEnumerator(), Token);
        }

        public async Task StartMusicPlaybackAsync(IEnumerator<IStorageItem> songs, int startIndex, int count)
        {
            CancelTask();
            PlaybackList.ShuffleEnabled = false;

            List<SongViewModel> list = new List<SongViewModel>();
            while (songs.MoveNext())
            {
                list.Add(new SongViewModel
                    (await (songs.Current as StorageFile).AsSongModelAsync()));
            }

            songs.Dispose();
            await CreatePlaybackListAsync(startIndex, count, list.GetEnumerator(), Token);
        }

        public async Task CreatePlaybackListAsync(int index, int count, IEnumerator<SongViewModel> songs, CancellationToken token)
        {
            while (!CanContinue)
            {
                // Not so efficient, but it's legitimately the only thing I could
                // think of to prevent the tasks from overlapping
                await Task.Delay(30);
            }

            PlayingSongs.Clear();
            PlaybackList.Items.Clear();
            CanContinue = false;
            songs.MoveNext();

            int pos = 0;
            int addedSongs = 1;

            while (pos != index)
            {
                pos++;
                songs.MoveNext();
            }

            // Add initial item to avoid delays when starting playback
            SongViewModel song = songs.Current;
            var item = await song.AsPlaybackItemAsync();

            PlaybackList.Items.Add(item);
            PlayingSongs.Add(songs.Current);

            // Not disposing the media player here is intentional, it gets
            // marshalled from a different thread when setting the media players
            // and running on the UI thread here isn't desirable.
            Player.Play();

            SetCurrentSong(0);
            while (addedSongs < count)
            {
                if (token.IsCancellationRequested)
                {
                    songs.Dispose();
                    CanContinue = true;
                    return;
                }

                if (!songs.MoveNext())
                {
                    songs.Reset();
                    songs.MoveNext();
                }

                song = songs.Current;
                item = await song.AsPlaybackItemAsync();

                PlaybackList.Items.Add(item);
                PlayingSongs.Add(songs.Current);

                addedSongs++;
            }

            songs.Dispose();
            CanContinue = true;
        }

        public async Task CreatePlaybackListAsync(int index, int count, IEnumerator<VideoViewModel> videos, CancellationToken token)
        {
            while (!CanContinue)
            {
                // Not so efficient, but it's legitimately the only thing I could
                // think of to prevent the tasks from overlapping
                await Task.Delay(30);
            }

            PlayingSongs.Clear();
            PlaybackList.Items.Clear();
            CanContinue = false;
            videos.MoveNext();

            int pos = 0;
            int addedVideos = 1;

            while (pos != index)
            {
                pos++;
                videos.MoveNext();
            }

            // Add initial item to avoid delays when starting playback
            VideoViewModel video = videos.Current;
            var item = await video.AsPlaybackItemAsync();

            PlaybackList.Items.Add(item);
            PlayingVideos.Add(videos.Current);

            // Not disposing the media player here is intentional, it gets
            // marshalled from a different thread when setting the media players
            // and running on the UI thread here isn't desirable.
            Player.Play();

            SetCurrentSong(0);
            while (addedVideos < count)
            {
                if (token.IsCancellationRequested)
                {
                    videos.Dispose();
                    CanContinue = true;
                    return;
                }

                if (!videos.MoveNext())
                {
                    videos.Reset();
                    videos.MoveNext();
                }

                video = videos.Current;
                item = await video.AsPlaybackItemAsync();

                PlaybackList.Items.Add(item);
                PlayingVideos.Add(videos.Current);

                addedVideos++;
            }

            videos.Dispose();
            CanContinue = true;
        }

        public void SetCurrentSong(uint index)
        {
            if (index >= 0 && index < PlayingSongs.Count)
            {
                CurrentSong = PlayingSongs[(int)index];
            }
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
                SetCurrentSong(sender.CurrentItemIndex);
            });
        }
    }
}
