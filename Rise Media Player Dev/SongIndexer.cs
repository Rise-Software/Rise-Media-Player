using RMP.App.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Search;
using Windows.Storage.Streams;
using Windows.UI.Xaml;

namespace RMP.App
{
    public class SongIndexer
    {
        #region Variables
        /// <summary>
        /// Instance of Song ViewModel.
        /// </summary>
        public static SongViewModel Song { get; set; }

        public static int AddedSongs { get; set; }

        /// <summary>
        /// Instance of Album ViewModel.
        /// </summary>
        public static AlbumViewModel Album { get; set; }

        /// <summary>
        /// Query options for song indexing.
        /// </summary>
        public static readonly QueryOptions SongQueryOptions =
            new QueryOptions(CommonFileQuery.OrderByDate,
            new string[]
            {
                ".mp3", ".wma", ".wav", ".ogg", ".flac", ".aiff", ".aac", ".m4a"
            })
            {
                FolderDepth = FolderDepth.Deep
            };

        /// <summary>
        /// List of extra properties to get from songs.
        /// </summary>
        public static readonly List<string> SongProperties = new List<string>
        {
            "System.Music.DiscNumber",
            "System.Music.PartOfSet"
        };

        /// <summary>
        /// List of invalid characters in a filename.
        /// </summary>
        private static char[] _invalids = new char[] { '"', '<', '>', '|',
            '\0', '\x0001', '\x0002', '\x0003', '\x0004', '\x0005',
            '\x0006', '\a', '\b', '\t', '\n', '\v', '\f', '\r',
            '\x000e', '\x000f', '\x0010', '\x0011', '\x0012',
            '\x0013', '\x0014', '\x0015', '\x0016', '\x0017',
            '\x0018', '\x0019', '\x001a', '\x001b', '\x001c',
            '\x001d', '\x001e', '\x001f', ':', '*', '?', '\\', '/' };
        #endregion

        /// <summary>
        /// Index songs in the user's library.
        /// </summary>
        public static async Task IndexAllSongs()
        {
            /*if (Settings.SetupSettings.SetupCompleted)
            {
                MainPage.Current.CheckTip.IsOpen = true;
            }*/

            AddedSongs = 0;

            // Optimize indexing performance by using the Windows Indexer
            SongQueryOptions.IndexerOption = IndexerOption.UseIndexerWhenAvailable;

            // Prefetch file properties
            SongQueryOptions.SetPropertyPrefetch(PropertyPrefetchOptions.MusicProperties, SongProperties);

            // Index music library
            await IndexFolder(KnownFolders.MusicLibrary);

            // Show information to the user
            if (AddedSongs > 0)
            {
                // App.ViewModel.Sync();
            }

            /*if (Settings.SetupSettings.SetupCompleted)
            {
                MainPage.Current.CheckTip.IsOpen = false;
                if (AddedSongs > 0)
                {
                    MainPage.Current.AddedTip.IsOpen = true;
                }
                else
                {
                    MainPage.Current.NoNewTip.IsOpen = true;
                }
            }*/
        }

        /// <summary>
        /// Convert StorageItemThumbnail to a BitmapImage and save it.
        /// </summary>
        /// <param name="thumbnail">StorageItemThumbnail to convert.</param>
        /// <param name="filename">Filename of output image.</param>
        public static async Task SaveBitmapFromThumbnail(StorageItemThumbnail thumbnail, string filename)
        {
            if (thumbnail != null && thumbnail.Type == ThumbnailType.Image)
            {
                StorageFile destinationFile = await ApplicationData.Current.LocalFolder.
                    CreateFileAsync(filename, CreationCollisionOption.OpenIfExists);

                Windows.Storage.Streams.Buffer MyBuffer = new Windows.Storage.Streams.
                    Buffer(Convert.ToUInt32(thumbnail.Size));

                IBuffer iBuf = await thumbnail.ReadAsync(MyBuffer,
                    MyBuffer.Capacity, InputStreamOptions.None);

                using (IRandomAccessStream strm = await
                    destinationFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    _ = await strm.WriteAsync(iBuf);
                }
            }
        }

        /// <summary>
        /// Replaces characters in <c>text</c> that are not allowed in 
        /// file names with the specified replacement character.
        /// </summary>
        /// <param name="text">Text to make into a valid filename. The same string is returned if it is valid already.</param>
        /// <param name="replacement">Replacement character, or null to simply remove bad characters.</param>
        /// <returns>A string that can be used as a filename. If the output string would otherwise be empty, returns "_".</returns>
        public static string MakeValidFileName(string text, char? replacement = '_')
        {
            StringBuilder sb = new StringBuilder(text.Length);
            char[] invalids = _invalids ?? (_invalids = Path.GetInvalidFileNameChars());
            bool changed = false;

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (invalids.Contains(c))
                {
                    changed = true;
                    char repl = replacement ?? '\0';
                    if (repl != '\0')
                    {
                        _ = sb.Append(repl);
                    }
                }
                else
                {
                    _ = sb.Append(c);
                }
            }

            return sb.Length == 0 ? "_" : changed ? sb.ToString() : text;
        }

        /// <summary>
        /// Add song to the database.
        /// </summary>
        /// <param name="file">File to add.</param>
        public static async Task AddSong(StorageFile file)
        {
            // Put the value into memory to make sure that the system
            // really fetches the property
            MusicProperties musicProperties =
                await file.Properties.GetMusicPropertiesAsync();

            int cd = 1;
            IDictionary<string, object> extraProps =
                await file.Properties.RetrievePropertiesAsync(SongProperties);

            // Check if disc number is valid
            if (extraProps["System.Music.DiscNumber"] != null)
            {
                try
                {
                    cd = int.Parse(extraProps["System.Music.DiscNumber"].ToString());
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Problem: " + ex.Message);
                    Debug.WriteLine("Problematic disc number: " + extraProps["System.Music.DiscNumber"].ToString());
                }
            }
            else if (extraProps["System.Music.PartOfSet"] != null)
            {
                try
                {
                    cd = int.Parse(extraProps["System.Music.PartOfSet"].ToString());
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Problem: " + ex.Message);
                    Debug.WriteLine("Problematic part of set: " + extraProps["System.Music.PartOfSet"].ToString());
                }
            }

            // Valid song metadata is needed
            string title = musicProperties.Title.Length > 0
                ? musicProperties.Title : file.DisplayName;

            string artist = musicProperties.Artist.Length > 0
            ? musicProperties.Artist : "Unknown Artist";

            string albumTitle = musicProperties.Album.Length > 0
                ? musicProperties.Album : "Unknown Album";

            string albumArtist = musicProperties.AlbumArtist.Length > 0
                ? musicProperties.AlbumArtist : "Unknown Artist";

            string genre = musicProperties.Genre.FirstOrDefault() != null
                ? musicProperties.Genre.First() : "Unknown";

            string length = musicProperties.Duration.ToString("mm\\:ss");

            // Check if song exists
            bool songExists = App.ViewModel.Songs.
                Any(s => s.Title == title &&
                s.Artist == artist &&
                s.Track == musicProperties.TrackNumber &&
                s.Disc == cd &&
                s.Album == albumTitle &&
                s.AlbumArtist == albumArtist &&
                s.Genre == genre &&
                s.Length == length &&
                s.Year == musicProperties.Year &&
                s.Rating == musicProperties.Rating);

            // If song isn't there already, add it to the database
            if (!songExists)
            {
                AddedSongs++;

                // Set SongViewModel data
                Song = new SongViewModel
                {
                    Title = title,
                    Artist = artist,
                    Track = musicProperties.TrackNumber,
                    Disc = cd,
                    Album = albumTitle,
                    AlbumArtist = albumArtist,
                    Genre = genre,
                    Length = length,
                    Year = musicProperties.Year,
                    Location = file.Path,
                    Rating = musicProperties.Rating
                };

                App.ViewModel.Songs.Add(Song);

                // Check if album exists
                bool albumExists = App.ViewModel.Albums.
                    Any(a => a.Title == Song.Album &&
                    a.Artist == Song.AlbumArtist);

                // If album isn't there already, add it to the database
                if (!albumExists)
                {
                    // Get song thumbnail and make a PNG out of it
                    StorageItemThumbnail thumbnail = await file.GetThumbnailAsync(ThumbnailMode.MusicView, 134);

                    string filename = MakeValidFileName(Song.Album);
                    await SaveBitmapFromThumbnail(thumbnail, $@"{filename}.png");

                    string thumb = "ms-appx:///Assets/Default.png";

                    IStorageItem resultingItem = await ApplicationData.Current.LocalFolder.
                        TryGetItemAsync($@"{filename}.png");

                    // If the file doesn't exist, use default thumbnail
                    if (resultingItem != null)
                    {
                        thumb = $@"ms-appdata:///local/{filename}.png";
                    }

                    // Set AlbumViewModel data
                    Album = new AlbumViewModel
                    {
                        Title = Song.Album,
                        Artist = Song.AlbumArtist,
                        Genre = Song.Genre,
                        SongCount = 1,
                        Thumbnail = thumb
                    };

                    // Add new data to the ViewModel
                    App.ViewModel.Albums.Add(Album);
                }
                else
                {
                    AlbumViewModel album = App.ViewModel.Albums.
                        First(a => a.Title == Song.Album &&
                        a.Artist == Song.AlbumArtist);
                    album.SongCount++;
                }
            }
        }

        /// <summary>
        /// Index a folder's contents and add the result to the ViewModel and database.
        /// </summary>
        /// <param name="folder">Folder to index.</param>
        public static async Task IndexFolder(StorageFolder folder)
        {
            // Prepare the query
            StorageFileQueryResult folderQueryResult = folder.CreateFileQueryWithOptions(SongQueryOptions);

            // Index by steps
            uint index = 0, stepSize = 10;
            IReadOnlyList<StorageFile> files = await folderQueryResult.GetFilesAsync(index, stepSize);
            index += 10;

            // Start crawling data
            while (files.Count != 0)
            {
                Task<IReadOnlyList<StorageFile>> fileTask =
                    folderQueryResult.GetFilesAsync(index, stepSize).AsTask();

                // Process files
                foreach (StorageFile file in files)
                {
                    await AddSong(file);
                }

                files = await fileTask;
                index += 10;
            }
        }
    }
}
