using Rise.Models;

namespace Rise.Repository
{
    public interface IRepository
    {
        /// <summary>
        /// Returns the songs repository.
        /// </summary>
        ISQLRepository<Song> Songs { get; set; }

        /// <summary>
        /// Returns the albums repository.
        /// </summary>
        ISQLRepository<Album> Albums { get; set; }

        /// <summary>
        /// Returns the artists repository.
        /// </summary>
        ISQLRepository<Artist> Artists { get; set; }

        /// <summary>
        /// Returns the artists repository.
        /// </summary>
        ISQLRepository<Genre> Genres { get; set; }

        /// <summary>
        /// Returns the videos repository.
        /// </summary>
        ISQLRepository<Video> Videos { get; set; }

        /// <summary>
        /// Returns the playlists repository.
        /// </summary>
        ISQLRepository<Playlist> Playlists { get; set; }
    }
}
