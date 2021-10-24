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

        public SQLAlbumRepository(Context db)
        {
            _db = db;
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

        public async Task UpsertAsync(Album album)
        {
            Album current = await _db.Albums.FirstOrDefaultAsync(_album => _album.Id == album.Id).ConfigureAwait(false);
            if (null == current)
            {
                _ = await _db.Albums.AddAsync(album).ConfigureAwait(false);
            }
            else
            {
                _db.Entry(current).CurrentValues.SetValues(album);
            }

            _ = await _db.SaveChangesAsync().ConfigureAwait(false);
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
