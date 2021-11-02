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
        /// Adds a new song if the song does not exist, updates the 
        /// existing song otherwise.
        /// </summary>
        Task UpsertAsync(Song song);

        /// <summary>
        /// Deletes a song.
        /// </summary>
        Task DeleteAsync(Song song);
    }
}
