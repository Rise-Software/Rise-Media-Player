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
        private readonly Context _db;
        private readonly List<Album> _albums;

        public SQLAlbumRepository(Context db)
        {
            _db = db;
            _albums = new List<Album>();
        }

        public async Task<IEnumerable<Album>> GetAsync()
        {
            return await _db.Albums
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Album> GetAsync(Guid id)
        {
            return await _db.Albums
                .AsNoTracking()
                .FirstOrDefaultAsync(album => album.Id == id);
        }

        public async Task<IEnumerable<Album>> GetAsync(string search)
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

        public void QueueUpsert(Album album)
            => _albums.Add(album);

        public async Task UpsertQueuedAsync()
        {
            await _db.BulkInsertOrUpdateAsync(_albums);
            _albums.Clear();
        }

        public async Task DeleteAsync(Album album)
        {
            if (null != album)
            {
                _ = _db.Albums.Remove(album);
                _ = await _db.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}
