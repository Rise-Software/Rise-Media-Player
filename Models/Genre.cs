using System;

namespace Rise.Models
{
    /// <summary>
    /// Represents a genre.
    /// </summary>
    public class Genre : DbObject, IEquatable<Genre>
    {
        public string Name { get; set; }

        /// <summary>
        /// Returns the genre.
        /// </summary>
        public override string ToString()
        {
            return Name;
        }

        public bool Equals(Genre other)
        {
            return Name == other.Name;
        }
    }
}
