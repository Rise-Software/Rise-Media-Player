using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Rise.Common.Helpers;
using Rise.Common.Interfaces;
using Windows.Media.Playback;

namespace Rise.Data.ViewModels
{
    /// <summary>
    /// This class contains properties and methods that
    /// help when it comes to controlling media playback.
    /// </summary>
    public partial class MediaPlaybackViewModel : ViewModel
    {
        /// <summary>
        /// List of media items that are currently queued for playback.
        /// </summary>
        public readonly SafeObservableCollection<IMediaItem> QueuedItems = new();

        private IMediaItem _playingItem;

        /// <summary>
        /// Gets the media item that is currently playing.
        /// </summary>
        public IMediaItem PlayingItem
        {
            get => _playingItem;
            private set
            {
                Set(ref _playingItem, value);
                PlayingItemChanged?.Invoke(this, _playingItem);
            }
        }

        private MediaPlayer _player;
        /// <summary>
        /// Gets the current <see cref="MediaPlayer"/> instance.
        /// Lazily instantiated to prevent Windows from showing the
        /// SMTC as soon as the app is opened.
        /// </summary>
        public MediaPlayer Player
        {
            get
            {
                if (_player == null)
                {
                    _player = CreatePlayerInstance();
                }

                return _player;
            }
        }

        /// <summary>
        /// The media playback list. It is permanently associated with
        /// the player, due to the fact that we don't ever dispose it.
        /// </summary>
        private readonly MediaPlaybackList PlaybackList = new();

        /// <summary>
        /// Whether the items in the playback list are played in a random
        /// order.
        /// </summary>
        public bool ShuffleEnabled
        {
            get => PlaybackList.ShuffleEnabled;
            set
            {
                PlaybackList.ShuffleEnabled = value;
                OnPropertyChanged();
            }
        }
    }

    // Events
    public partial class MediaPlaybackViewModel
    {
        /// <summary>
        /// Occurs when the media player is disposed and recreated. Also
        /// gets fired when the initial lazy instance takes place.
        /// </summary>
        public event EventHandler<MediaPlayer> MediaPlayerRecreated;

        /// <summary>
        /// Occurs when the currently playing item changes.
        /// </summary>
        public event EventHandler<IMediaItem> PlayingItemChanged;
    }

    // Methods, Event handling
    public partial class MediaPlaybackViewModel
    {
        /// <summary>
        /// Helps cancel a group of Tasks that starts playback.
        /// </summary>
        private readonly CancellableTaskHelper PlaybackCancelHelper = new();

        /// <summary>
        /// Begins playback of an <see cref="IMediaItem"/>.
        /// </summary>
        /// <param name="item">Item to play.</param>
        public async Task PlaySingleItemAsync(IMediaItem item)
        {
            var token = new CancellationToken();
            try
            {
                await PlaybackCancelHelper.CompletePendingAsync(token);

                var task = PlaySingleItemImpl(item, PlaybackCancelHelper.Token);
                await PlaybackCancelHelper.RunAsync(task);
            }
            catch (OperationCanceledException)
            {

            }
        }

        /// <summary>
        /// Begins playback of a collection of <see cref="IMediaItem"/>.
        /// </summary>
        /// <param name="items">Items to play.</param>
        public async Task PlayItemsAsync(IEnumerable<IMediaItem> items)
        {
            var token = new CancellationToken();
            try
            {
                await PlaybackCancelHelper.CompletePendingAsync(token);

                var task = PlayItemsImpl(items, PlaybackCancelHelper.Token);
                await PlaybackCancelHelper.RunAsync(task);
            }
            catch (OperationCanceledException)
            {

            }
        }

        private async Task PlaySingleItemImpl(IMediaItem item, CancellationToken token)
        {
            ResetPlayback();

            token.ThrowIfCancellationRequested();
            var playItem = await item.AsPlaybackItemAsync();

            // We add stuff manually here because the playback list
            // isn't used for single items
            QueuedItems.Add(item);
            PlayingItem = item;

            token.ThrowIfCancellationRequested();
            Player.Source = playItem;
            Player.Play();
        }

        private async Task PlayItemsImpl(IEnumerable<IMediaItem> items, CancellationToken token)
        {
            ResetPlayback();

            int i = 0;
            foreach (var item in items)
            {
                token.ThrowIfCancellationRequested();

                var playItem = await item.AsPlaybackItemAsync();
                PlaybackList.Items.Add(playItem);
                QueuedItems.Add(item);

                // Start playback right after adding the first item...
                if (i == 0)
                {
                    token.ThrowIfCancellationRequested();
                    Player.Source = PlaybackList;
                    Player.Play();

                    // ...and never again.
                    i++;
                }
            }
        }

        private void OnCurrentItemChanged(MediaPlaybackList sender, CurrentMediaPlaybackItemChangedEventArgs args)
        {
            if (args.Reason == MediaPlaybackItemChangedReason.Error)
            {
                System.Diagnostics.Debug.WriteLine("Error when going to next item.");
                return;
            }

            if (sender.CurrentItem != null)
            {
                PlayingItem = QueuedItems[(int)sender.CurrentItemIndex];
            }
        }
    }

    // Constructors, Initializers
    public partial class MediaPlaybackViewModel
    {
        public MediaPlaybackViewModel()
        {
            PlaybackList.CurrentItemChanged += OnCurrentItemChanged;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="MediaPlayer"/>,
        /// invokes the <see cref="MediaPlayerRecreated"/> event and returns
        /// the new instance.
        /// </summary>
        private MediaPlayer CreatePlayerInstance()
        {
            var player = new MediaPlayer();
            MediaPlayerRecreated?.Invoke(this, player);

            return player;
        }

        /// <summary>
        /// Fully resets playback by disposing of the player, clearing
        /// lists and setting the current item to null.
        /// </summary>
        private void ResetPlayback()
        {
            PlaybackList.Items.Clear();
            QueuedItems.Clear();

            PlayingItem = null;
        }
    }
}
