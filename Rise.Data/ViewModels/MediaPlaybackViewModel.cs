using Rise.Common.Helpers;
using Rise.Common.Interfaces;
using System;
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

        private readonly Lazy<MediaPlayer> _player;
        /// <summary>
        /// Gets the app-wide <see cref="MediaPlayer"/> instance.
        /// Lazily instantiated to prevent Windows from showing the
        /// SMTC as soon as the app is opened.
        /// </summary>
        public MediaPlayer Player => _player.Value;

        /// <summary>
        /// The media playback list. It is permanently associated with
        /// the player, due to the fact that we don't ever dispose it.
        /// </summary>
        private readonly MediaPlaybackList PlaybackList = new();
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

    // Constructors, initializers
    public partial class MediaPlaybackViewModel
    {
        public MediaPlaybackViewModel()
        {
            _player = new(CreatePlayerInstance());
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
    }
}
