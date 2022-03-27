using Rise.App.Views;
using Rise.Common.Extensions;
using Rise.Common.Helpers;
using Rise.Data.ViewModels;
using Rise.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace Rise.App.ViewModels
{
    public class PlaylistViewModel : ViewModel<Playlist>
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the PlaylistViewModel class that wraps a Playlist object.
        /// </summary>
        public PlaylistViewModel(Playlist model = null)
        {
            Model = model ?? new Playlist();

            _songs ??= new();
        }

        /// <summary>
        /// Creates a <see cref="Playlist"/> based on a <see cref="StorageFile"/>.
        /// </summary>
        /// <param name="file">Playlist file.</param>
        /// <returns>A playlist based on the file.</returns>
        public static async Task<PlaylistViewModel> GetFromFileAsync(StorageFile file)
        {
            // Read playlist file
            var lines = await FileIO.ReadLinesAsync(file, Windows.Storage.Streams.UnicodeEncoding.Utf8);
            PlaylistViewModel playlist = new()
            {
                Title = string.Empty,
                Description = string.Empty,
                Duration = string.Empty,
                Icon = string.Empty
            };

            // Check if linked to directory
            if (lines.Count == 1 && Uri.TryCreate(lines[0], UriKind.RelativeOrAbsolute, out var refUri))
            {
                Uri baseUri = new(Path.GetDirectoryName(file.Path));
                string dirPath = refUri.ToAbsoluteUri(baseUri).AbsolutePath;

                if (dirPath.EndsWith(".m3u"))
                {
                    StorageFile linkedPlaylistFile = await StorageFile.GetFileFromPathAsync(dirPath);
                    return await GetFromFileAsync(linkedPlaylistFile);
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
                            var song = await Song.GetFromFileAsync(songFile);

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
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the playlist title.
        /// </summary>
        public string Title
        {
            get => Model.Title;
            set
            {
                if (value != Model.Title)
                {
                    Model.Title = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the playlist description.
        /// </summary>
        public string Description
        {
            get => Model.Description;
            set
            {
                if (value != Model.Description)
                {
                    Model.Description = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the playlist icon.
        /// </summary>
        public string Icon
        {
            get => Model.Icon;
            set
            {
                if (value != Model.Icon)
                {
                    Model.Icon = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the playlist duration.
        /// </summary>
        public string Duration
        {
            get => Model.Duration;
            set
            {
                if (value != Model.Duration)
                {
                    Model.Duration = value;
                    OnPropertyChanged(nameof(Duration));
                }
            }
        }

        private ThreadSafeCollection<SongViewModel> _songs;

        public ThreadSafeCollection<SongViewModel> Songs
        {
            get => _songs;
            set
            {
                if (_songs != value)
                {
                    _songs = value;
                }
            }
        }

        public int SongsCount => Songs.Count;
        #endregion

        #region Backend
        /// <summary>
        /// Saves item data to the backend.
        /// </summary>
        public async Task SaveAsync()
        {
            App.MViewModel.Playlists.Add(this);
            await App.PBackendController.UpsertAsync(this);
        }

        /// <summary>
        /// Deletes item data from the backend.
        /// </summary>
        public async Task DeleteAsync()
        {
            App.MViewModel.Playlists.Remove(this);
            await App.PBackendController.DeleteAsync(this);
        }

        /// <summary>
        /// Checks whether or not the item is available. If it's not,
        /// delete it.
        /// </summary>
        /*public async Task CheckAvailabilityAsync()
        {
            if (TrackCount == 0)
            {
                await DeleteAsync();
                return;
            }
        }*/
        #endregion

        #region Item management
        /// <summary>
        /// Adds a song to the playlist.
        /// </summary>
        public async Task AddSongAsync(SongViewModel song, bool newPlaylist = false)
        {
            Songs.Add(song);
            if (newPlaylist)
            {
                await SaveAsync();
            }
            else await SaveEditsAsync();
        }

        /// <summary>
        /// Removes a song to the playlist.
        /// </summary>
        public async Task RemoveSongAsync(SongViewModel song)
        {
            Songs.Remove(song);
            await SaveEditsAsync();
        }

        /// <summary>
        /// Adds multiple songs to the playlist.
        /// </summary>
        public async Task AddSongsAsync(IEnumerable<SongViewModel> songs, bool newPlaylist = false)
        {
            try
            {
                foreach (SongViewModel song in songs)
                {
                    Songs.Add(song);
                }
            }
            finally
            {
                if (newPlaylist)
                {
                    await SaveAsync();
                }
                else await SaveEditsAsync();
            }
        }

        /// <summary>
        /// Removes multiple songs from the playlist.
        /// </summary>
        public async Task RemoveSongsAsync(List<SongViewModel> songs)
        {
            try
            {
                foreach (SongViewModel song in songs)
                {
                    Songs.Remove(song);
                }
            }
            finally
            {
                await SaveEditsAsync();
            }
        }
        #endregion

        #region Editing
        public async Task StartEditAsync()
        {
            _ = await typeof(PlaylistPropertiesPage).
                ShowInApplicationViewAsync(this, 380, 550, true);
        }

        /// <summary>
        /// Saves any edits that have been made.
        /// </summary>
        public async Task SaveEditsAsync()
        {
            await App.PBackendController.DeleteAsync(this);
            await App.PBackendController.UpsertAsync(this);
        }

        /// <summary>
        /// Discards any edits that have been made, restoring the original values.
        /// </summary>
        public async Task CancelEditsAsync()
        {
            Model = (await App.PBackendController.GetAsync(Model.Id)).Model;
        }
        #endregion
    }
}
