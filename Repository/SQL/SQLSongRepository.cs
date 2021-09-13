using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Rise.Models;

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

        public async Task<IEnumerable<Song>> GetAsync(string value)
        {
            string[] parameters = value.Split(' ');
            return await _db.Songs
                .Where(song =>
                    parameters.Any(parameter =>
                        song.Title.StartsWith(parameter, StringComparison.OrdinalIgnoreCase) ||
                        song.Artist.StartsWith(parameter, StringComparison.OrdinalIgnoreCase) ||
                        song.Genre.StartsWith(parameter, StringComparison.OrdinalIgnoreCase) ||
                        song.Location.StartsWith(parameter, StringComparison.OrdinalIgnoreCase)))
                .OrderByDescending(song =>
                    parameters.Count(parameter =>
                        song.Title.StartsWith(parameter) ||
                        song.Artist.StartsWith(parameter) ||
                        song.Genre.StartsWith(parameter) ||
                        song.Location.StartsWith(parameter)))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Song> UpsertAsync(Song song)
        {
            var current = await _db.Songs.FirstOrDefaultAsync(_song => _song.Id == song.Id);
            if (null == current)
            {
                _db.Songs.Add(song);
            }
            else
            {
                _db.Entry(current).CurrentValues.SetValues(song);
            }

            await _db.SaveChangesAsync();
            return song;
        }

        public async Task DeleteAsync(string location)
        {
            var song = await _db.Songs.FirstOrDefaultAsync(_song => _song.Location == location);
            if (null != song)
            {
                _db.Songs.Remove(song);
                await _db.SaveChangesAsync();
            }
        }
    }
}
