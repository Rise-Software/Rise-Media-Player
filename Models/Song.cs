using System;

namespace Rise.Models
{
    /// <summary>
    /// Represents a song.
    /// </summary>
    public class Song : DbObject, IEquatable<Song>
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public uint Track { get; set; }
        public int Disc { get; set; }
        public string Album { get; set; }
        public string AlbumArtist { get; set; }
        public string Genres { get; set; }
        public string Length { get; set; }
        public uint Year { get; set; }
        public string Location { get; set; }
        public uint Rating { get; set; }
        public bool Removed { get; set; }
        public string Thumbnail { get; set; }

        /// <summary>
        /// Returns the song title.
        /// </summary>
        public override string ToString() => Title;

        public bool Equals(Song other)
        {
            return Title == other.Title &&
                   Artist == other.Artist &&
                   Track == other.Track &&
                   Disc == other.Disc &&
                   Album == other.Album &&
                   AlbumArtist == other.AlbumArtist &&
                   Genres == other.Genres &&
                   Length == other.Length &&
                   Year == other.Year;
        }
    }
}
