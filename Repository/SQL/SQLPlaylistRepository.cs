using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Rise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rise.Repository.SQL
{
    public class SQLPlaylistRepository : ISQLRepository<Playlist>
    {
        private static Context _db;
        private readonly DbContextOptions<Context> _dbOptions;
        private readonly List<Playlist> _upsertQueue;
        private readonly List<Playlist> _removalQueue;

        public SQLPlaylistRepository(DbContextOptions<Context> options)
        {
            _dbOptions = options;

            _upsertQueue = new List<Playlist>();
            _removalQueue = new List<Playlist>();
        }

        public async Task<IEnumerable<Playlist>> GetAsync()
        {
            using (_db = new Context(_dbOptions))
            {
                return await _db.Playlists
                    .AsNoTracking()
                    .ToListAsync();
            }
        }

        public async Task<Playlist> GetAsync(Guid id)
        {
            using (_db = new Context(_dbOptions))
            {
                return await _db.Playlists
                    .AsNoTracking()
                    .FirstOrDefaultAsync(playlist => playlist.Id == id);
            }
        }

        public async Task<IEnumerable<Playlist>> GetAsync(string search)
        {
            using (_db = new Context(_dbOptions))
            {
                string[] parameters = search.Split(' ');
                return await _db.Playlists
                    .Where(playlist =>
                        parameters.Any(parameter =>
                            playlist.Title.StartsWith(parameter, StringComparison.OrdinalIgnoreCase)))
                    .OrderByDescending(playlist =>
                        parameters.Count(parameter =>
                            playlist.Title.StartsWith(parameter)))
                    .AsNoTracking()
                    .ToListAsync();
            }
        }

        public async Task QueueUpsertAsync(Playlist item)
        {
            _upsertQueue.Add(item);
            if (_upsertQueue.Count >= 200)
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

        public async Task QueueDeletionAsync(Playlist item)
        {
            _removalQueue.Add(item);
            if (_removalQueue.Count >= 200)
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
