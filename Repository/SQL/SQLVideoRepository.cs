using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Rise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rise.Repository.SQL
{
    public class SQLVideoRepository : ISQLRepository<Video>
    {
        private static Context _db;
        private readonly DbContextOptions<Context> _dbOptions;
        private readonly List<Video> _videos;

        public SQLVideoRepository(DbContextOptions<Context> options)
        {
            _dbOptions = options;
            _videos = new List<Video>();
        }

        public async Task<IEnumerable<Video>> GetAsync()
        {
            using (_db = new Context(_dbOptions))
            {
                return await _db.Videos
                    .AsNoTracking()
                    .ToListAsync();
            }
        }

        public async Task<Video> GetAsync(Guid id)
        {
            using (_db = new Context(_dbOptions))
            {
                return await _db.Videos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(video => video.Id == id);
            }
        }

        public async Task<IEnumerable<Video>> GetAsync(string search)
        {
            using (_db = new Context(_dbOptions))
            {
                string[] parameters = search.Split(' ');
                return await _db.Videos
                    .Where(video =>
                        parameters.Any(parameter =>
                            video.Title.StartsWith(parameter, StringComparison.OrdinalIgnoreCase)))
                    .OrderByDescending(artist =>
                        parameters.Count(parameter =>
                            artist.Title.StartsWith(parameter)))
                    .AsNoTracking()
                    .ToListAsync();
            }
        }

        public async Task QueueUpsertAsync(Video item)
        {
            _videos.Add(item);
            if (_videos.Count >= 200)
            {
                await UpsertQueuedAsync();
            }
        }

        public async Task UpsertQueuedAsync()
        {
            using (_db = new Context(_dbOptions))
            {
                await _db.BulkInsertOrUpdateAsync(_videos);
                _videos.Clear();
            }
        }

        public async Task DeleteAsync(Video item)
        {
            using (_db = new Context(_dbOptions))
            {
                if (null != item)
                {
                    _ = _db.Videos.Remove(item);
                    _ = await _db.SaveChangesAsync().ConfigureAwait(false);
                }
            }
        }
    }
}
