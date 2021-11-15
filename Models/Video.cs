using System;

namespace Rise.Models
{
    /// <summary>
    /// Represents a video.
    /// </summary>
    public class Video : DbObject, IEquatable<Video>
    {
        public string Directors { get; set; }
        public string Location { get; set; }
        public uint Rating { get; set; }
        public string Title { get; set; }
        public uint Year { get; set; }
        public bool Removed { get; set; }

        /// <summary>
        /// Returns the video title.
        /// </summary>
        public override string ToString() => Title;

        public bool Equals(Video other)
        {
            return Title == other.Title &&
                   Year == other.Year &&
                   Directors == other.Directors;
        }
    }
}
