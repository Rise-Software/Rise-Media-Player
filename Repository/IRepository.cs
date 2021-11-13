using Rise.Models;

namespace Rise.Repository
{
    public interface IRepository
    {
        /// <summary>
        /// Returns the songs repository.
        /// </summary>
        ISQLRepository<Song> Songs { get; }

        /// <summary>
        /// Returns the albums repository.
        /// </summary>
        ISQLRepository<Album> Albums { get; }

        /// <summary>
        /// Returns the artists repository.
        /// </summary>
        ISQLRepository<Artist> Artists { get; }

        /// <summary>
        /// Returns the artists repository.
        /// </summary>
        ISQLRepository<Genre> Genres { get; }

        /// <summary>
        /// Returns the videos repository.
        /// </summary>
        ISQLRepository<Video> Videos { get; }
    }
}
