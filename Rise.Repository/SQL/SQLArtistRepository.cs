using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Rise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rise.Repository.SQL
{
    public class SQLArtistRepository : ISQLRepository<Artist>
    {
        private static Context _db;
        private readonly DbContextOptions<Context> _dbOptions;
        private readonly List<Artist> _upsertQueue;
        private readonly List<Artist> _removalQueue;

        public SQLArtistRepository(DbContextOptions<Context> options)
        {
            _dbOptions = options;

            _upsertQueue = new List<Artist>();
            _removalQueue = new List<Artist>();
        }

        public async Task<IEnumerable<Artist>> GetAsync()
        {
            using (_db = new Context(_dbOptions))
            {
                return await _db.Artists
                    .AsNoTracking()
                    .ToListAsync();
            }
        }

        public async Task<Artist> GetAsync(Guid id)
        {
            using (_db = new Context(_dbOptions))
            {
                return await _db.Artists
                    .AsNoTracking()
                    .FirstOrDefaultAsync(artist => artist.Id == id);
            }
        }

        public async Task<IEnumerable<Artist>> GetAsync(string search)
        {
            using (_db = new Context(_dbOptions))
            {
                string[] parameters = search.Split(' ');
                return await _db.Artists
                    .Where(artist =>
                        parameters.Any(parameter =>
                            artist.Name.StartsWith(parameter, StringComparison.OrdinalIgnoreCase)))
                    .OrderByDescending(artist =>
                        parameters.Count(parameter =>
                            artist.Name.StartsWith(parameter)))
                    .AsNoTracking()
                    .ToListAsync();
            }
        }

        public async Task UpdateAsync(Artist item)
        {
            using (_db = new Context(_dbOptions))
            {
                _db.Artists.Update(item);
                await _db.SaveChangesAsync();
            }
        }

        public async Task UpsertAsync(Artist item)
        {
            using (_db = new Context(_dbOptions))
            {
                await _db.Artists.AddAsync(item);
                await _db.SaveChangesAsync();
            }
        }

        public async Task QueueUpsertAsync(Artist item)
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

        public async Task DeleteAsync(Artist item)
        {
            using (_db = new Context(_dbOptions))
            {
                _db.Artists.Remove(item);
                await _db.SaveChangesAsync();
            }
        }

        public async Task QueueDeletionAsync(Artist item)
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
