using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Rise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rise.Repository.SQL
{
    public class SQLGenreRepository : ISQLRepository<Genre>
    {
        private static Context _db;
        private readonly DbContextOptions<Context> _dbOptions;
        private readonly List<Genre> _upsertQueue;
        private readonly List<Genre> _removalQueue;

        public SQLGenreRepository(DbContextOptions<Context> options)
        {
            _dbOptions = options;

            _upsertQueue = new List<Genre>();
            _removalQueue = new List<Genre>();
        }

        public async Task<IEnumerable<Genre>> GetAsync()
        {
            using (_db = new Context(_dbOptions))
            {
                return await _db.Genres
                    .AsNoTracking()
                    .ToListAsync();
            }
        }

        public async Task<Genre> GetAsync(Guid id)
        {
            using (_db = new Context(_dbOptions))
            {
                return await _db.Genres
                    .AsNoTracking()
                    .FirstOrDefaultAsync(g => g.Id == id);
            }
        }

        public async Task<IEnumerable<Genre>> GetAsync(string search)
        {
            using (_db = new Context(_dbOptions))
            {
                string[] parameters = search.Split(' ');
                return await _db.Genres
                    .Where(genre =>
                        parameters.Any(parameter =>
                            genre.Name.StartsWith(parameter, StringComparison.OrdinalIgnoreCase)))
                    .OrderByDescending(genre =>
                        parameters.Count(parameter =>
                            genre.Name.StartsWith(parameter)))
                    .AsNoTracking()
                    .ToListAsync();
            }
        }

        public async Task UpsertAsync(Genre item)
        {
            using (_db = new Context(_dbOptions))
            {
                await _db.Genres.AddAsync(item);
                await _db.SaveChangesAsync();
            }
        }

        public async Task QueueUpsertAsync(Genre item)
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

        public async Task DeleteAsync(Genre item)
        {
            using (_db = new Context(_dbOptions))
            {
                _db.Genres.Remove(item);
                await _db.SaveChangesAsync();
            }
        }

        public async Task QueueDeletionAsync(Genre item)
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
