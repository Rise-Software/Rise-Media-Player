using RMP.App.Settings;
using RMP.App.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.FileProperties;
using Windows.Storage.Search;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace RMP.App
{
    public class Indexer
    {
        #region Variables
        /// <summary>
        /// Instance of Song ViewModel.
        /// </summary>
        public SongViewModel Song { get; set; }

        /// <summary>
        /// Instance of Album ViewModel.
        /// </summary>
        public AlbumViewModel Album { get; set; }

        /// <summary>
        /// List of indexed songs.
        /// </summary>
        private List<StorageFile> Songs { get; set; }

        /// <summary>
        /// Query options for song indexing.
        /// </summary>
        private readonly QueryOptions SongQueryOptions =
            new QueryOptions(CommonFileQuery.DefaultQuery,
            new string[]
            {
                ".mp3", ".wma", ".wav", ".ogg", ".flac", ".aiff", ".aac", ".m4a"
            })
            {
                FolderDepth = FolderDepth.Deep
            };

        /// <summary>
        /// List of indexed Videos.
        /// </summary>
        // private List<StorageFile> Videos { get; set; }

        /// <summary>
        /// List of extra properties to get from songs.
        /// </summary>
        private readonly List<string> SongProperties = new List<string>
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

        private ResourceLoader resourceLoader;
        #endregion

        /// <summary>
        /// Index songs in the music library and FutureAccessList.
        /// </summary>
        public async void IndexSongs()
        {
            resourceLoader = ResourceLoader.GetForViewIndependentUse("Information");

            // Init file list
            Debug.WriteLine("Started data crawl.");
            Songs = new List<StorageFile>();

            // Add music library files to list
            IReadOnlyList<StorageFile> musicLibrary = await KnownFolders.MusicLibrary.
                CreateFileQueryWithOptions(SongQueryOptions).GetFilesAsync();

            Songs.AddRange(musicLibrary);

            // Get items from future access list
            StorageItemAccessList fa = StorageApplicationPermissions.FutureAccessList;

            // Iterate the entries, get the files inside them
            foreach (AccessListEntry entry in fa.Entries)
            {
                // Get folder from future access list
                string faToken = entry.Token;
                StorageFolder folder = await fa.GetFolderAsync(faToken);

                // Query files inside folder
                try
                {
                    IReadOnlyList<StorageFile> folderContents =
                        await folder.CreateFileQueryWithOptions(SongQueryOptions).GetFilesAsync();
                    Songs.AddRange(folderContents);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }

            int filesLength = Songs.Count();
            int currentFile = 0;

            foreach (StorageFile file in Songs)
            {
                currentFile++;
                MainPage.Current.LoadingBar.Value = currentFile * 100 / filesLength;
                MainPage.Current.IndexProgress.Text = resourceLoader.GetString("Indexed") + " " +
                    currentFile + " " + resourceLoader.GetString("OutOf") + " " +
                    filesLength + " " + resourceLoader.GetString("Files") + " ";

                // Get file properties
                MusicProperties musicProperties = await file.Properties.GetMusicPropertiesAsync();
                int cd = 1;

                // Get the specified properties through StorageFile.Properties
                IDictionary<string, object> extraProperties = await file.Properties.RetrievePropertiesAsync(SongProperties);

                if (extraProperties["System.Music.DiscNumber"] != null)
                {
                    try
                    {
                        cd = int.Parse(extraProperties["System.Music.DiscNumber"].ToString());
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Problem: " + ex.Message);
                        Debug.WriteLine("Problematic disc number: " + extraProperties["System.Music.DiscNumber"].ToString());
                    }
                }
                else if (extraProperties["System.Music.PartOfSet"] != null)
                {
                    try
                    {
                        cd = int.Parse(extraProperties["System.Music.PartOfSet"].ToString());
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Problem: " + ex.Message);
                        Debug.WriteLine("Problematic part of set: " + extraProperties["System.Music.PartOfSet"].ToString());
                    }
                }

                MediaSource source = MediaSource.CreateFromStorageFile(file);
                MediaPlaybackItem media = new MediaPlaybackItem(source);

                MediaItemDisplayProperties props = media.GetDisplayProperties();
                props.Type = MediaPlaybackType.Music;

                props.MusicProperties.Title = musicProperties.Title.Length > 0
                    ? musicProperties.Title : file.DisplayName;

                props.MusicProperties.Artist = musicProperties.Artist.Length > 0
                    ? musicProperties.Artist : "Unknown Artist";

                props.MusicProperties.AlbumTitle = musicProperties.Album.Length > 0
                    ? musicProperties.Album : "Unknown Album";

                props.MusicProperties.AlbumArtist = musicProperties.AlbumArtist.Length > 0
                    ? musicProperties.AlbumArtist : "Unknown Artist";

                /* props.MusicProperties.TrackNumber = musicProperties.TrackNumber > 0
                    ? musicProperties.TrackNumber : 0; */

                props.MusicProperties.AlbumTrackCount = (uint)(cd > 1
                    ? cd : 1);

                string genre = musicProperties.Genre.FirstOrDefault() != null
                    ? musicProperties.Genre.First() : "Unknown";

                // Set SongViewModel data
                Song = new SongViewModel
                {
                    Title = props.MusicProperties.Title,
                    Artist = props.MusicProperties.Artist,
                    Track = musicProperties.TrackNumber,
                    Disc = cd,
                    Album = props.MusicProperties.AlbumTitle,
                    AlbumArtist = props.MusicProperties.AlbumArtist,
                    Genre = genre,
                    Length = musicProperties.Duration.ToString("mm\\:ss"),
                    Year = musicProperties.Year,
                    Location = file.Path,
                    Rating = musicProperties.Rating
                };

                // Get song thumbnail and make a PNG out of it
                StorageItemThumbnail thumbnail = await file.GetThumbnailAsync(ThumbnailMode.MusicView, 134);

                string filename = MakeValidFileName(Song.Album);
                await SaveBitmapFromThumbnail(thumbnail, $@"{filename}.png");

                // Set AlbumViewModel data
                Album = new AlbumViewModel
                {
                    Title = Song.Album,
                    Artist = Song.AlbumArtist,
                    Genre = Song.Genre,
                    Thumbnail = $@"{filename}.png"
                };

                // Add new data to the ViewModel
                App.ViewModel.Albums.Add(Album);
                App.ViewModel.Songs.Add(Song);
            }

            // Save changes
            MainPage.Current.IndexProgress.Text = resourceLoader.GetString("Organizing");
            App.ViewModel.Sync();

            // Dispose Song list, unnecessary now
            Songs.Clear();
            Songs.TrimExcess();
        }

        /// <summary>
        /// Convert StorageItemThumbnail to a BitmapImage and save it.
        /// </summary>
        /// <param name="thumbnail">StorageItemThumbnail to convert.</param>
        /// <param name="filename">Filename of output image.</param>
        public async Task SaveBitmapFromThumbnail(StorageItemThumbnail thumbnail, string filename)
        {
            if (thumbnail != null && thumbnail.Type == ThumbnailType.Image)
            {
                var destinationFile = await ApplicationData.Current.LocalCacheFolder.
                    CreateFileAsync(filename, CreationCollisionOption.OpenIfExists);

                Windows.Storage.Streams.Buffer MyBuffer = new Windows.Storage.Streams.
                    Buffer(Convert.ToUInt32(thumbnail.Size));

                IBuffer iBuf = await thumbnail.ReadAsync(MyBuffer,
                    MyBuffer.Capacity, InputStreamOptions.None);

                using (IRandomAccessStream strm = await
                    destinationFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    await strm.WriteAsync(iBuf);
                }
            }
        }

        /// <summary>Replaces characters in <c>text</c> that are not allowed in 
        /// file names with the specified replacement character.</summary>
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
                        sb.Append(repl);
                    }
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.Length == 0 ? "_" : changed ? sb.ToString() : text;
        }
    }
}
