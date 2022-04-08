using Rise.Common.Helpers;
using Rise.Common.Interfaces;

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
    }
}
