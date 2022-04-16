using Rise.Common.Helpers;
using Rise.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
            private set => Set(ref _playingItem, value);
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
    }

    // Methods
    public partial class MediaPlaybackViewModel
    {
        /// <summary>
        /// Begins playback of an <see cref="IMediaItem"/>.
        /// </summary>
        /// <param name="item">Item to play.</param>
        public async Task PlayItemAsync(IMediaItem item)
        {
            ResetPlayback();

            var playItem = await item.AsPlaybackItemAsync();

            Player.Source = playItem;
            Player.Play();
        }

        /// <summary>
        /// Begins playback of a collection of <see cref="IMediaItem"/>.
        /// </summary>
        /// <param name="items">Items to play.</param>
        /// <param name="cancellationToken">A token for cancellation support.
        /// In case you want to play a new set of items before this Task is
        /// done.</param>
        public async Task PlayItemsAsync(IEnumerable<IMediaItem> items,
            CancellationToken cancellationToken)
        {
            ResetPlayback();

            int i = 0;
            foreach (var item in items)
            {
                var playItem = await item.AsPlaybackItemAsync();
                PlaybackList.Items.Add(playItem);

                // Start playback right after adding the first item...
                if (i == 0)
                {
                    Player.Source = PlaybackList;
                    Player.Play();

                    // ...and never again.
                    i++;
                }

                cancellationToken.ThrowIfCancellationRequested();
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
    }

    // Constructors, Initializers
    public partial class MediaPlaybackViewModel
    {
        public MediaPlaybackViewModel() { }

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
            _ = DisposePlayerInstance();

            PlaybackList.Items.Clear();
            QueuedItems.Clear();

            PlayingItem = null;
        }

        /// <summary>
        /// Disposes the current instance of the <see cref="MediaPlayer"/>.
        /// </summary>
        /// <returns>true if the player was successfully disposed, false
        /// otherwise.</returns>
        private bool DisposePlayerInstance()
        {
            if (_player != null)
            {
                _player.Pause();
                _player.Dispose();
                _player = null;

                return true;
            }

            return false;
        }
    }
}
