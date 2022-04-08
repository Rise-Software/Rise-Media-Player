using Windows.Media;

namespace Rise.Common.Interfaces
{
    /// <summary>
    /// Defines a set of properties suitable for displaying
    /// media items.
    /// </summary>
    public interface IMediaItem
    {
        /// <summary>
        /// Title of the playback item. Generally a filename, title
        /// or name.
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Subtitle of the playback item. Generally the author of
        /// the playback item.
        /// </summary>
        string Subtitle { get; }

        /// <summary>
        /// Thumbnail of the playback item. This is simply an URI
        /// stored as a string.
        /// </summary>
        string Thumbnail { get; }

        /// <summary>
        /// Type of playback item.
        /// </summary>
        MediaPlaybackType ItemType { get; }
    }
}
