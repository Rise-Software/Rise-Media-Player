using Rise.App.Props;
using Rise.App.ViewModels;
using Rise.Common.Constants;
using Rise.Common.Extensions;
using Rise.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;

namespace Rise.App.Common
{
    public static class FileExtensions
    {
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
    }
}
