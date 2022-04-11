using Rise.Common.Interfaces;
using Rise.Data.ViewModels;
using Rise.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Media.Playback;
using Windows.Storage;

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

        private PlaybackMetaViewModel _currentPlaybackItem = new();

        /// <summary>
        /// Gets the media that's currently playing.
        /// </summary>
        public PlaybackMetaViewModel CurrentPlaybackItem
        {
            get => _currentPlaybackItem;
            set => Set(ref _currentPlaybackItem, value);
        }

        public readonly MediaPlayer Player = new();

        public MediaPlaybackList PlaybackList { get; set; }
            = new MediaPlaybackList();

        public CancellationTokenSource CTS { get; set; }
            = new CancellationTokenSource();

        public CancellationToken Token => CTS.Token;

        public bool CanContinue { get; set; }
            = true;

        public event EventHandler CurrentMediaChanged, CurrentSongChanged, CurrentVideoChanged;
        #endregion

        private void ClearLists()
        {
            PlayingSongs.Clear();
            PlayingVideos.Clear();
            PlaybackList.Items.Clear();

            CurrentSong = null;
            CurrentVideo = null;
        }

        public async Task PlaySongAsync(SongViewModel song)
        {
            try
            {
                CancelTask();

                ClearLists();
                PlaybackList.ShuffleEnabled = false;

                PlaybackList.Items.Add(await song.AsPlaybackItemAsync());

                CurrentSong = song;

                Player.Play();

                if (App.SViewModel.Color == -3)
                {
                    App.SViewModel.Color = -1;
                    App.SViewModel.Color = -3;
                }

                CurrentMediaChanged?.Invoke(this, EventArgs.Empty);
                CurrentSongChanged?.Invoke(this, EventArgs.Empty);
                CurrentPlaybackItem.NotifyChanges(false);
            }
            catch
            {

            }
        }

        public void PlaySongFromUrl(SongViewModel song)
        {
            try
            {
                CancelTask();

                ClearLists();
                PlaybackList.ShuffleEnabled = false;

                PlaybackList.Items.Add(song.AsPlaybackItem(new Uri(song.Location)));

                CurrentSong = song;

                Player.Play();

                if (App.SViewModel.Color == -3)
                {
                    App.SViewModel.Color = -1;
                    App.SViewModel.Color = -3;
                }

                CurrentMediaChanged?.Invoke(this, EventArgs.Empty);
                CurrentSongChanged?.Invoke(this, EventArgs.Empty);

                CurrentPlaybackItem.NotifyChanges(false);
            }
            catch
            {

            }
        }

        public async Task PlayVideoAsync(VideoViewModel video)
        {
            try
            {
                CancelTask();

                ClearLists();
                PlaybackList.ShuffleEnabled = false;

                PlaybackList.Items.Add(await video.AsPlaybackItemAsync());

                CurrentVideo = video;

                Player.Play();

                CurrentMediaChanged?.Invoke(this, EventArgs.Empty);
                CurrentVideoChanged?.Invoke(this, EventArgs.Empty);
                CurrentPlaybackItem.NotifyChanges(true);
            } catch
            {

            }
        }

        public void PlayVideoFromUrl(VideoViewModel video)
        {
            try
            {
                CancelTask();

                ClearLists();
                PlaybackList.ShuffleEnabled = false;

                PlaybackList.Items.Add(video.AsPlaybackItem(new Uri(video.Location)));

                CurrentVideo = video;

                Player.Play();

                CurrentMediaChanged?.Invoke(this, EventArgs.Empty);
                CurrentVideoChanged?.Invoke(this, EventArgs.Empty);
                CurrentPlaybackItem.NotifyChanges(true);
            }
            catch
            {

            }
        }

        public async Task StartVideoPlaybackAsync(IEnumerator<VideoViewModel> videos, int startIndex, int count, bool shuffle = false)
        {
            try
            {
                CancelTask();
                PlaybackList.ShuffleEnabled = shuffle;

                await CreatePlaybackListAsync(startIndex, count, videos, Token);
            } catch
            {

            }
        }

        public async Task StartMusicPlaybackAsync(IEnumerator<SongViewModel> songs, int startIndex, int count, bool shuffle = false)
        {
            try
            {
                CancelTask();
                PlaybackList.ShuffleEnabled = shuffle;

                await CreatePlaybackListAsync(startIndex, count, songs, Token);
            } catch
            {

            }
        }

        public async Task StartMusicPlaybackFromUrlAsync(IEnumerator<SongViewModel> songs, int startIndex, int count, bool shuffle = false)
        {
            try
            {
                CancelTask();
                PlaybackList.ShuffleEnabled = shuffle;

                await CreatePlaybackListAsync(startIndex, count, songs, Token);
            } catch
            {

            }
        }

        public async Task StartMusicPlaybackAsync(IEnumerator<IStorageItem> songs, int startIndex, int count)
        {
            try
            {
                CancelTask();
                PlaybackList.ShuffleEnabled = false;

                List<SongViewModel> list = new();
                while (songs.MoveNext())
                {
                    var song = await Song.GetFromFileAsync(songs.Current as StorageFile);
                    list.Add(new SongViewModel(song));
                }

                songs.Dispose();
                await CreatePlaybackListAsync(startIndex, count, list.GetEnumerator(), Token);

            } catch
            {

            }
        }

        public async Task CreatePlaybackListAsync(int index, int count, IEnumerator<SongViewModel> songs, CancellationToken token)
        {
            while (!CanContinue)
            {
                // Not so efficient, but it's legitimately the only thing I could
                // think of to prevent the tasks from overlapping
                await Task.Delay(30);
            }

            ClearLists();
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

            // Make sure the song instance isn't null in case there are no songs
            if (song == null)
            {
                songs.Dispose();
                CanContinue = true;
                return;
            }

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

            ClearLists();
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

            SetCurrentVideo(0);
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

        public void SetCurrentSong(int index)
        {
            try
            {
                if (index >= 0 && index < PlayingSongs.Count)
                {
                    CurrentVideo = null;
                    CurrentSong = PlayingSongs[index];

                    if (App.SViewModel.Color == -3)
                    {
                        App.SViewModel.Color = -1;
                        App.SViewModel.Color = -3;
                    }

                    CurrentMediaChanged?.Invoke(this, EventArgs.Empty);
                    CurrentSongChanged?.Invoke(this, EventArgs.Empty);
                    CurrentPlaybackItem.NotifyChanges(false);
                }
            } catch
            {

            }
        }

        public void SetCurrentVideo(int index)
        {
            try
            {
                if (index >= 0 && index < PlayingVideos.Count)
                {
                    CurrentSong = null;
                    CurrentVideo = PlayingVideos[index];

                    if (App.SViewModel.Color == -3)
                    {
                        App.SViewModel.Color = -1;
                        App.SViewModel.Color = -3;
                    }

                    CurrentMediaChanged?.Invoke(this, EventArgs.Empty);
                    CurrentVideoChanged?.Invoke(this, EventArgs.Empty);
                    CurrentPlaybackItem.NotifyChanges(true);
                }
            } catch
            {

            }
        }

        public void CancelTask()
        {
            CTS.Cancel();
            CTS = new CancellationTokenSource();
        }

        private void PlaybackList_CurrentItemChanged(MediaPlaybackList sender, CurrentMediaPlaybackItemChangedEventArgs args)
        {
            if (args.Reason == MediaPlaybackItemChangedReason.Error)
            {
                Debug.WriteLine("Error when going to next item.");
                return;
            }

            if (sender.CurrentItem != null)
            {
                if (sender.CurrentItem.GetDisplayProperties().Type == MediaPlaybackType.Music)
                {
                    SetCurrentSong((int)sender.CurrentItemIndex);
                }
                else
                {
                    SetCurrentVideo((int)sender.CurrentItemIndex);
                }
            }
        }
    }
}
