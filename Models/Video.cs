using System;

namespace Rise.Models
{
    public class Video : DbObject, IEquatable<Video>
    {
        public string Directors { get; set; }
        public string Location { get; set; }
        public string Rating { get; set; }
        public string Title { get; set; }
        public string Year { get; set; }

        public bool Equals(Video other)
        {
            return Title == other.Title &&
                   Year == other.Year &&
                   Directors == other.Directors;
        }
    }
}
