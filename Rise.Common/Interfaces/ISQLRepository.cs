using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rise.Common.Interfaces
{
    public interface ISQLRepository<T> where T : class
    {
        /// <summary>
        /// Returns all items.
        /// </summary>
        Task<IEnumerable<T>> GetAsync();

        /// <summary>
        /// Returns all items with a data field matching the start of the
        /// given string. 
        /// </summary>
        Task<IEnumerable<T>> GetAsync(string search);

        /// <summary>
        /// Returns the item with the given id. 
        /// </summary>
        Task<T> GetAsync(Guid id);

        /// <summary>
        /// Directly updates an item.
        /// </summary>
        Task UpdateAsync(T item);

        /// <summary>
        /// Directly upserts an item.
        /// </summary>
        Task UpsertAsync(T item);

        /// <summary>
        /// Queues an item for upserting.
        /// </summary>
        /// <remarks>If a high amount of items are queued, call
        /// <see cref="UpsertQueuedAsync"/> to prevent excessive
        /// RAM usage.</remarks>
        Task QueueUpsertAsync(T item);

        /// <summary>
        /// Upserts all queued items.
        /// </summary>
        Task UpsertQueuedAsync();

        /// <summary>
        /// Directly deletes an item.
        /// </summary>
        Task DeleteAsync(T item);

        /// <summary>
        /// Queues an item for deletion.
        /// </summary>
        /// <remarks>If a high amount of items are queued, call
        /// <see cref="DeleteQueuedAsync"/> to prevent excessive
        /// RAM usage.</remarks>
        Task QueueDeletionAsync(T item);

        /// <summary>
        /// Deletes all queued items.
        /// </summary>
        Task DeleteQueuedAsync();
    }
}
