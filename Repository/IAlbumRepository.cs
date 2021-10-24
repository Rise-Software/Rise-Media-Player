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
        /// Adds a new album if the album does not exist, updates the 
        /// existing album otherwise.
        /// </summary>
        Task UpsertAsync(Album album);

        /// <summary>
        /// Deletes an album.
        /// </summary>
        Task DeleteAsync(Album album);
    }
}
