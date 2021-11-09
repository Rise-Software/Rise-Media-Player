using Rise.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rise.Repository
{
    public interface IGenreRepository
    {
        /// <summary>
        /// Returns all genres.
        /// </summary>
        Task<IEnumerable<Genre>> GetAsync();

        /// <summary>
        /// Returns all genres with a data field matching the start of the given string. 
        /// </summary>
        Task<IEnumerable<Genre>> GetAsync(string search);

        /// <summary>
        /// Returns the genre with the given id. 
        /// </summary>
        Task<Genre> GetAsync(Guid id);

        /// <summary>
        /// Queues a genre for upserting.
        /// </summary>
        Task QueueUpsertAsync(Genre genre);

        /// <summary>
        /// Upserts all queued genres.
        /// </summary>
        Task UpsertQueuedAsync();

        /// <summary>
        /// Deletes a genre.
        /// </summary>
        Task DeleteAsync(Genre genre);
    }
}
