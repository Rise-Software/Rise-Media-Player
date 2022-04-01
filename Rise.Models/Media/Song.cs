using Rise.Common.Constants;
using Rise.Common.Enums;
using Rise.Common.Interfaces;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace Rise.Models
{
    /// <summary>
    /// Represents a song.
    /// </summary>
    [Table("Songs")]
    public class Song : DbObject, IEquatable<Song>, IMatchable<Song>
    {
        [Column(nameof(Title))]
        public string Title { get; set; }

        [Column(nameof(Artist))]
        public string Artist { get; set; }

        [Column(nameof(Track))]
        [NotNull]
        public uint Track { get; set; }

        [Column(nameof(Disc))]
        [NotNull]
        public int Disc { get; set; }

        [Column(nameof(Album))]
        public string Album { get; set; }

        [Column(nameof(AlbumArtist))]
        public string AlbumArtist { get; set; }

        [Column(nameof(Genres))]
        public string Genres { get; set; }

        [Column(nameof(Length))]
        public TimeSpan Length { get; set; }

        [Column(nameof(Year))]
        [NotNull]
        public uint Year { get; set; }

        [Column(nameof(Location))]
        public string Location { get; set; }

        [Column(nameof(Rating))]
        [NotNull]
        public uint Rating { get; set; }

        [Column(nameof(Thumbnail))]
        public string Thumbnail { get; set; }

        /// <summary>
        /// Returns the song title.
        /// </summary>
        public override string ToString()
        {
            return Title;
        }

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

        public override int GetHashCode()
        {
            return (Title, Artist, Track, Disc, Album,
                AlbumArtist, Genres, Length, Year).GetHashCode();
        }

        /// <summary>
        /// Creates a <see cref="Song"/> based on a <see cref="StorageFile"/>.
        /// </summary>
        /// <param name="file">Song file.</param>
        /// <returns>A song based on the file.</returns>
        public static async Task<Song> GetFromFileAsync(StorageFile file)
        {
            // Put the value into memory to make sure that the system
            // really fetches the property
            MusicProperties musicProperties =
                await file.Properties.GetMusicPropertiesAsync();

            int cd = 1;
            IDictionary<string, object> extraProps =
                await file.Properties.RetrievePropertiesAsync(SongProperties.DiscProperties);

            // Check if disc number is valid
            if (extraProps[SystemMusic.DiscNumber] != null)
            {
                try
                {
                    if (int.TryParse(extraProps[SystemMusic.DiscNumber].ToString(), out int result))
                    {
                        cd = result;
                    }
                    else
                    {
                        Debug.WriteLine("Something wrong happened while parsing song.\nProblematic part of set: " + extraProps[SystemMusic.DiscNumber].ToString());
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Problem: " + ex.Message);
                    Debug.WriteLine("Problematic disc number: " + extraProps[SystemMusic.DiscNumber].ToString());
                }
            }
            else if (extraProps[SystemMusic.PartOfSet] != null)
            {
                try
                {
                    if (int.TryParse(extraProps[SystemMusic.PartOfSet].ToString(), out int result))
                    {
                        cd = result;
                    }
                    else
                    {
                        Debug.WriteLine("Something wrong happened while parsing song.\nProblematic part of set: " + extraProps[SystemMusic.PartOfSet].ToString());
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Problem: " + ex.Message);
                    Debug.WriteLine("Problematic part of set: " + extraProps[SystemMusic.PartOfSet].ToString());
                }
            }

            // Valid song metadata is needed
            string title = musicProperties.Title.Length > 0
                ? musicProperties.Title : Path.GetFileNameWithoutExtension(file.Path);

            string artist = musicProperties.Artist.Length > 0
                ? musicProperties.Artist : "UnknownArtistResource";

            string albumTitle = musicProperties.Album.Length > 0
                ? musicProperties.Album : "UnknownAlbumResource";

            string albumArtist = musicProperties.AlbumArtist.Length > 0
                ? musicProperties.AlbumArtist : "UnknownArtistResource";

            string genre = musicProperties.Genre.FirstOrDefault() != null
                ? musicProperties.Genre.FirstOrDefault() : "UnknownGenreResource";

            TimeSpan length = musicProperties.Duration;

            return new Song
            {
                Title = title,
                Artist = artist,
                Track = musicProperties.TrackNumber,
                Disc = cd,
                Album = albumTitle,
                AlbumArtist = albumArtist,
                Genres = genre,
                Length = length,
                Year = musicProperties.Year,
                Location = file.Path,
                Rating = musicProperties.Rating
            };
        }

        public MatchLevel Matches(Song other)
        {
            if (Title.Equals(other.Title))
            {
                return MatchLevel.Full;
            }

            if (Title.Contains(other.Title))
            {
                return MatchLevel.Partial;
            }

            return MatchLevel.None;
        }
    }
}
