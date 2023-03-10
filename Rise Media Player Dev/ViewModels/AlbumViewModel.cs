﻿using Rise.Common.Extensions.Markup;
using Rise.Data.ViewModels;
using Rise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rise.App.ViewModels
{
    public class AlbumViewModel : ViewModel<Album>
    {

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the AlbumViewModel class that wraps an Album object.
        /// </summary>
        public AlbumViewModel(Album model = null)
        {
            Model = model ?? new Album();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the album title.
        /// </summary>
        public string Title
        {
            get
            {
                if (Model.Title == "UnknownAlbumResource")
                    return ResourceHelper.GetString("UnknownAlbumResource");
                return Model.Title;
            }
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
        /// Gets or sets the album artist.
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
        /// Gets the album title + artist together.
        /// </summary>
        public string TitleWithArtist => $"{Title} - {Artist}";

        /// <summary>
        /// Gets or sets the album genre.
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
        /// Gets or sets the album thumbnail.
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
        /// Gets or setss the album release year.
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
                    OnPropertyChanged(nameof(LocalizedYear));
                }
            }
        }

        public string LocalizedYear
        {
            get
            {
                string year = ResourceHelper.GetString("ReleaseYearN");
                if (Year == 0)
                    return string.Format(year, ResourceHelper.GetString("Unknown"));
                return string.Format(year, Year);
            }
        }

        public int TrackCount
            => App.MViewModel.Songs.Count(s => s.Album == Model.Title);
        public string LocalizedTrackCount
            => ResourceHelper.GetLocalizedCount("Song", TrackCount);
        #endregion

        #region Backend
        /// <summary>
        /// Saves item data to the backend.
        /// </summary>
        public async Task SaveAsync(bool queue = false)
        {
            if (!App.MViewModel.Albums.Contains(this))
            {
                App.MViewModel.Albums.Add(this);
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
        public async Task DeleteAsync(bool queue = false)
        {
            if (App.MViewModel.Albums.Contains(this))
            {
                App.MViewModel.Albums.Remove(this);

                if (queue)
                    NewRepository.Repository.QueueRemove(Model);
                else
                    await NewRepository.Repository.DeleteAsync(Model);
            }

            IEnumerable<ArtistViewModel> artists = App.MViewModel.Artists.Where(a => a.Model.Name == Model.Artist);

            foreach (var artist in artists)
                await artist.CheckAvailabilityAsync(queue);
        }

        /// <summary>
        /// Checks whether or not the item is available. If it's not,
        /// delete it.
        /// </summary>
        public async Task CheckAvailabilityAsync(bool queue = false)
        {
            var trackCount = (await NewRepository.Repository.GetItemsAsync<Song>()).Count(s => s.Album == Model.Title);

            if (trackCount == 0)
                await DeleteAsync(queue);
        }
        #endregion

        #region Editing
        /// <summary>
        /// Discards any edits that have been made, restoring the original values.
        /// </summary>
        public async Task CancelEditsAsync()
        {
            Model = await NewRepository.Repository.GetItemAsync<Album>(Model.Id);
        }
        #endregion
    }
}
