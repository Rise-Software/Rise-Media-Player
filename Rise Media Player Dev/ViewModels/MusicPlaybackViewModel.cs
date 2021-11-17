using Rise.App.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.UI.Core;

namespace Rise.App.ViewModels
{
    public class MusicPlaybackViewModel : ViewModel, ICancellableTask
    {
        /// <summary>
        /// Creates a new <see cref="MusicPlaybackViewModel"/>.
        /// </summary>
        public MusicPlaybackViewModel()
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

        public CancellationTokenSource CTS { get; set; }
            = new CancellationTokenSource();

        public CancellationToken Token => CTS.Token;

        public bool CanContinue { get; set; }
            = true;
        #endregion

        public async Task StartShuffleAsync(IEnumerator<object> songs, int count)
        {
            List<SongViewModel> list = new List<SongViewModel>();

            while (songs.MoveNext())
            {
                list.Add(songs.Current as SongViewModel);
            }

            Random rng = new Random();
            list = list.OrderBy(s => rng.Next()).ToList();

            CancelTask();
            await CreatePlaybackListAsync(0, count,
                list.AsEnumerable().GetEnumerator(), Token);
        }

        public async Task StartPlaybackAsync(IEnumerator<object> songs, int startIndex, int count)
        {
            CancelTask();
            await CreatePlaybackListAsync(startIndex, count, songs, Token);
        }

        public async Task StartPlaybackAsync(IEnumerator<IStorageItem> songs, int startIndex, int count)
        {
            CancelTask();
            List<SongViewModel> list = new List<SongViewModel>();
            while (songs.MoveNext())
            {
                list.Add(new SongViewModel
                    (await (songs.Current as StorageFile).AsSongModelAsync()));
            }

            await CreatePlaybackListAsync(startIndex, count, list.GetEnumerator(), Token);
            songs.Dispose();
        }

        public async Task CreatePlaybackListAsync(int index, int count, IEnumerator<object> songs, CancellationToken token)
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
            SongViewModel song = songs.Current as SongViewModel;
            MediaPlaybackItem item = await song.AsPlaybackItemAsync();

            PlaybackList.Items.Add(item);
            PlayingSongs.Add(songs.Current as SongViewModel);

            // Not disposing the media player here is intentional, it gets
            // marshalled from a different thread when setting the media players
            // and running on the UI thread here isn't desirable.
            Player.Source = PlaybackList;
            Player.Play();

            SetCurrentSong(0);
            while (addedSongs < count)
            {
                if (token.IsCancellationRequested)
                {
                    Debug.WriteLine("Stop!");
                    songs.Dispose();
                    CanContinue = true;
                    return;
                }

                if (!songs.MoveNext())
                {
                    songs.Reset();
                    songs.MoveNext();
                }

                song = songs.Current as SongViewModel;
                item = await song.AsPlaybackItemAsync();

                PlaybackList.Items.Add(item);
                PlayingSongs.Add(songs.Current as SongViewModel);

                addedSongs++;
            }

            songs.Dispose();
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
