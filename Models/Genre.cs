using System;

namespace Rise.Models
{
    /// <summary>
    /// Represents a genre.
    /// </summary>
    public class Genre : DbObject, IEquatable<Genre>
    {
        public string Name { get; set; }
        public bool Removed { get; set; }

        /// <summary>
        /// Returns the genre.
        /// </summary>
        public override string ToString() => Name;

        public bool Equals(Genre other)
        {
            return Name == other.Name;
        }
    }
}
