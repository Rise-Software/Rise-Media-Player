using EFCore.BulkExtensions;
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
        private readonly List<Genre> _genres;

        public SQLGenreRepository(Context db)
        {
            _db = db;
            _genres = new List<Genre>();
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

        public async Task QueueUpsertAsync(Genre genre)
        {
            _genres.Add(genre);
            if (_genres.Count >= 200)
            {
                await UpsertQueuedAsync();
            }
        }

        public async Task UpsertQueuedAsync()
        {
            await _db.BulkInsertOrUpdateAsync(_genres);
            _genres.Clear();
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
