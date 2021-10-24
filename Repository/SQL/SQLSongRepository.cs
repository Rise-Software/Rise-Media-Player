using Microsoft.EntityFrameworkCore;
using Rise.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Rise.Repository.SQL
{
    public class SQLSongRepository : ISongRepository
    {
        private readonly Context _db;

        public SQLSongRepository(Context db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Song>> GetAsync()
        {
            return await _db.Songs
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Song> GetAsync(Guid id)
        {
            return await _db.Songs
                .AsNoTracking()
                .FirstOrDefaultAsync(song => song.Id == id);
        }

        public async Task<IEnumerable<Song>> GetAsync(string search)
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

        public async Task UpsertAsync(Song song)
        {
            Song current = await _db.Songs.FirstOrDefaultAsync(_song => _song.Id == song.Id).ConfigureAwait(false);
            if (null == current)
            {
                _ = await _db.Songs.AddAsync(song).ConfigureAwait(false);
                Debug.WriteLine("Upserted song to DB!");
            }
            else
            {
                _db.Entry(current).CurrentValues.SetValues(song);
            }

            _ = await _db.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task DeleteAsync(Song song)
        {
            if (null != song)
            {
                _ = _db.Songs.Remove(song);
                _ = await _db.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}
