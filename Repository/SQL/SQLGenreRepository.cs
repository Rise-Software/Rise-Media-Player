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
        private static Context _db;
        private readonly DbContextOptions<Context> _dbOptions;
        private readonly List<Genre> _genres;

        public SQLGenreRepository(DbContextOptions<Context> options)
        {
            _dbOptions = options;
            _genres = new List<Genre>();
        }

        public async Task<IEnumerable<Genre>> GetAsync()
        {
            using (_db = new Context(_dbOptions))
            {
                return await _db.Genres
                    .AsNoTracking()
                    .ToListAsync();
            }
        }

        public async Task<Genre> GetAsync(Guid id)
        {
            using (_db = new Context(_dbOptions))
            {
                return await _db.Genres
                    .AsNoTracking()
                    .FirstOrDefaultAsync(g => g.Id == id);
            }
        }

        public async Task<IEnumerable<Genre>> GetAsync(string search)
        {
            using (_db = new Context(_dbOptions))
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
            using (_db = new Context(_dbOptions))
            {
                await _db.BulkInsertOrUpdateAsync(_genres);
                _genres.Clear();
            }
        }

        public async Task DeleteAsync(Genre genre)
        {
            using (_db = new Context(_dbOptions))
            {
                if (null != genre)
                {
                    _ = _db.Genres.Remove(genre);
                    _ = await _db.SaveChangesAsync().ConfigureAwait(false);
                }
            }
        }
    }
}
