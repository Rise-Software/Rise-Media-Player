using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Rise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rise.Repository.SQL
{
    public class SQLAlbumRepository : ISQLRepository<Album>
    {
        private static Context _db;
        private readonly DbContextOptions<Context> _dbOptions;
        private readonly List<Album> _upsertQueue;
        private readonly List<Album> _removalQueue;

        public SQLAlbumRepository(DbContextOptions<Context> options)
        {
            _dbOptions = options;

            _upsertQueue = new List<Album>();
            _removalQueue = new List<Album>();
        }

        public async Task<IEnumerable<Album>> GetAsync()
        {
            using (_db = new Context(_dbOptions))
            {
                return await _db.Albums
                    .AsNoTracking()
                    .ToListAsync();
            }
        }

        public async Task<Album> GetAsync(Guid id)
        {
            using (_db = new Context(_dbOptions))
            {
                return await _db.Albums
                    .AsNoTracking()
                    .FirstOrDefaultAsync(album => album.Id == id);
            }
        }

        public async Task<IEnumerable<Album>> GetAsync(string search)
        {
            using (_db = new Context(_dbOptions))
            {
                string[] parameters = search.Split(' ');
                return await _db.Albums
                    .Where(album =>
                        parameters.Any(parameter =>
                            album.Title.StartsWith(parameter, StringComparison.OrdinalIgnoreCase) ||
                            album.Artist.StartsWith(parameter, StringComparison.OrdinalIgnoreCase)))
                    .OrderByDescending(album =>
                        parameters.Count(parameter =>
                            album.Title.StartsWith(parameter) ||
                            album.Artist.StartsWith(parameter)))
                    .AsNoTracking()
                    .ToListAsync();
            }
        }

        public async Task UpdateAsync(Album item)
        {
            using (_db = new Context(_dbOptions))
            {
                _db.Albums.Update(item);
                await _db.SaveChangesAsync();
            }
        }

        public async Task UpsertAsync(Album item)
        {
            using (_db = new Context(_dbOptions))
            {
                await _db.Albums.AddAsync(item);
                await _db.SaveChangesAsync();
            }
        }

        public async Task QueueUpsertAsync(Album item)
        {
            _upsertQueue.Add(item);
            if (_upsertQueue.Count >= 250)
            {
                await UpsertQueuedAsync();
            }
        }

        public async Task UpsertQueuedAsync()
        {
            using (_db = new Context(_dbOptions))
            {
                await _db.BulkInsertOrUpdateAsync(_upsertQueue);
                _upsertQueue.Clear();
            }
        }

        public async Task DeleteAsync(Album item)
        {
            using (_db = new Context(_dbOptions))
            {
                _db.Albums.Remove(item);
                await _db.SaveChangesAsync();
            }
        }

        public async Task QueueDeletionAsync(Album item)
        {
            _removalQueue.Add(item);
            if (_removalQueue.Count >= 250)
            {
                await DeleteQueuedAsync();
            }
        }

        public async Task DeleteQueuedAsync()
        {
            using (_db = new Context(_dbOptions))
            {
                await _db.BulkDeleteAsync(_removalQueue);
                _removalQueue.Clear();
            }
        }
    }
}
