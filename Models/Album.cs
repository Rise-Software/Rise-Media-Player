using System;

namespace Rise.Models
{
    /// <summary>
    /// Represents an album.
    /// </summary>
    public class Album : DbObject, IEquatable<Album>
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Genres { get; set; }
        public uint TrackCount { get; set; }
        public string Thumbnail { get; set; }
        public uint Year { get; set; }

        /// <summary>
        /// Returns the Album title.
        /// </summary>
        public override string ToString() => Title;

        public bool Equals(Album other)
        {
            return Title == other.Title;
        }
    }
}
