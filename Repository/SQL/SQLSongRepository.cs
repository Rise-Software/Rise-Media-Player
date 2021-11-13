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
        private readonly List<Song> _songs;

        public SQLSongRepository(DbContextOptions<Context> options)
        {
            _dbOptions = options;
            _songs = new List<Song>();
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

        public async Task QueueUpsertAsync(Song item)
        {
            _songs.Add(item);
            if (_songs.Count >= 200)
            {
                await UpsertQueuedAsync();
            }
        }

        public async Task UpsertQueuedAsync()
        {
            using (_db = new Context(_dbOptions))
            {
                await _db.BulkInsertOrUpdateAsync(_songs);
                _songs.Clear();
            }
        }

        public async Task DeleteAsync(Song item)
        {
            using (_db = new Context(_dbOptions))
            {
                if (null != item)
                {
                    _ = _db.Songs.Remove(item);
                    _ = await _db.SaveChangesAsync().ConfigureAwait(false);
                }
            }
        }
    }
}
