using System;

namespace Rise.Models
{
    /// <summary>
    /// Represents a playlist.
    /// </summary>
    public class Playlist : DbObject, IEquatable<Playlist>
    {
        public string Title { get; set; }
        public string Icon { get; set; }
        public string Description { get; set; }
        public string Duration { get; set; }

        /// <summary>
        /// Returns the playlist title.
        /// </summary>
        public override string ToString()
        {
            return Title;
        }

        public bool Equals(Playlist other)
        {
            return Title == other.Title &&
                   Duration == other.Duration &&
                   Icon == other.Icon &&
                   Description == other.Description;
        }
    }
}
