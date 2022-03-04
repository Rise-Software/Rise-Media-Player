using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Rise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rise.Repository.SQL
{
    public class SQLSongRepository : ISQLRepository<Song>
    {
        private static Context _db;
        private readonly DbContextOptions<Context> _dbOptions;
        private readonly List<Song> _upsertQueue;
        private readonly List<Song> _removalQueue;

        public SQLSongRepository(DbContextOptions<Context> options)
        {
            _dbOptions = options;

            _upsertQueue = new List<Song>();
            _removalQueue = new List<Song>();
        }

        public async Task<IEnumerable<Song>> GetAsync()
        {
            using (_db = new Context(_dbOptions))
            {
                return await _db.Songs
                    .AsNoTracking()
                    .ToListAsync();
            }
        }

        public async Task<Song> GetAsync(Guid id)
        {
            using (_db = new Context(_dbOptions))
            {
                return await _db.Songs
                    .AsNoTracking()
                    .FirstOrDefaultAsync(song => song.Id == id);
            }
        }

        public async Task<IEnumerable<Song>> GetAsync(string search)
        {
            using (_db = new Context(_dbOptions))
            {
                string[] parameters = search.Split(' ');
                return await _db.Songs
                    .Where(song =>
                        parameters.Any(parameter =>
                            song.Title.StartsWith(parameter, StringComparison.OrdinalIgnoreCase) ||
                            song.Artist.StartsWith(parameter, StringComparison.OrdinalIgnoreCase) ||
                            song.Location.StartsWith(parameter, StringComparison.OrdinalIgnoreCase)))
                    .OrderByDescending(song =>
                        parameters.Count(parameter =>
                            song.Title.StartsWith(parameter) ||
                            song.Artist.StartsWith(parameter) ||
                            song.Location.StartsWith(parameter)))
                    .AsNoTracking()
                    .ToListAsync();
            }
        }

        public async Task UpdateAsync(Song item)
        {
            using (_db = new Context(_dbOptions))
            {
                _db.Songs.Update(item);
                await _db.SaveChangesAsync();
            }
        }

        public async Task UpsertAsync(Song item)
        {
            using (_db = new Context(_dbOptions))
            {
                await _db.Songs.AddAsync(item);
                await _db.SaveChangesAsync();
            }
        }

        public async Task QueueUpsertAsync(Song item)
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

        public async Task DeleteAsync(Song item)
        {
            using (_db = new Context(_dbOptions))
            {
                _db.Songs.Remove(item);
                await _db.SaveChangesAsync();
            }
        }

        public async Task QueueDeletionAsync(Song item)
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
