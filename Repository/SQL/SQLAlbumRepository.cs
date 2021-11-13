using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Rise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rise.Repository.SQL
{
    public class SQLAlbumRepository : IAlbumRepository
    {
        private static Context _db;
        private readonly DbContextOptions<Context> _dbOptions;
        private readonly List<Album> _albums;

        public SQLAlbumRepository(DbContextOptions<Context> options)
        {
            _dbOptions = options;
            _albums = new List<Album>();
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

        public async Task QueueUpsertAsync(Album album)
        {
            _albums.Add(album);
            if (_albums.Count >= 200)
            {
                await UpsertQueuedAsync();
            }
        }

        public async Task UpsertQueuedAsync()
        {
            using (_db = new Context(_dbOptions))
            {
                await _db.BulkInsertOrUpdateAsync(_albums);
                _albums.Clear();
            }
        }

        public async Task DeleteAsync(Album album)
        {
            using (_db = new Context(_dbOptions))
            {
                if (null != album)
                {
                    _ = _db.Albums.Remove(album);
                    _ = await _db.SaveChangesAsync().ConfigureAwait(false);
                }
            }
        }
    }
}
