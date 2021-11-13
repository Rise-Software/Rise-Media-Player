using Microsoft.EntityFrameworkCore;
using Rise.Models;

namespace Rise.Repository.SQL
{
    public class SQLRepository : IRepository
    {
        private readonly DbContextOptions<Context> _dbOptions;

        public SQLRepository(DbContextOptionsBuilder<Context>
            dbOptionsBuilder)
        {
            _dbOptions = dbOptionsBuilder.Options;
            using (Context db = new Context(_dbOptions))
            {
                _ = db.Database.EnsureCreated();
            }
        }

        public ISQLRepository<Song> Songs => new SQLSongRepository(_dbOptions);
        public ISQLRepository<Album> Albums => new SQLAlbumRepository(_dbOptions);
        public ISQLRepository<Artist> Artists => new SQLArtistRepository(_dbOptions);
        public ISQLRepository<Genre> Genres => new SQLGenreRepository(_dbOptions);
        public ISQLRepository<Video> Videos => new SQLVideoRepository(_dbOptions);
    }
}
