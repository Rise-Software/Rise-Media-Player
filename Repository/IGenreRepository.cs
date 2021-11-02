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
        /// Adds a new genre if the genre does not exist, updates the 
        /// existing genre otherwise.
        /// </summary>
        Task UpsertAsync(Genre genre);

        /// <summary>
        /// Deletes a genre.
        /// </summary>
        Task DeleteAsync(Genre genre);
    }
}
