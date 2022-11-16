﻿using Microsoft.Toolkit.Uwp.UI.Controls.TextToolbarSymbols;
using Newtonsoft.Json;
using Rise.App.Helpers;
using Rise.Common.Constants;
using Rise.Common.Extensions;
using Rise.Common.Extensions.Markup;
using Rise.Common.Helpers;
using Rise.Common.Interfaces;
using Rise.Data.ViewModels;
using Rise.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TagLib;
using Windows.Media.Playback;
using Windows.Storage;

namespace Rise.App.ViewModels
{
    public partial class SongViewModel : ViewModel<Song>, IMediaItem
    {

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the SongViewModel class that wraps a Song object.
        /// </summary>
        public SongViewModel(Song model = null)
        {
            Model = model ?? new Song();

            OnPropertyChanged(nameof(AlbumViewModel.TrackCount));
            OnPropertyChanged(nameof(ArtistViewModel.SongCount));
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the song title.
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
        /// Gets or sets the song artist.
        /// </summary>
        public string Artist
        {
            get
            {
                if (Model.Artist == "UnknownArtistResource")
                    return ResourceHelper.GetString("UnknownArtistResource");
                return Model.Artist;
            }
            set
            {
                if (value != Model.Artist)
                {
                    Model.Artist = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the song track.
        /// </summary>
        public uint Track
        {
            get => Model.Track;
            set
            {
                if (value != Model.Track)
                {
                    Model.Track = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the song disc.
        /// </summary>
        public int Disc
        {
            get => Model.Disc;
            set
            {
                if (value != Model.Disc)
                {
                    Model.Disc = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the song bitrate.
        /// </summary>
        public uint Bitrate => Model.Bitrate / 1000;

        /// <summary>
        /// Gets or sets the song album. 
        /// </summary>
        public string Album
        {
            get
            {
                if (Model.Album == "UnknownAlbumResource")
                    return ResourceHelper.GetString("UnknownAlbumResource");
                return Model.Album;
            }
            set
            {
                if (value != Model.Album)
                {
                    Model.Album = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the song album artist. 
        /// </summary>
        public string AlbumArtist
        {
            get
            {
                if (Model.AlbumArtist == "UnknownArtistResource")
                    return ResourceHelper.GetString("UnknownArtistResource");
                return Model.AlbumArtist;
            }
            set
            {
                if (value != Model.AlbumArtist)
                {
                    Model.AlbumArtist = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the song genres.
        /// </summary>
        public string Genres
        {
            get
            {
                if (Model.Genres == "UnknownGenreResource")
                    return ResourceHelper.GetString("UnknownGenreResource");
                return Model.Genres;
            }
            set
            {
                if (value != Model.Genres)
                {
                    Model.Genres = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the song length. 
        /// </summary>
        public TimeSpan Length
        {
            get => Model.Length;
            set
            {
                if (value != Model.Length)
                {
                    Model.Length = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the song year.
        /// </summary>
        public uint Year
        {
            get => Model.Year;
            set
            {
                if (value != Model.Year)
                {
                    Model.Year = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the song location. 
        /// </summary>
        public string Location
        {
            get => Model.Location;
            set
            {
                if (value != Model.Location)
                {
                    Model.Location = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the song filename.
        /// </summary>
        [JsonIgnore]
        public string FileName => Path.GetFileName(Location);

        /// <summary>
        /// Gets the song extension.
        /// </summary>
        [JsonIgnore]
        public string Extension => Path.GetExtension(Location);

        /// <summary>
        /// Gets or sets the song rating.
        /// </summary>
        public uint Rating
        {
            get => Model.Rating;
            set
            {
                if (value != Model.Rating)
                {
                    Model.Rating = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the song album's thumbnail.
        /// </summary>
        public string Thumbnail
        {
            get => Model.Thumbnail;
            set
            {
                if (value != Model.Thumbnail)
                {
                    Model.Thumbnail = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Fetches lyrics for the song.
        /// </summary>
        /// <param name="fromOnlineService">Specifies whether to fetch lyrics from Musixmatch or from file metadata.</param>
        /// <returns>A string with the song lyrics.</returns>
        public async Task<string> GetLyricsAsync(bool fromOnlineService = true)
        {
            if (fromOnlineService)
            {
                try
                {
                    var lyrics = await MusixmatchHelper.GetLyricsAsync(Title, Artist);

                    if (lyrics == null)
                        return null;

                    var lyricsString = lyrics.Message.Body.Lyrics.LyricsBody;

                    lyricsString += lyrics;
                    lyricsString += "\n\n";
                    lyricsString += lyrics.Message.Body.Lyrics.LyricsCopyright;
                    lyricsString += "\nPowered by Musixmatch.";

                    return lyricsString;
                }
                catch (Exception e)
                {
                    e.WriteToOutput();
                    return null;
                }
            } else
            {
                try
                {
                    var file = await StorageFile.GetFileFromPathAsync(Location);
                    var taglibFile = await Task.Run(() => TagLib.File.Create(new UwpStorageFileAbstraction(file)));

                    return (await Task.Run(() => taglibFile.Tag)).Lyrics;
                } catch (Exception e)
                {
                    e.WriteToOutput();
                    return null;
                }
            }
        }
        #endregion

        #region Backend
        /// <summary>
        /// Saves item data to the backend.
        /// </summary>
        public async Task SaveAsync(bool queue = false)
        {
            if (!App.MViewModel.Songs.Contains(this))
            {
                App.MViewModel.Songs.Add(this);
            }

            if (queue)
            {
                NewRepository.Repository.QueueUpsert(Model);
            }
            else
            {
                await NewRepository.Repository.UpsertAsync(Model);
            }
        }

        /// <summary>
        /// Deletes item data from the backend.
        /// </summary>
        public async Task DeleteAsync()
        {
            App.MViewModel.Songs.Remove(this);

            await NewRepository.Repository.DeleteAsync(Model);

            AlbumViewModel album = App.MViewModel.Albums.
                FirstOrDefault(a => a.Model.Title == Model.Album &&
                           a.Model.Artist == Model.AlbumArtist);

            if (album != null)
            {
                await album.CheckAvailabilityAsync();
            }

            ArtistViewModel artist = App.MViewModel.Artists.
                FirstOrDefault(a => a.Model.Name == Model.Artist);

            if (artist != null)
            {
                await artist.CheckAvailabilityAsync();
            }
        }
        #endregion

        #region Editing
        /// <summary>
        /// Discards any edits that have been made, restoring the original values.
        /// </summary>
        public async Task CancelEditsAsync()
        {
            Model = await NewRepository.Repository.GetItemAsync<Song>(Model.Id);
        }

        /// <summary>
        /// Edits lyrics of a song in the file metadata.
        /// </summary>
        /// <param name="lyrics">The lyrics to save.</param>
        /// <returns>A task which represents the operation.</returns>
        public async Task<bool> SaveLyricsAsync(string lyrics)
        {
            try
            {
                var file = await StorageFile.GetFileFromPathAsync(Location);
                var taglibFile = await Task.Run(() => TagLib.File.Create(new UwpStorageFileAbstraction(file)));

                var tag = await Task.Run(() => taglibFile.Tag);

                tag.Lyrics = lyrics;
                await Task.Run(() => taglibFile.Save());

                return true;
            } catch (Exception e)
            {
                e.WriteToOutput();
            }

            return false;
        }
        #endregion

        #region Playback
        /// <summary>
        /// Creates a <see cref="MediaPlaybackItem"/> from this <see cref="SongViewModel"/>.
        /// </summary>
        /// <returns>A <see cref="MediaPlaybackItem"/> based on the song.</returns>
        public async Task<MediaPlaybackItem> AsPlaybackItemAsync()
        {
            var uri = new Uri(Location);
            if (uri.IsFile)
            {
                var file = await StorageFile.GetFileFromPathAsync(Location);
                return await file.GetSongAsync();
            }

            return WebHelpers.GetSongFromUri(uri, Title, Artist, Thumbnail);
        }
        #endregion
    }

    public static class SongViewModelExtensions
    {
        public static Task<SongViewModel> AsSongAsync(this MediaPlaybackItem item)
        {
            var displayProps = item.GetDisplayProperties();

            var song = new SongViewModel
            {
                Title = displayProps.MusicProperties.Title,
                Artist = displayProps.MusicProperties.Artist,
                Album = displayProps.MusicProperties.AlbumTitle,
                AlbumArtist = displayProps.MusicProperties.AlbumArtist,
                Genres = string.Join(";", displayProps.MusicProperties.Genres),
                Location = item.Source.Uri.ToString(),
                Length = (TimeSpan)item.Source.Duration,
                Thumbnail = URIs.MusicThumb
            };

            return Task.FromResult(song);
        }
    }
}
