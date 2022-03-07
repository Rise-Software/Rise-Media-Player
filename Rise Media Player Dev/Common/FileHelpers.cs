using Rise.App.Props;
using Rise.App.ViewModels;
using Rise.Common.Constants;
using Rise.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.System;
using Buffer = Windows.Storage.Streams.Buffer;

namespace Rise.App.Common
{
    public class FileHelpers
    {
        /// <summary>
        /// Saves a <see cref="SoftwareBitmap"/> to a <see cref="StorageFile"/>.
        /// </summary>
        /// <param name="softwareBitmap"><see cref="SoftwareBitmap"/> to save.</param>
        /// <param name="outputFile"><see cref="StorageFile"/> where the <see cref="SoftwareBitmap"/>
        /// should be stored.</param>
        /// <returns>Whether or not the operation was successful.</returns>
        public static async Task<bool> SaveSoftwareBitmapToFile(SoftwareBitmap softwareBitmap, StorageFile outputFile)
        {
            using IRandomAccessStream stream = await outputFile.OpenAsync(FileAccessMode.ReadWrite);
            // Create an encoder with the desired format
            BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);

            // Set the software bitmap
            encoder.SetSoftwareBitmap(softwareBitmap);
            encoder.IsThumbnailGenerated = true;

            try
            {
                await encoder.FlushAsync();
            }
            catch (Exception err)
            {
                const int WINCODEC_ERR_UNSUPPORTEDOPERATION = unchecked((int)0x88982F81);
                switch (err.HResult)
                {
                    case WINCODEC_ERR_UNSUPPORTEDOPERATION:
                        // If the encoder does not support writing a thumbnail, then try again
                        // but disable thumbnail generation.
                        encoder.IsThumbnailGenerated = false;
                        break;
                    default:
                        return false;
                }
            }

            if (encoder.IsThumbnailGenerated == false)
            {
                await encoder.FlushAsync();
            }

            return true;
        }

        /// <summary>
        /// Converts a <see cref="StorageItemThumbnail"/> to a <see cref="Buffer"/> and saves it.
        /// </summary>
        /// <param name="thumbnail"><see cref="StorageItemThumbnail"/> to convert.</param>
        /// <param name="filename">Filename of output image.</param>
        /// <returns>The image's filename. If the item has no thumbnail, returns "/".</returns>
        public static async Task<string> SaveBitmapFromThumbnailAsync(StorageItemThumbnail thumbnail, string filename)
        {
            if (thumbnail != null && thumbnail.Type == ThumbnailType.Image)
            {
                StorageFile destinationFile = await ApplicationData.Current.LocalFolder.
                    CreateFileAsync(filename, CreationCollisionOption.GenerateUniqueName);

                Buffer buffer = new(Convert.ToUInt32(thumbnail.Size));
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
        /// Converts a <see cref="StorageItemThumbnail"/> to a <see cref="Buffer"/> and saves it.
        /// </summary>
        /// <param name="thumbnail"><see cref="StorageItemThumbnail"/> to convert.</param>
        /// <param name="filename">Filename of output image.</param>
        /// <returns>The image's filename. If the item has no thumbnail, returns "/".</returns>
        public static async Task<string> SaveBitmapFromThumbnailWithReplaceAsync(StorageItemThumbnail thumbnail, string filename)
        {
            if (thumbnail != null && thumbnail.Type == ThumbnailType.Image)
            {
                StorageFile destinationFile = await ApplicationData.Current.LocalFolder.
                    CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);

                Buffer buffer = new(Convert.ToUInt32(thumbnail.Size));
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
    }

    public static class FileExtensions
    {
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

        /// <summary>
        /// Launchs an <see cref="Uri"/> from a <see cref="string"/>.
        /// </summary>
        /// <param name="str">The <see cref="Uri"/> <see cref="string"/>.</param>
        /// <returns>Whether or not the launch was successful.</returns>
        public static async Task<bool> LaunchAsync(this string str)
        {
            if (str.IsValidUri())
            {
                return await Launcher.LaunchUriAsync(new Uri(str));
            }

            return false;
        }

        /// <summary>
        /// Launchs an <see cref="Uri"/>.
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> to launch.</param>
        /// <returns>Whether or not the launch was successful.</returns>
        public static async Task<bool> LaunchAsync(this Uri uri)
            => await Launcher.LaunchUriAsync(uri);

        /// <summary>
        /// Checks whether or not the provided <see cref="string"/> is
        /// a valid <see cref="Uri"/>.
        /// </summary>
        /// <param name="str"><see cref="string"/> to check.</param>
        /// <param name="kind">The kind of <see cref="Uri"/> to check.
        /// Won't check for a specific type by default.</param>
        /// <returns>Whether or not the <see cref="string"/> is
        /// a valid <see cref="Uri"/>.</returns>
        public static bool IsValidUri(this string str,
            UriKind kind = UriKind.RelativeOrAbsolute)
            => Uri.TryCreate(str, kind, out _);

        /// <summary>
        /// Uses the provided <paramref name="baseUri"/> to make <paramref name="relativeUri"/>
        /// an absolute URI.
        /// If <paramref name="relativeUri"/> is already an absolute URI, no change is made.
        /// </summary>
        public static Uri ToAbsoluteUri(this Uri relativeUri, Uri baseUri)
        {
            return relativeUri.IsAbsoluteUri
                ? relativeUri
                : new Uri(baseUri, relativeUri);
        }

        /// <summary>
        /// Replaces characters in <c>text</c> that are not allowed in 
        /// file names with the specified replacement character.
        /// </summary>
        /// <param name="text">Text to make into a valid filename. The same string is returned if it is valid already.</param>
        /// <param name="replacement">Replacement character, or null to simply remove bad characters.</param>
        /// <returns>A string that can be used as a filename. If the output string would otherwise be empty, returns "_".</returns>
        public static string AsValidFileName(this string text, char? replacement = '_')
        {
            StringBuilder sb = new(text.Length);
            char[] invalids = _invalids ??= Path.GetInvalidFileNameChars();
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
        /// Creates a <see cref="Song"/> based on a <see cref="StorageFile"/>.
        /// </summary>
        /// <param name="file">Song file.</param>
        /// <returns>A song based on the file.</returns>
        public static async Task<Song> AsSongModelAsync(this StorageFile file)
        {
            // Put the value into memory to make sure that the system
            // really fetches the property.
            MusicProperties musicProperties =
                await file.Properties.GetMusicPropertiesAsync();

            int cd = 1;
            IDictionary<string, object> extraProps =
                await file.Properties.RetrievePropertiesAsync(Properties.DiscProperties);

            // Check if disc number is valid.
            if (extraProps[SystemMusic.DiscNumber] != null)
            {
                try
                {
                    if (int.TryParse(extraProps[SystemMusic.DiscNumber].ToString(), out int result))
                    {
                        cd = result;
                    }
                    else
                    {
                        Debug.WriteLine("Something wrong happened while parsing song.\nProblematic part of set: " + extraProps[SystemMusic.DiscNumber].ToString());
                    }
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
                    if (int.TryParse(extraProps[SystemMusic.PartOfSet].ToString(), out int result))
                    {
                        cd = result;
                    }
                    else
                    {
                        Debug.WriteLine("Something wrong happened while parsing song.\nProblematic part of set: " + extraProps[SystemMusic.PartOfSet].ToString());
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Problem: " + ex.Message);
                    Debug.WriteLine("Problematic part of set: " + extraProps[SystemMusic.PartOfSet].ToString());
                }
            }

            // Valid song metadata is needed.
            string title = musicProperties.Title.Length > 0
                ? musicProperties.Title : Path.GetFileNameWithoutExtension(file.Path);

            string artist = musicProperties.Artist.Length > 0
                ? musicProperties.Artist : "UnknownArtistResource";

            string albumTitle = musicProperties.Album.Length > 0
                ? musicProperties.Album : "UnknownAlbumResource";

            string albumArtist = musicProperties.AlbumArtist.Length > 0
                ? musicProperties.AlbumArtist : "UnknownArtistResource";

            string genre = musicProperties.Genre.FirstOrDefault() != null
                ? musicProperties.Genre.FirstOrDefault() : "UnknownGenreResource";

            TimeSpan length = musicProperties.Duration;

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
        /// Creates a <see cref="Video"/> based on a <see cref="StorageFile"/>.
        /// </summary>
        /// <param name="file">Video file.</param>
        /// <returns>A video based on the file.</returns>
        public static async Task<Video> AsVideoModelAsync(this StorageFile file)
        {
            // Put the value into memory to make sure that the system
            // really fetches the property.
            VideoProperties videoProperties =
                await file.Properties.GetVideoPropertiesAsync();

            // Valid video metadata is needed.
            string title = videoProperties.Title.Length > 0
                ? videoProperties.Title : Path.GetFileNameWithoutExtension(file.Path);

            string directors = videoProperties.Directors.Count > 0
                ? string.Join(";", videoProperties.Directors) : "UnknownArtistResource";

            return new Video
            {
                Title = title,
                Directors = directors,
                Length = videoProperties.Duration,
                Year = videoProperties.Year,
                Location = file.Path,
                Rating = videoProperties.Rating
            };
        }

        /// <summary>
        /// Creates a <see cref="Playlist"/> based on a <see cref="StorageFile"/>.
        /// </summary>
        /// <param name="file">Playlist file.</param>
        /// <returns>A playlist based on the file.</returns>
        public static async Task<PlaylistViewModel> AsPlaylistModelAsync(this StorageFile file)
        {
            // Read playlist file
            var lines = await FileIO.ReadLinesAsync(file, Windows.Storage.Streams.UnicodeEncoding.Utf8);
            PlaylistViewModel playlist = new()
            {
                Title = string.Empty,
                Description = string.Empty,
                Duration = string.Empty,
                Icon = string.Empty,
                Songs = new System.Collections.ObjectModel.ObservableCollection<SongViewModel>()
            };

            // Check if linked to directory
            if (lines.Count == 1 && Uri.TryCreate(lines[0], UriKind.RelativeOrAbsolute, out var refUri))
            {
                Uri baseUri = new(Path.GetDirectoryName(file.Path));
                string dirPath = refUri.ToAbsoluteUri(baseUri).AbsolutePath;

                if (dirPath.EndsWith(".m3u"))
                {
                    StorageFile linkedPlaylistFile = await StorageFile.GetFileFromPathAsync(dirPath);
                    return await linkedPlaylistFile.AsPlaylistModelAsync();
                }

                foreach (var songPath in Directory.EnumerateFiles(dirPath))
                {
                    playlist.Songs.Add(new SongViewModel()
                    {
                        Location = new Uri(songPath).ToAbsoluteUri(baseUri).AbsolutePath,
                    });
                }
                goto done;
            }

            // Get details
            string title = null, artist = null, icon = null;
            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i].Trim();

                if (!string.IsNullOrWhiteSpace(line))
                {
                    if (line.StartsWith("#"))
                    {
                        int splitIdx = line.IndexOf(':');
                        string prop;
                        string value = null;
                        if (splitIdx >= 0)
                        {
                            prop = line.Substring(0, splitIdx).Trim();
                            value = line.Substring(splitIdx + 1).Trim();
                        }
                        else
                        {
                            prop = line;
                        }

                        if (prop == "#EXTINF")
                        {
                            string[] inf = value.Split(new[] { ',', '-' }, 3);
                            string duration = inf[0].Trim();
                            artist = inf[1].Trim();
                            title = inf[2].Trim();
                        }
                        else if (prop == "#EXTDESC" || prop == "#DESCRIPTION")
                        {
                            playlist.Description = value;
                        }
                        else if (prop == "#EXTIMG")
                        {
                            playlist.Icon = lines[++i];
                        }
                        else if (prop == "#EXTDURATION" || prop == "#DURATION")
                        {
                            playlist.Duration = value;
                        }
                        else if (prop == "#PLAYLIST")
                        {
                            playlist.Title = value;
                        }

                        // Otherwise, we skip this line because we don't want anything from it
                        // or it's a whitespace
                    }
                    else
                    {
                        StorageFile songFile = await StorageFile.GetFileFromPathAsync(line);
                        if (songFile != null)
                        {
                            Song song = await songFile.AsSongModelAsync();

                            // If the playlist entry includes track info, override the tag data
                            if (title != null)
                            {
                                song.Title = title;
                                title = null;
                            }
                            if (artist != null)
                            {
                                song.Artist = artist;
                                artist = null;
                            }
                            if (icon != null)
                            {
                                song.Thumbnail = icon;
                                icon = null;
                            }

                            playlist.Songs.Add(new SongViewModel(song));
                        }
                    }
                }
            }

        done:
            if (string.IsNullOrWhiteSpace(playlist.Icon))
                playlist.Icon = "ms-appx://Assets/NavigationView/PlaylistsPage/blankplaylist.png";

            return playlist;
        }

        /// <summary>
        /// Creates a <see cref="MediaPlaybackItem"/> from a <see cref="StorageFile"/>.
        /// </summary>
        /// <param name="file">File to convert.</param>
        /// <returns>A <see cref="MediaPlaybackItem"/> based on the file.</returns>
        public static async Task<MediaPlaybackItem> AsSongPlaybackItemAsync(this StorageFile file)
        {
            MediaSource source = MediaSource.CreateFromStorageFile(file);
            MediaPlaybackItem media = new(source);

            Song song = await file.AsSongModelAsync();

            MediaItemDisplayProperties props = media.GetDisplayProperties();
            props.Type = MediaPlaybackType.Music;

            props.MusicProperties.Title = song.Title;
            props.MusicProperties.Artist = song.Artist;
            props.MusicProperties.AlbumTitle = song.Album;
            props.MusicProperties.AlbumArtist = song.AlbumArtist;
            props.MusicProperties.TrackNumber = song.Track;

            StorageItemThumbnail thumb =
                await file.GetThumbnailAsync(ThumbnailMode.MusicView, 134);

            props.Thumbnail = RandomAccessStreamReference.CreateFromStream(thumb);
            thumb.Dispose();

            media.ApplyDisplayProperties(props);
            return media;
        }
    }
}
