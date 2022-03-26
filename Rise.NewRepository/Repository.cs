using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Rise.NewRepository
{
    public static class Repository
    {
        private static List<SqliteCommand> _commandQueue { get; set; } = new List<SqliteCommand>();
        public static readonly string DbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Files1.db");

        public async static Task InitializeDatabaseAsync()
        {
            await ApplicationData.Current.LocalFolder.CreateFileAsync("Files1.db", CreationCollisionOption.OpenIfExists);
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Files1.db");
            using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                string songsTableCommand = "CREATE TABLE IF NOT " +
                    "EXISTS Songs (Primary_Key INTEGER PRIMARY KEY, " +
                    "Text_Entry NVARCHAR(2048) NULL)";

                string artistsTableCommand = "CREATE TABLE IF NOT " +
                    "EXISTS Artists (Primary_Key INTEGER PRIMARY KEY, " +
                    "Text_Entry NVARCHAR(2048) NULL)";

                string albumsTableCommand = "CREATE TABLE IF NOT " +
                    "EXISTS Albums (Primary_Key INTEGER PRIMARY KEY, " +
                    "Text_Entry NVARCHAR(2048) NULL)";

                string genresTableCommand = "CREATE TABLE IF NOT " +
                    "EXISTS Genres (Primary_Key INTEGER PRIMARY KEY, " +
                    "Text_Entry NVARCHAR(2048) NULL)";

                _commandQueue.Add(new SqliteCommand(songsTableCommand, db));
                _commandQueue.Add(new SqliteCommand(artistsTableCommand, db));
                _commandQueue.Add(new SqliteCommand(albumsTableCommand, db));
                _commandQueue.Add(new SqliteCommand(genresTableCommand, db));

                await DoQueuedWorkAsync();
            }
        }

        public async static Task<List<SqliteDataReader>> DoQueuedWorkAsync()
        {
            List<SqliteDataReader> list = new List<SqliteDataReader>();

            foreach (SqliteCommand command in _commandQueue)
            {
                list.Add(await command.ExecuteReaderAsync());
            }

            return list;
        }
    }
}
