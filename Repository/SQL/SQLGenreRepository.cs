using Microsoft.EntityFrameworkCore;
using Rise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rise.Repository.SQL
{
    public class SQLGenreRepository : IGenreRepository
    {
        private readonly Context _db;

        public SQLGenreRepository(Context db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Genre>> GetAsync()
        {
            return await _db.Genres
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Genre> GetAsync(Guid id)
        {
            return await _db.Genres
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<IEnumerable<Genre>> GetAsync(string search)
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

        public async Task UpsertAsync(Genre genre)
        {
            Artist current = await _db.Artists.FirstOrDefaultAsync(_genre => _genre.Id == genre.Id).ConfigureAwait(false);
            if (null == current)
            {
                _ = await _db.Genres.AddAsync(genre).ConfigureAwait(false);
            }
            else
            {
                _db.Entry(current).CurrentValues.SetValues(genre);
            }

            _ = await _db.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task DeleteAsync(Genre genre)
        {
            if (null != genre)
            {
                _ = _db.Genres.Remove(genre);
                _ = await _db.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}
