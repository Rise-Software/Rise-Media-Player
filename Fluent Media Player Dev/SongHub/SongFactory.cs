using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;

namespace Fluent_Media_Player_Dev.SongHub
{
    internal class SongFactory
    {
        public SongFactory()
        {
        }

        public static SongFactory Create()
        {
            return new SongFactory();
        }

        public async Task<List<string>> GetMusicFiles(string path = "")
        {
            List<string> musicFiles = new List<string>();
            // Temp implementation.

            QueryOptions queryOption = new QueryOptions
            (CommonFileQuery.OrderByTitle, new string[] { ".mp3", ".mp4", ".wma" })
            {
                FolderDepth = FolderDepth.Deep
            };

            Queue<IStorageFolder> folders = new Queue<IStorageFolder>();

            var files = await KnownFolders.MusicLibrary.CreateFileQueryWithOptions
              (queryOption).GetFilesAsync();

            foreach (var file in files)
            {
                musicFiles.Add(file.Name);
            }

            return new List<string>(musicFiles);
        }
    }
}