using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Media.Playback;

namespace Rise.Common.Interfaces
{
    /// <summary>
    /// Defines a set of properties suitable for displaying
    /// media items.
    /// </summary>
    public interface IMediaItem : INotifyPropertyChanged
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
        /// Extra information for the playback item. Keep it
        /// short if possible.
        /// </summary>
        string ExtraInfo { get; }

        /// <summary>
        /// Path to the playback item. This is simply an URI
        /// stored as a string.
        /// </summary>
        string Location { get; }

        /// <summary>
        /// Thumbnail of the playback item. This is simply an URI
        /// stored as a string.
        /// </summary>
        string Thumbnail { get; }

        /// <summary>
        /// Whether the playback item is stored online or offline.
        /// </summary>
        bool IsOnline { get; }

        /// <summary>
        /// The length of the media item.
        /// </summary>
        TimeSpan Length { get; }

        /// <summary>
        /// Type of playback item.
        /// </summary>
        MediaPlaybackType ItemType { get; }

        /// <summary>
        /// Creates a <see cref="MediaPlaybackItem"/> representation
        /// of this item.
        /// </summary>
        /// <returns>A Task that represents the conversion
        /// operation.</returns>
        Task<MediaPlaybackItem> AsPlaybackItemAsync();
    }
}
