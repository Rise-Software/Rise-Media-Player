using Rise.Common.Enums;
using Rise.Common.Extensions;
using Rise.Common.Interfaces;
using SQLite;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace Rise.Models
{
    /// <summary>
    /// Represents a video.
    /// </summary>
    [Table("Videos")]
    public partial class Video : DbObject, IEquatable<Video>, IMatchable<Video>
    {
        public string Directors { get; set; }

        public TimeSpan Length { get; set; }

        public string Location { get; set; }

        [NotNull]
        public uint Rating { get; set; }

        public string Title { get; set; }

        [NotNull]
        public uint Year { get; set; }

        public string Thumbnail { get; set; }

        [Ignore]
        public bool IsLocal { get; set; }

        /// <summary>
        /// Returns the video title.
        /// </summary>
        public override string ToString()
        {
            return Title;
        }
    }

    // Constructors/Factory methods
    public partial class Video
    {
        /// <summary>
        /// Creates a <see cref="Video"/> based on a <see cref="StorageFile"/>.
        /// </summary>
        /// <param name="file">Video file.</param>
        /// <returns>A video based on the file.</returns>
        public static async Task<Video> GetFromFileAsync(StorageFile file)
        {
            // Put the value into memory to make sure that the system
            // really fetches the properties
            var videoProperties = await file.Properties.GetVideoPropertiesAsync();

            string directors = videoProperties.Directors.Count > 0
                ? string.Join(";", videoProperties.Directors) : "UnknownArtistResource";

            return new Video
            {
                Title = videoProperties.Title.ReplaceIfNullOrWhiteSpace(file.DisplayName),
                Directors = directors,
                Length = videoProperties.Duration,
                Year = videoProperties.Year,
                Location = file.Path,
                Rating = videoProperties.Rating,
                IsLocal = file.Provider.Id == "computer"
            };
        }
    }

    // IEquatable implementation
    public partial class Video : IEquatable<Video>
    {
        public bool Equals(Video other)
        {
            return Location == other.Location;
        }

        public override int GetHashCode()
        {
            return Location.GetHashCode();
        }
    }

    // IMatchable implementation
    public partial class Video : IMatchable<Video>
    {
        public MatchLevel Matches(Video other)
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
