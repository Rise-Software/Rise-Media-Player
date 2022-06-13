using Rise.Common.Interfaces;
using Rise.Data.ViewModels;
using Rise.Models;
using System;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Rise.App.ViewModels
{
    public partial class VideoViewModel : ViewModel<Video>, IMediaItem
    {

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

        public bool IsOnline { get; set; }
        #endregion

        #region Backend
        /// <summary>
        /// Saves item data to the backend.
        /// </summary>
        public async Task SaveAsync(bool queue = false)
        {
            if (!App.MViewModel.Videos.Contains(this))
            {
                App.MViewModel.Videos.Add(this);
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
            if (App.MViewModel.Videos.Contains(this))
            {
                App.MViewModel.Videos.Remove(this);
                await NewRepository.Repository.DeleteAsync(Model);
            }
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
            Model = await NewRepository.Repository.GetItemAsync<Video>(Model.Id);
        }
        #endregion

        #region Playback
        /// <summary>
        /// Creates a <see cref="MediaPlaybackItem"/> from this <see cref="VideoViewModel"/>.
        /// </summary>
        /// <returns>A <see cref="MediaPlaybackItem"/> based on the video.</returns>
        public async Task<MediaPlaybackItem> AsPlaybackItemAsync()
        {
            MediaSource source;
            var uri = new Uri(Location);

            if (uri.IsFile)
            {
                StorageFile file = await StorageFile.GetFileFromPathAsync(Location);
                source = MediaSource.CreateFromStorageFile(file);
            }
            else
            {
                source = MediaSource.CreateFromUri(uri);
            }

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

    // IMediaItem implementation
    public partial class VideoViewModel : IMediaItem
    {
        string IMediaItem.Subtitle => Directors;
        string IMediaItem.ExtraInfo => Year.ToString();

        MediaPlaybackType IMediaItem.ItemType => MediaPlaybackType.Video;
    }
}
