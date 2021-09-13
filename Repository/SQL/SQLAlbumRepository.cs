using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Rise.Models;

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

        public async Task<IEnumerable<Album>> GetAsync(string value)
        {
            string[] parameters = value.Split(' ');
            return await _db.Albums
                .Where(album =>
                    parameters.Any(parameter =>
                        album.Title.StartsWith(parameter, StringComparison.OrdinalIgnoreCase) ||
                        album.Artist.StartsWith(parameter, StringComparison.OrdinalIgnoreCase) ||
                        album.Genre.StartsWith(parameter, StringComparison.OrdinalIgnoreCase)))
                .OrderByDescending(album =>
                    parameters.Count(parameter =>
                        album.Title.StartsWith(parameter) ||
                        album.Artist.StartsWith(parameter) ||
                        album.Genre.StartsWith(parameter)))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Album> UpsertAsync(Album album)
        {
            var current = await _db.Albums.FirstOrDefaultAsync(_album => _album.Id == album.Id);
            if (null == current)
            {
                _db.Albums.Add(album);
            }
            else
            {
                _db.Entry(current).CurrentValues.SetValues(album);
            }

            await _db.SaveChangesAsync();
            return album;
        }

        public async Task DeleteAsync(Guid id)
        {
            var album = await _db.Albums.FirstOrDefaultAsync(_album => _album.Id == id);
            if (null != album)
            {
                _db.Albums.Remove(album);
                await _db.SaveChangesAsync();
            }
        }
    }
}
