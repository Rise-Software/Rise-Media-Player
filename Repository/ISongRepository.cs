using Rise.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rise.Repository
{
    public interface ISongRepository
    {
        /// <summary>
        /// Returns all songs.
        /// </summary>
        Task<IEnumerable<Song>> GetAsync();

        /// <summary>
        /// Returns all songs with a data field matching the start of the given string. 
        /// </summary>
        Task<IEnumerable<Song>> GetAsync(string search);

        /// <summary>
        /// Returns the song with the given id. 
        /// </summary>
        Task<Song> GetAsync(Guid id);

        /// <summary>
        /// Queues a song for upserting.
        /// </summary>
        void QueueUpsert(Song song);

        /// <summary>
        /// Upserts all queued songs.
        /// </summary>
        Task UpsertQueuedAsync();

        /// <summary>
        /// Deletes a song.
        /// </summary>
        Task DeleteAsync(Song song);
    }
}
