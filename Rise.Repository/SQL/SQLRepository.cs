using Microsoft.EntityFrameworkCore;
using Rise.Models;
using System.IO;
using Windows.Storage;

namespace Rise.Repository.SQL
{
    public class SQLRepository
    {
        private readonly DbContextOptions<Context> _dbOptions;

        private static SQLRepository _repository;
        public static SQLRepository Repository
        {
            get
            {
                if (_repository == null)
                {
                    string dbPath = Path.Combine(ApplicationData.Current.LocalCacheFolder.Path, "Files.db");

                    DbContextOptionsBuilder<Context> dbOptions = new DbContextOptionsBuilder<Context>().UseSqlite(
                        "Data Source=" + dbPath);

                    _repository = new SQLRepository(dbOptions);
                }

                return _repository;
            }
        }

        private SQLRepository(DbContextOptionsBuilder<Context>
            dbOptionsBuilder)
        {
            _dbOptions = dbOptionsBuilder.Options;
            using (Context db = new Context(_dbOptions))
            {
                _ = db.Database.EnsureCreated();
            }

            Songs = new SQLSongRepository(_dbOptions);
            Albums = new SQLAlbumRepository(_dbOptions);
            Artists = new SQLArtistRepository(_dbOptions);
            Genres = new SQLGenreRepository(_dbOptions);
            Videos = new SQLVideoRepository(_dbOptions);
        }

        /// <summary>
        /// Returns the songs repository.
        /// </summary>
        public ISQLRepository<Song> Songs { get; set; }

        /// <summary>
        /// Returns the albums repository.
        /// </summary>
        public ISQLRepository<Album> Albums { get; set; }

        /// <summary>
        /// Returns the artists repository.
        /// </summary>
        public ISQLRepository<Artist> Artists { get; set; }

        /// <summary>
        /// Returns the genres repository.
        /// </summary>
        public ISQLRepository<Genre> Genres { get; set; }

        /// <summary>
        /// Returns the videos repository.
        /// </summary>
        public ISQLRepository<Video> Videos { get; set; }
    }
}