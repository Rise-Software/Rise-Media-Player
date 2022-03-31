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
        public static readonly string DbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Lists.db");

        private static SQLiteConnection _db;
        private static SQLiteAsyncConnection _asyncDb;

        public static List<Song> Songs => _db.Table<Song>().ToList();
        public static List<Artist> Artists => _db.Table<Artist>().ToList();
        public static List<Album> Albums => _db.Table<Album>().ToList();
        public static List<Genre> Genres => _db.Table<Genre>().ToList();
        public static List<Video> Videos => _db.Table<Video>().ToList();

        public static async Task InitializeDatabaseAsync()
        {
            await ApplicationData.Current.LocalFolder.CreateFileAsync("Lists.db", CreationCollisionOption.OpenIfExists);

            _db = new SQLiteConnection(DbPath);
            _asyncDb = new SQLiteAsyncConnection(DbPath);

            await _asyncDb.CreateTableAsync<Song>();
            await _asyncDb.CreateTableAsync<Artist>();
            await _asyncDb.CreateTableAsync<Album>();
            await _asyncDb.CreateTableAsync<Genre>();
            await _asyncDb.CreateTableAsync<Video>();
        }

        public static async Task<IList<T>> GetItemsAsync<T>()
            where T : DbObject, new()
        {
            return await _asyncDb.Table<T>().ToListAsync();
        }

        public static void Upsert(DbObject item)
        {
            _db.InsertOrReplace(item);
        }

        public static async Task UpsertAsync(DbObject item)
        {
            await _asyncDb.InsertOrReplaceAsync(item);
        }

        public static void Delete(DbObject item)
        {
            _db.Delete(item);
        }

        public static async Task DeleteAsync(DbObject item)
        {
            await _asyncDb.DeleteAsync(item);
        }

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
