using Rise.App.Helpers;
using Rise.Models;
using System;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Rise.App.ViewModels
{
    public class VideoViewModel : ViewModel<Video>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VideoViewModel"/>
        /// class that wraps a <see cref="Video"/> object.
        /// </summary>
        public VideoViewModel(Video model = null)
        {
            Model = model ?? new Video();
            IsNew = true;
        }

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

        private SoftwareBitmap _thumbnail;
        /// <summary>
        /// The video thumbnail as a <see cref="SoftwareBitmap"/>.
        /// </summary>
        public SoftwareBitmap Thumbnail
        {
            get
            {
                if (_thumbnail == null)
                {
                    bool res = App.MViewModel.VideoThumbnailDictionary.
                        TryGetValue(Model.Id.ToString(), out var thumbnail);

                    if (res)
                    {
                        _thumbnail = thumbnail;
                    }
                }

                return _thumbnail;
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

        private bool _isNew;
        /// <summary>
        /// Gets or sets a value that indicates whether this is a new item.
        /// </summary>
        public bool IsNew
        {
            get => _isNew;
            set => Set(ref _isNew, value);
        }

        private bool _isInEdit;
        /// <summary>
        /// Gets or sets a value that indicates whether the item data is being edited.
        /// </summary>
        public bool IsInEdit
        {
            get => _isInEdit;
            set => Set(ref _isInEdit, value);
        }

        /// <summary>
        /// Saves video data that has been edited.
        /// </summary>
        public async Task SaveAsync()
        {
            if (IsNew)
            {
                IsNew = false;
                App.MViewModel.Videos.Add(this);
            }

            await App.Repository.Videos.QueueUpsertAsync(Model);
        }

        /// <summary>
        /// Creates a <see cref="MediaPlaybackItem"/> from this <see cref="VideoViewModel"/>.
        /// </summary>
        /// <returns>A <see cref="MediaPlaybackItem"/> based on the video.</returns>
        public async Task<MediaPlaybackItem> AsPlaybackItemAsync()
        {
            StorageFile file = await StorageFile.GetFileFromPathAsync(Location);

            MediaSource source = MediaSource.CreateFromStorageFile(file);
            MediaPlaybackItem media = new MediaPlaybackItem(source);

            MediaItemDisplayProperties props = media.GetDisplayProperties();
            props.Type = MediaPlaybackType.Video;

            props.VideoProperties.Title = Title;
            props.VideoProperties.Subtitle = Directors;

            if (Thumbnail != null)
            {
                props.Thumbnail = RandomAccessStreamReference.
                    CreateFromStream(await Thumbnail.GetStreamAsync());
            }

            media.ApplyDisplayProperties(props);
            return media;
        }
    }
}
