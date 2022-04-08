using Rise.Common.Helpers;
using Rise.Common.Interfaces;
using Windows.Media.Playback;

namespace Rise.Data.ViewModels
{
    /// <summary>
    /// This class contains properties and methods that
    /// help when it comes to controlling media playback.
    /// </summary>
    public class MediaPlaybackViewModel : ViewModel
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
        /// Gets the app-wide <see cref="MediaPlayer"/> instance.
        /// Lazily instantiated to prevent Windows from showing the
        /// SMTC as soon as the app is opened.
        /// </summary>
        public MediaPlayer Player
        {
            get
            {
                if (_player == null)
                {
                    _player = new();
                    _player.Source = PlaybackList;
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
}
