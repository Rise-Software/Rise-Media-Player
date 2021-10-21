using Rise.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rise.Repository
{
    public interface IArtistRepository
    {
        /// <summary>
        /// Returns all artists.
        /// </summary>
        Task<IEnumerable<Artist>> GetAsync();

        /// <summary>
        /// Returns all artists with a data field matching the start of the given string. 
        /// </summary>
        Task<IEnumerable<Artist>> GetAsync(string search);

        /// <summary>
        /// Returns the artist with the given id. 
        /// </summary>
        Task<Artist> GetAsync(Guid id);

        /// <summary>
        /// Adds a new artist if the artist does not exist, updates the 
        /// existing artist otherwise.
        /// </summary>
        Task UpsertAsync(Artist artist);

        /// <summary>
        /// Deletes an artist.
        /// </summary>
        Task DeleteAsync(Artist artist);
    }
}
