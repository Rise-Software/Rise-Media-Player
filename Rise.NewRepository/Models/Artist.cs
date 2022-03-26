using System;

namespace Rise.Models
{
    /// <summary>
    /// Represents an Artist.
    /// </summary>
    public class Artist1 :  IEquatable<Artist1>
    {
        public string Name { get; set; }
        public string Picture { get; set; }
        public int SongCount { get; set; }
        public int AlbumCount { get; set; }

        /// <summary>
        /// Returns the Album title.
        /// </summary>
        public override string ToString() => Name;

        public bool Equals(Artist1 other)
        {
            return Name == other.Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
