using System;
using SQLite;

namespace Rise.Models
{
    /// <summary>
    /// Represents a genre.
    /// </summary>
    [Table("Genres")]
    public class Genre : DbObject, IEquatable<Genre>
    {
        [Column(nameof(Name))]
        public string Name { get; set; }

        /// <summary>
        /// Returns the genre.
        /// </summary>
        public override string ToString() => Name;

        public bool Equals(Genre other)
        {
            return Name == other.Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
