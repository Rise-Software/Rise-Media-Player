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
        public static List<Song1> UpsertQueue { get; private set; } = new List<Song1>();

        public async static Task InsertAsync(Song1 song)
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

        public async static Task UpdateAsync(Song1 song)
        {

        }

        public async static Task UpsertAsync(Song1 song)
        {
            /*
             if (Songs.Contains(song)) 
             {
                 InsertAsync(song);
             } else UpdateAsync(song);
             */
        }

        public async static Task QueueUpsertAsync(Song1 song)
        {
            if (UpsertQueue.Count >= 100)
            {
                await UpsertAsync(song);
            }
        }

        public static void Insert(Song1 song)
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