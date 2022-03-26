using SQLite;
using System;
using Rise.Models;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using System.Collections.Generic;

namespace Rise.NewRepository
{
    public static class Repository
    {
        public static readonly string DbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Lists.db");

        private static SQLiteConnection _db;
        private static SQLiteAsyncConnection _asyncDb;

        public static List<Song> Songs 
        { 
            get
            {
                return _db.Table<Song>().ToList();
            }
        }

        public static List<Artist> Artists
        {
            get
            {
                return _db.Table<Artist>().ToList();
            }
        }

        public static List<Album> Albums
        {
            get
            {
                return _db.Table<Album>().ToList();
            }
        }

        public static List<Genre> Genres
        {
            get
            {
                return _db.Table<Genre>().ToList();
            }
        }

        public static List<Video> Videos
        {
            get
            {
                return _db.Table<Video>().ToList();
            }
        }

        public async static Task InitializeDatabaseAsync()
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

        #region Songs
        public async static Task<List<Song>> GetSongsAsync()
        {
            return await _asyncDb.Table<Song>().ToListAsync();
        }

        public static void Upsert(Song song)
        {
            _db.InsertOrReplace(song);
        }

        public async static Task UpsertAsync(Song song)
        {
            await _asyncDb.InsertOrReplaceAsync(song);
        }

        public static void Delete(Song song)
        {
            _db.Delete(song);
        }

        public async static Task DeleteAsync(Song song)
        {
            await _asyncDb.DeleteAsync(song);
        }

        public async static Task<Song> GetSongAsync(Guid id)
        {
            foreach (Song song in await GetSongsAsync())
            {
                if (song.Id == id)
                {
                    return song;
                }
            }

            return null;
        }
        #endregion

        #region Artists
        public async static Task<List<Artist>> GetArtistsAsync()
        {
            return await _asyncDb.Table<Artist>().ToListAsync();
        }

        public static void Upsert(Artist artist)
        {
            _db.InsertOrReplace(artist);
        }

        public async static Task UpsertAsync(Artist artist)
        {
            await _asyncDb.InsertOrReplaceAsync(artist);
        }

        public static void Delete(Artist artist)
        {
            _db.Delete(artist);
        }

        public async static Task DeleteAsync(Artist artist)
        {
            await _asyncDb.DeleteAsync(artist);
        }

        public async static Task<Artist> GetArtistAsync(Guid id)
        {
            foreach (Artist artist in await GetArtistsAsync())
            {
                if (artist.Id == id)
                {
                    return artist;
                }
            }

            return null;
        }
        #endregion

        #region Albums
        public async static Task<List<Album>> GetAlbumsAsync()
        {
            return await _asyncDb.Table<Album>().ToListAsync();
        }

        public static void Upsert(Album album)
        {
            _db.InsertOrReplace(album);
        }

        public async static Task UpsertAsync(Album album)
        {
            await _asyncDb.InsertOrReplaceAsync(album);
        }

        public static void Delete(Album album)
        {
            _db.Delete(album);
        }

        public async static Task DeleteAsync(Album album)
        {
            await _asyncDb.DeleteAsync(album);
        }

        public async static Task<Album> GetAlbumAsync(Guid id)
        {
            foreach (Album album in await GetAlbumsAsync())
            {
                if (album.Id == id)
                {
                    return album;
                }
            }

            return null;
        }
        #endregion

        #region Genres
        public async static Task<List<Genre>> GetGenresAsync()
        {
            return await _asyncDb.Table<Genre>().ToListAsync();
        }

        public static void Upsert(Genre genre)
        {
            _db.InsertOrReplace(genre);
        }

        public async static Task UpsertAsync(Genre genre)
        {
            await _asyncDb.InsertOrReplaceAsync(genre);
        }

        public static void Delete(Genre genre)
        {
            _db.Delete(genre);
        }

        public async static Task DeleteAsync(Genre genre)
        {
            await _asyncDb.DeleteAsync(genre);
        }

        public async static Task<Genre> GetGenreAsync(Guid id)
        {
            foreach (Genre genre in await GetGenresAsync())
            {
                if (genre.Id == id)
                {
                    return genre;
                }
            }

            return null;
        }
        #endregion

        #region Videos
        public async static Task<List<Video>> GetVideosAsync()
        {
            return await _asyncDb.Table<Video>().ToListAsync();
        }

        public static void Upsert(Video video)
        {
            _db.InsertOrReplace(video);
        }

        public async static Task UpsertAsync(Video video)
        {
            await _asyncDb.InsertOrReplaceAsync(video);
        }

        public static void Delete(Video video)
        {
            _db.Delete(video);
        }

        public async static Task DeleteAsync(Video video)
        {
            await _asyncDb.DeleteAsync(video);
        }

        public async static Task<Video> GetVideoAsync(Guid id)
        {
            foreach (Video video in await GetVideosAsync())
            {
                if (video.Id == id)
                {
                    return video;
                }
            }

            return null;
        }
        #endregion
    }
}
