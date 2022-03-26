using System;

namespace Rise.Models
{
    /// <summary>
    /// Represents a genre.
    /// </summary>
    public class Genre1 : IEquatable<Genre1>
    {
        public string Name { get; set; }

        /// <summary>
        /// Returns the genre.
        /// </summary>
        public override string ToString() => Name;

        public bool Equals(Genre1 other)
        {
            return Name == other.Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
