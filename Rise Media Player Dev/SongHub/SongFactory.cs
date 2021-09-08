using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;

namespace RMP.App.SongHub
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

        public async Task<List<OfflineSong>> GetMusicFiles(string path = "")
        {
            // Temp implementation for grabbing music files from user music library folder.
            // Note: Things can change anytime.
            List<OfflineSong> musicFiles = new List<OfflineSong>();

            try
            {
                QueryOptions queryOption = new QueryOptions
                (CommonFileQuery.OrderByTitle, new string[] { ".mp3", ".m4a", ".wma", ".aac", ".wav", ".flac" })
                {
                    FolderDepth = FolderDepth.Deep
                };

                IReadOnlyList<StorageFile> musicLibraryfiles = await KnownFolders.MusicLibrary.CreateFileQueryWithOptions
                  (queryOption).GetFilesAsync();

                foreach (StorageFile file in musicLibraryfiles)
                {
                    musicFiles.Add(OfflineSong.Create(file.Name, file.Path));
                }
            }

            catch (Exception exception)
            {
                Console.WriteLine("Error ", exception.Message);
            }

            return new List<OfflineSong>(musicFiles);
        }
    }
}