using Rise.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
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

        public static List<Song> Songs => _db.Table<Song>().ToList();
        public static List<Artist> Artists => _db.Table<Artist>().ToList();
        public static List<Album> Albums => _db.Table<Album>().ToList();
        public static List<Genre> Genres => _db.Table<Genre>().ToList();
        public static List<Video> Videos => _db.Table<Video>().ToList();

        /// <summary>
        /// Initializes the database and its tables.
        /// </summary>
        public static async Task InitializeDatabaseAsync()
        {
            _ = await ApplicationData.Current.LocalFolder.CreateFileAsync("Lists.db", CreationCollisionOption.OpenIfExists);

            _db = new SQLiteConnection(DbPath);
            _asyncDb = new SQLiteAsyncConnection(DbPath);

            _ = await _asyncDb.CreateTableAsync<Song>();
            _ = await _asyncDb.CreateTableAsync<Artist>();
            _ = await _asyncDb.CreateTableAsync<Album>();
            _ = await _asyncDb.CreateTableAsync<Genre>();
            _ = await _asyncDb.CreateTableAsync<Video>();
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
        {
            return _db.InsertOrReplace(item);
        }

        /// <summary>
        /// Upserts an item to the database asynchronously.
        /// </summary>
        /// <returns>A Task that represents the upsert operation,
        /// with the amount of modified rows.</returns>
        public static Task<int> UpsertAsync(DbObject item)
        {
            return _asyncDb.InsertOrReplaceAsync(item);
        }

        /// <summary>
        /// Removes an item from the database.
        /// </summary>
        /// <returns>Amount of rows that were removed.</returns>
        public static int Delete(DbObject item)
        {
            return _db.Delete(item);
        }

        /// <summary>
        /// Removes an item from the database asynchronously.
        /// </summary>
        /// <returns>A Task that represents the removal operation,
        /// with the amount of rows that were removed.</returns>
        public static Task<int> DeleteAsync(DbObject item)
        {
            return _asyncDb.DeleteAsync(item);
        }

        /// <summary>
        /// Gets the item with the specified Id.
        /// </summary>
        /// <typeparam name="T">Desired item type.</typeparam>
        /// <returns>The item if found, null otherwise.</returns>
        public static T GetItem<T>(Guid id)
            where T : DbObject, new()
        {
            foreach (var item in GetItems<T>())
            {
                if (item.Id == id)
                {
                    return item;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the item with the specified Id asynchronously.
        /// </summary>
        /// <typeparam name="T">Desired item type.</typeparam>
        /// <returns>The item if found, null otherwise.</returns>
        public static async Task<T> GetItemAsync<T>(Guid id)
            where T : DbObject, new()
        {
            foreach (var item in await GetItemsAsync<T>())
            {
                if (item.Id == id)
                {
                    return item;
                }
            }

            return null;
        }
    }
}
