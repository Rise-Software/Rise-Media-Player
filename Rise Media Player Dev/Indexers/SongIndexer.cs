using Rise.Models;
using RMP.App.Props;
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
using Buffer = Windows.Storage.Streams.Buffer;

namespace RMP.App.Indexers
{
    public class SongIndexer
    {
        #region Variables
        /// <summary>
        /// Gets the app-wide MViewModel instance.
        /// </summary>
        private static MainViewModel ViewModel => App.MViewModel;

        /// <summary>
        /// Amount of songs added after indexing.
        /// </summary>
        public static int AddedSongs { get; set; }

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
        public static async Task IndexAllSongsAsync()
        {
            /*if (Settings.SetupSettings.SetupCompleted)
            {
                MainPage.Current.CheckTip.IsOpen = true;
            }*/

            AddedSongs = 0;

            // Optimize indexing performance by using the Windows Indexer
            SongQueryOptions.IndexerOption = IndexerOption.UseIndexerWhenAvailable;

            // Prefetch file properties
            SongQueryOptions.SetPropertyPrefetch(PropertyPrefetchOptions.MusicProperties, Properties.DiscProperties);
            StorageLibrary musicLibrary = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Music);

            // Index music library
            foreach (StorageFolder folder in musicLibrary.Folders)
            {
                await IndexFolderAsync(folder);
            }

            // Show information to the user
            if (AddedSongs > 0)
            {
                Debug.WriteLine("Added " + AddedSongs + " songs.");
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
        /// Index a folder's contents.
        /// </summary>
        /// <param name="folder">Folder to index.</param>
        public static async Task IndexFolderAsync(StorageFolder folder)
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
                    Song song = await CreateModelAsync(file);
                    await SaveModelsAsync(song, file);
                }

                files = await fileTask;
                index += 10;
            }
        }

        /// <summary>
        /// Creates a song based on a StorageFile.
        /// </summary>
        /// <param name="file">Song file.</param>
        /// <returns>A song based on the song model.</returns>
        public static async Task<Song> CreateModelAsync(StorageFile file)
        {
            // Put the value into memory to make sure that the system
            // really fetches the property
            MusicProperties musicProperties =
                await file.Properties.GetMusicPropertiesAsync();

            int cd = 1;
            IDictionary<string, object> extraProps =
                await file.Properties.RetrievePropertiesAsync(Properties.DiscProperties);

            // Check if disc number is valid
            if (extraProps[SystemMusic.DiscNumber] != null)
            {
                try
                {
                    cd = int.Parse(extraProps[SystemMusic.DiscNumber].ToString());
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Problem: " + ex.Message);
                    Debug.WriteLine("Problematic disc number: " + extraProps[SystemMusic.DiscNumber].ToString());
                }
            }
            else if (extraProps[SystemMusic.PartOfSet] != null)
            {
                try
                {
                    cd = int.Parse(extraProps[SystemMusic.PartOfSet].ToString());
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Problem: " + ex.Message);
                    Debug.WriteLine("Problematic part of set: " + extraProps[SystemMusic.PartOfSet].ToString());
                }
            }

            // Valid song metadata is needed
            string title = musicProperties.Title.Length > 0
                ? musicProperties.Title : file.DisplayName;

            string artist = musicProperties.Artist.Length > 0
                ? musicProperties.Artist : "UnknownArtistResource";

            string albumTitle = musicProperties.Album.Length > 0
                ? musicProperties.Album : "UnknownAlbumResource";

            string albumArtist = musicProperties.AlbumArtist.Length > 0
                ? musicProperties.AlbumArtist : "UnknownArtistResource";

            string genre = musicProperties.Genre.FirstOrDefault() != null
                ? string.Join("; ", musicProperties.Genre) : "UnknownGenreResource";

            string length = musicProperties.Duration.ToString("mm\\:ss");

            return new Song
            {
                Title = title,
                Artist = artist,
                Track = musicProperties.TrackNumber,
                Disc = cd,
                Album = albumTitle,
                AlbumArtist = albumArtist,
                Genres = genre,
                Length = length,
                Year = musicProperties.Year,
                Location = file.Path,
                Rating = musicProperties.Rating
            };
        }

        /// <summary>
        /// Saves a song to the repository and ViewModel.
        /// </summary>
        /// <param name="song">Song to add.</param>
        /// <param name="file">The song file.</param>
        public static async Task SaveModelsAsync(Song song, StorageFile file)
        {
            // Check if song exists
            bool songExists = ViewModel.Songs.
                Any(s => s.Model.Equals(song));

            // Check if album exists
            bool albumExists = ViewModel.Albums.
                Any(a => a.Model.Title == song.Album &&
                    a.Model.Artist == song.AlbumArtist);

            // Check if artist exists
            bool artistExists = ViewModel.Artists.
                Any(a => a.Model.Name == song.Artist);

            // If song isn't there already, add it to the database
            if (!songExists)
            {
                AddedSongs++;
                SongViewModel svm = new SongViewModel(song);
                await svm.SaveAsync();
            }

            // If album isn't there already, add it to the database
            if (!albumExists)
            {
                string thumb = "ms-appx:///Assets/Default.png";

                // If the album is unknown, no need to get a thumbnail
                if (song.Album != "UnknownAlbumResource")
                {
                    // Get song thumbnail and make a PNG out of it
                    StorageItemThumbnail thumbnail = await file.GetThumbnailAsync(ThumbnailMode.MusicView, 200);

                    string filename = MakeValidFileName(song.Album);
                    filename = await SaveBitmapFromThumbnailAsync(thumbnail, $@"{filename}.png");

                    if (filename != "/")
                    {
                        thumb = $@"ms-appdata:///local/{filename}.png";
                    }
                }

                // Set AlbumViewModel data
                AlbumViewModel alvm = new AlbumViewModel
                {
                    Title = song.Album,
                    Artist = song.AlbumArtist,
                    Genres = song.Genres,
                    Thumbnail = thumb
                };

                // Add new data to the MViewModel
                await alvm.SaveAsync();
            }
            else
            {
                AlbumViewModel alvm = ViewModel.Albums.
                    First(a => a.Model.Title == song.Album &&
                               a.Model.Artist == song.AlbumArtist);

                // Update album information, in case previous songs don't have it
                // and the album is known
                if (alvm.Model.Title != "UnknownAlbumResource")
                {
                    if (alvm.Model.Artist == "UnknownArtistResource")
                    {
                        alvm.Model.Artist = song.AlbumArtist;
                    }

                    if (alvm.Thumbnail == "ms-appx:///Assets/Default.png")
                    {
                        // Get song thumbnail and make a PNG out of it
                        StorageItemThumbnail thumbnail = await file.GetThumbnailAsync(ThumbnailMode.MusicView, 134);

                        string filename = MakeValidFileName(song.Album);
                        filename = await SaveBitmapFromThumbnailAsync(thumbnail, $@"{filename}.png");

                        if (filename != "/")
                        {
                            alvm.Thumbnail = $@"ms-appdata:///local/{filename}.png";
                        }
                    }
                }
            }

            // If artist isn't there already, add it to the database
            if (!artistExists)
            {
                ArtistViewModel arvm = new ArtistViewModel
                {
                    Name = song.Artist,
                    Picture = "ms-appx:///Assets/Default.png"
                };

                await arvm.SaveAsync();
            }

            // Check for the album artist as well
            artistExists = ViewModel.Artists.
                Any(a => a.Model.Name == song.Artist);

            // If album artist isn't there already, add it to the database
            if (!artistExists)
            {
                ArtistViewModel arvm = new ArtistViewModel
                {
                    Name = song.AlbumArtist,
                    Picture = "ms-appx:///Assets/Default.png"
                };

                await arvm.SaveAsync();
            }
        }

        /// <summary>
        /// Convert StorageItemThumbnail to a BitmapImage and save it.
        /// </summary>
        /// <param name="thumbnail">StorageItemThumbnail to convert.</param>
        /// <param name="filename">Filename of output image.</param>
        /// <returns>The image's filename. If the item has no thumbnail, returns "/".</returns>
        public static async Task<string> SaveBitmapFromThumbnailAsync(StorageItemThumbnail thumbnail, string filename)
        {
            if (thumbnail != null && thumbnail.Type == ThumbnailType.Image)
            {
                StorageFile destinationFile = await ApplicationData.Current.LocalFolder.
                    CreateFileAsync(filename, CreationCollisionOption.GenerateUniqueName);

                Buffer buffer = new Buffer(Convert.ToUInt32(thumbnail.Size));

                IBuffer iBuf = await thumbnail.ReadAsync(buffer,
                    buffer.Capacity, InputStreamOptions.None);

                using (IRandomAccessStream strm = await
                    destinationFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    _ = await strm.WriteAsync(iBuf);
                }

                return Path.GetFileNameWithoutExtension(destinationFile.Path);
            }

            return "/";
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
    }
}
