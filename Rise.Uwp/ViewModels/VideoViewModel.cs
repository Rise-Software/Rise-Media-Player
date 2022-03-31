using Rise.Common.Interfaces;
using Rise.Data.ViewModels;
using Rise.Models;
using Rise.Repository.SQL;
using System;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Rise.App.ViewModels
{
    public class VideoViewModel : ViewModel<Video>
    {
        private ISQLRepository<Video> Repository => SQLRepository.Repository.Videos;

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="VideoViewModel"/>
        /// class that wraps a <see cref="Video"/> object.
        /// </summary>
        public VideoViewModel(Video model = null)
        {
            Model = model ?? new Video();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the video title.
        /// </summary>
        public string Title
        {
            get => Model.Title;
            set
            {
                if (Model.Title != value)
                {
                    Model.Title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        /// <summary>
        /// Gets or sets the video length.
        /// </summary>
        public TimeSpan Length
        {
            get => Model.Length;
            set
            {
                if (Model.Length != value)
                {
                    Model.Length = value;
                    OnPropertyChanged(nameof(Length));
                }
            }
        }

        /// <summary>
        /// Gets or sets the video directors.
        /// </summary>
        public string Directors
        {
            get => Model.Directors;
            set
            {
                if (Model.Directors != value)
                {
                    Model.Directors = value;
                    OnPropertyChanged(nameof(Directors));
                }
            }
        }

        /// <summary>
        /// Gets or sets the video location.
        /// </summary>
        public string Location
        {
            get => Model.Location;
            set
            {
                if (Model.Location != value)
                {
                    Model.Location = value;
                    OnPropertyChanged(nameof(Location));
                }
            }
        }

        /// <summary>
        /// Gets or sets the video thumbnail.
        /// </summary>
        public string Thumbnail
        {
            get => Model.Thumbnail;
            set
            {
                if (Model.Thumbnail != value)
                {
                    Model.Thumbnail = value;
                    OnPropertyChanged(nameof(Thumbnail));
                }
            }
        }

        /// <summary>
        /// Gets or sets the video year.
        /// </summary>
        public uint Year
        {
            get => Model.Year;
            set
            {
                if (Model.Year != value)
                {
                    Model.Year = value;
                    OnPropertyChanged(nameof(Year));
                }
            }
        }

        /// <summary>
        /// Gets or sets the video rating.
        /// </summary>
        public uint Rating
        {
            get => Model.Rating;
            set
            {
                if (Model.Rating != value)
                {
                    Model.Rating = value;
                    OnPropertyChanged(nameof(Rating));
                }
            }
        }
        #endregion

        #region Backend
        /// <summary>
        /// Saves item data to the backend.
        /// </summary>
        public async Task SaveAsync()
        {
            bool hasMatch = await Repository.CheckForMatchAsync(Model);
            if (!hasMatch)
            {
                App.MViewModel.Videos.Add(this);
                await Repository.QueueUpsertAsync(Model);
            }
            else
            {
                await Repository.UpdateAsync(Model);
            }
        }

        /// <summary>
        /// Deletes item data from the backend.
        /// </summary>
        public async Task DeleteAsync()
        {
            App.MViewModel.Videos.Remove(this);

            await Repository.DeleteAsync(Model);
        }
        #endregion

        #region Editing
        /// <summary>
        /// Enables edit mode.
        /// </summary>
        /*public async Task StartEditAsync()
        {
            StorageFile file = await StorageFile.GetFileFromPathAsync(Location);

            if (file != null)
            {
                SongPropertiesViewModel props = new SongPropertiesViewModel(this, file.DateCreated)
                {
                    FileProps = await file.GetBasicPropertiesAsync()
                };

                _ = await typeof(PropertiesPage).
                    PlaceInWindowAsync(ApplicationViewMode.Default, 380, 550, true, props);
            }
        }*/

        /// <summary>
        /// Discards any edits that have been made, restoring the original values.
        /// </summary>
        public async Task CancelEditsAsync()
        {
            Model = await Repository.GetAsync(Model.Id);
        }
        #endregion

        #region Playback
        /// <summary>
        /// Creates a <see cref="MediaPlaybackItem"/> from this <see cref="VideoViewModel"/>.
        /// </summary>
        /// <returns>A <see cref="MediaPlaybackItem"/> based on the video.</returns>
        public async Task<MediaPlaybackItem> AsPlaybackItemAsync()
        {
            StorageFile file = await StorageFile.GetFileFromPathAsync(Location);

            MediaSource source = MediaSource.CreateFromStorageFile(file);
            MediaPlaybackItem media = new(source);

            MediaItemDisplayProperties props = media.GetDisplayProperties();
            props.Type = MediaPlaybackType.Video;

            props.VideoProperties.Title = Title;
            props.VideoProperties.Subtitle = Directors;

            if (Thumbnail != null)
            {
                props.Thumbnail = RandomAccessStreamReference.
                    CreateFromUri(new Uri(Thumbnail));
            }

            media.ApplyDisplayProperties(props);
            return media;
        }

        public MediaPlaybackItem AsPlaybackItem(Uri uri)
        {
            MediaSource source = MediaSource.CreateFromUri(uri);
            MediaPlaybackItem media = new(source);

            MediaItemDisplayProperties props = media.GetDisplayProperties();
            props.Type = MediaPlaybackType.Video;

            props.VideoProperties.Title = Title;
            props.VideoProperties.Subtitle = Directors;

            if (Thumbnail != null)
            {
                props.Thumbnail = RandomAccessStreamReference.
                    CreateFromUri(new Uri(Thumbnail));
            }

            media.ApplyDisplayProperties(props);
            return media;
        }
        #endregion
    }
}
