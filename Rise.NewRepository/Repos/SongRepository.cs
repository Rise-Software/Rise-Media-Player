using Microsoft.Data.Sqlite;
using Rise.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace Rise.NewRepository.Repos
{
    public static class SongRepository
    {
        public static List<Song> UpsertQueue { get; private set; } = new List<Song>();

        public async static Task InsertAsync(Song song)
        {
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Files1.db");
            using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                SqliteCommand insertCommand = new SqliteCommand
                {
                    Connection = db,
                    CommandText = "INSERT INTO Songs VALUES (NULL, @Entry);"
                };

                insertCommand.Parameters.AddWithValue("@Entry", song);

                await insertCommand.ExecuteReaderAsync();

                db.Close();
            }
        }

        public async static Task UpdateAsync(Song song)
        {

        }

        public async static Task UpsertAsync(Song song)
        {
            /*
             if (Songs.Contains(song)) 
             {
                 InsertAsync(song);
             } else UpdateAsync(song);
             */
        }

        public async static Task QueueUpsertAsync(Song song)
        {
            if (UpsertQueue.Count >= 100)
            {
                await UpsertAsync(song);
            }
        }

        public static void Insert(Song song)
        {
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Files1.db");
            using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                SqliteCommand insertCommand = new SqliteCommand
                {
                    Connection = db,
                    CommandText = "INSERT INTO Songs VALUES (NULL, @Entry);"
                };

                insertCommand.Parameters.AddWithValue("@Entry", song);

                insertCommand.ExecuteReader();

                db.Close();
            }
        }
    }
}