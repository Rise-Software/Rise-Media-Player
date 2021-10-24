using Microsoft.EntityFrameworkCore;
using Rise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rise.Repository.SQL
{
    public class SQLArtistRepository : IArtistRepository
    {
        private readonly Context _db;

        public SQLArtistRepository(Context db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Artist>> GetAsync()
        {
            return await _db.Artists
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Artist> GetAsync(Guid id)
        {
            return await _db.Artists
                .AsNoTracking()
                .FirstOrDefaultAsync(artist => artist.Id == id);
        }

        public async Task<IEnumerable<Artist>> GetAsync(string search)
        {
            string[] parameters = search.Split(' ');
            return await _db.Artists
                .Where(artist =>
                    parameters.Any(parameter =>
                        artist.Name.StartsWith(parameter, StringComparison.OrdinalIgnoreCase)))
                .OrderByDescending(artist =>
                    parameters.Count(parameter =>
                        artist.Name.StartsWith(parameter)))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task UpsertAsync(Artist artist)
        {
            Artist current = await _db.Artists.FirstOrDefaultAsync(_artist => _artist.Id == artist.Id).ConfigureAwait(false);
            if (null == current)
            {
                _ = await _db.Artists.AddAsync(artist).ConfigureAwait(false);
            }
            else
            {
                _db.Entry(current).CurrentValues.SetValues(artist);
            }

            _ = await _db.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task DeleteAsync(Artist artist)
        {
            if (null != artist)
            {
                _ = _db.Artists.Remove(artist);
                _ = await _db.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}
