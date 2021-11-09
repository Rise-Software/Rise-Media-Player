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
        /// Queues an artist for upserting.
        /// </summary>
        Task QueueUpsertAsync(Artist artist);

        /// <summary>
        /// Upserts all queued artists.
        /// </summary>
        Task UpsertQueuedAsync();

        /// <summary>
        /// Deletes an artist.
        /// </summary>
        Task DeleteAsync(Artist artist);
    }
}
