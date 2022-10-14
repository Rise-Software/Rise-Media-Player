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
        /// Path to the playback item. This is simply an URI
        /// stored as a string.
        /// </summary>
        string Location { get; }

        /// <summary>
        /// Whether the playback item is stored online or offline.
        /// </summary>
        bool IsOnline { get; }

        /// <summary>
        /// Creates a <see cref="MediaPlaybackItem"/> representation
        /// of this item.
        /// </summary>
        /// <returns>A Task that represents the conversion
        /// operation.</returns>
        Task<MediaPlaybackItem> AsPlaybackItemAsync();
    }
}
