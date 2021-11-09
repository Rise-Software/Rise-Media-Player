using Rise.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rise.Repository
{
    public interface IAlbumRepository
    {
        /// <summary>
        /// Returns all albums.
        /// </summary>
        Task<IEnumerable<Album>> GetAsync();

        /// <summary>
        /// Returns all albums with a data field matching the start of the given string. 
        /// </summary>
        Task<IEnumerable<Album>> GetAsync(string search);

        /// <summary>
        /// Returns the album with the given id. 
        /// </summary>
        Task<Album> GetAsync(Guid id);

        /// <summary>
        /// Queues an album for upserting.
        /// </summary>
        Task QueueUpsertAsync(Album album);

        /// <summary>
        /// Upserts all queued albums.
        /// </summary>
        Task UpsertQueuedAsync();

        /// <summary>
        /// Deletes a song.
        /// </summary>
        Task DeleteAsync(Album album);
    }
}
