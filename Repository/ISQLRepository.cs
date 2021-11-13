using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rise.Repository
{
    public interface ISQLRepository<T> where T : class
    {
        /// <summary>
        /// Returns all items.
        /// </summary>
        Task<IEnumerable<T>> GetAsync();

        /// <summary>
        /// Returns all items with a data field matching the start of the given string. 
        /// </summary>
        Task<IEnumerable<T>> GetAsync(string search);

        /// <summary>
        /// Returns the item with the given id. 
        /// </summary>
        Task<T> GetAsync(Guid id);

        /// <summary>
        /// Queues an item for upserting.
        /// </summary>
        Task QueueUpsertAsync(T item);

        /// <summary>
        /// Upserts all queued items.
        /// </summary>
        Task UpsertQueuedAsync();

        /// <summary>
        /// Deletes an item.
        /// </summary>
        Task DeleteAsync(T item);
    }
}
