using System;
using SQLite;

namespace Rise.Models
{
    /// <summary>
    /// Represents an Artist.
    /// </summary>
    [Table("Artists")]
    public class Artist : DbObject, IEquatable<Artist>
    {
        [Column(nameof(Name))]
        public string Name { get; set; }

        [Column(nameof(Picture))]
        public string Picture { get; set; }

        [Column(nameof(SongCount))]
        [NotNull]
        public int SongCount { get; set; }

        [Column(nameof(AlbumCount))]
        [NotNull]
        public int AlbumCount { get; set; }

        /// <summary>
        /// Returns the Album title.
        /// </summary>
        public override string ToString() => Name;

        public bool Equals(Artist other)
        {
            return Name == other.Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
