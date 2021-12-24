using System;

namespace Rise.Models
{
    /// <summary>
    /// Represents an Artist.
    /// </summary>
    public class Artist : DbObject, IEquatable<Artist>
    {
        public string Name { get; set; }
        public string Picture { get; set; }
        public int SongCount { get; set; }
        public int AlbumCount { get; set; }

        /// <summary>
        /// Returns the Album title.
        /// </summary>
        public override string ToString()
        {
            return Name;
        }

        public bool Equals(Artist other)
        {
            return Name == other.Name;
        }
    }
}
