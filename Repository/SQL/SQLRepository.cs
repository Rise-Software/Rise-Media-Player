using Microsoft.EntityFrameworkCore;

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

        public ISongRepository Songs => new SQLSongRepository(
            new Context(_dbOptions));

        public IAlbumRepository Albums => new SQLAlbumRepository(
            new Context(_dbOptions));

        public IArtistRepository Artists => new SQLArtistRepository(
            new Context(_dbOptions));

        // public IVideoRepository Videos => new SQLVideoRepository(
        // new Context(_dbOptions));
    }
}
