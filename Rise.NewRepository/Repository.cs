using Rise.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace Rise.NewRepository
{
    public static class Repository
    {
        public static readonly string DbPath =
            Path.Combine(ApplicationData.Current.LocalFolder.Path, "Lists.db");

        private static SQLiteConnection _db;
        private static SQLiteAsyncConnection _asyncDb;

        private static Queue<DbObject> _upsertQueue;

        /// <summary>
        /// Initializes the database and its tables.
        /// </summary>
        public static async Task InitializeDatabaseAsync()
        {
            _ = await ApplicationData.Current.LocalFolder.CreateFileAsync("Lists.db", CreationCollisionOption.OpenIfExists);

            _db ??= new SQLiteConnection(DbPath);
            _asyncDb ??= new SQLiteAsyncConnection(DbPath);

            _ = await _asyncDb.CreateTableAsync<Song>();
            _ = await _asyncDb.CreateTableAsync<Artist>();
            _ = await _asyncDb.CreateTableAsync<Album>();
            _ = await _asyncDb.CreateTableAsync<Genre>();
            _ = await _asyncDb.CreateTableAsync<Video>();

            _upsertQueue ??= new();
        }

        /// <summary>
        /// Gets all items from the table which contains
        /// objects of the specified type.
        /// </summary>
        /// <returns>The list of items.</returns>
        public static List<T> GetItems<T>()
            where T : DbObject, new()
        {
            var table = _db.Table<T>();
            return table.ToList();
        }

        /// <summary>
        /// Gets all items from the table which contains
        /// objects of the specified type.
        /// </summary>
        /// <returns>A Task that represents the get operation.</returns>
        public static Task<List<T>> GetItemsAsync<T>()
            where T : DbObject, new()
        {
            var table = _asyncDb.Table<T>();
            return table.ToListAsync();
        }

        /// <summary>
        /// Upserts an item to the database.
        /// </summary>
        /// <returns>Amount of modified rows.</returns>
        public static int Upsert(DbObject item)
            => _db.InsertOrReplace(item);

        /// <summary>
        /// Upserts an item to the database asynchronously.
        /// </summary>
        /// <returns>A Task that represents the upsert operation,
        /// with the amount of modified rows.</returns>
        public static Task<int> UpsertAsync(DbObject item)
            => _asyncDb.InsertOrReplaceAsync(item);

        /// <summary>
        /// Queues an item to the database for upserting.
        /// </summary>
        /// <returns>A <see cref="System.Boolean" /> which provides the state of the operation.</returns>
        /// <param name="item">The DB object to queue.</param>
        public static bool QueueUpsert(DbObject item)
        {
            if (!_upsertQueue.Contains(item))
            {
                _upsertQueue.Enqueue(item);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Upserts all queued items asynchronously.
        /// </summary>
        /// <returns>A <see cref="Task" /> which represents the operation.</returns>
        public static async Task UpsertQueuedAsync()
        {
            while (_upsertQueue.TryDequeue(out DbObject item))
            {
                _ = await _asyncDb.InsertOrReplaceAsync(item);
            }
        }

        /// <summary>
        /// Removes an item from the database.
        /// </summary>
        /// <returns>Amount of rows that were removed.</returns>
        public static int Delete(DbObject item)
            => _db.Delete(item);

        /// <summary>
        /// Removes an item from the database asynchronously.
        /// </summary>
        /// <returns>A Task that represents the removal operation,
        /// with the amount of rows that were removed.</returns>
        public static Task<int> DeleteAsync(DbObject item)
            => _asyncDb.DeleteAsync(item);

        /// <summary>
        /// Gets the item with the specified Id.
        /// </summary>
        /// <typeparam name="T">Desired item type.</typeparam>
        /// <returns>The item if found, null otherwise.</returns>
        public static T GetItem<T>(Guid id, bool parallel = false) where T : DbObject, new()
        {
            if (parallel)
                return GetItems<T>().AsParallel().FirstOrDefault(i => i.Id == id);

            return GetItems<T>().FirstOrDefault(i => i.Id == id);
        }

        /// <summary>
        /// Gets the item with the specified Id asynchronously.
        /// </summary>
        /// <typeparam name="T">Desired item type.</typeparam>
        /// <returns>The item if found, null otherwise.</returns>
        public static async Task<T> GetItemAsync<T>(Guid id, bool parallel = false) where T : DbObject, new()
        {
            if (parallel)
                return (await GetItemsAsync<T>()).AsParallel().FirstOrDefault(i => i.Id == id);

            return (await GetItemsAsync<T>()).FirstOrDefault(i => i.Id == id);
        }
    }
}
