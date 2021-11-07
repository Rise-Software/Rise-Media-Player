using Rise.Models;
using RMP.App.Common;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Windows.Data.Xml.Dom;

namespace RMP.App.ViewModels
{
    public class ArtistViewModel : BaseViewModel
    {
        // private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        /// <summary>
        /// Initializes a new instance of the ArtistViewModel class that wraps an Artist object.
        /// </summary>
        public ArtistViewModel(Artist model = null)
        {
            Model = model ?? new Artist();
            IsNewArtist = true;
        }

        private Artist _model;

        /// <summary>
        /// Gets or sets the underlying Artist object.
        /// </summary>
        public Artist Model
        {
            get => _model;
            set
            {
                if (_model != value)
                {
                    _model = value;

                    // Raise the PropertyChanged event for all properties.
                    OnPropertyChanged(string.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the artist name.
        /// </summary>
        public string Name
        {
            get
            {
                if (Model.Name == "UnknownArtistResource")
                {
                    return ResourceLoaders.MediaDataLoader.GetString("UnknownArtistResource");
                }

                return Model.Name;
            }
            set
            {
                if (value != Model.Name)
                {
                    Model.Name = value;
                    IsModified = true;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        /// <summary>
        /// Gets or sets the artist picture.
        /// </summary>
        public string Picture
        {
            get => Model.Picture;
            set
            {
                if (value != Model.Picture)
                {
                    Model.Picture = value;
                    IsModified = true;
                    OnPropertyChanged(nameof(Picture));
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether or not the
        /// item has to be removed.
        /// </summary>
        public bool Removed
        {
            get => Model.Removed;
            private set
            {
                if (value != Model.Removed)
                {
                    Model.Removed = value;
                    IsModified = true;
                    OnPropertyChanged(string.Empty);
                }
            }
        }

        public async Task<string> GetPictureAsync()
        {
            string name = HttpUtility.UrlEncode(Name);
            string xml = null;

            try
            {
                xml = await WebHelpers.
                    CreateGETRequestAsync(URLs.MusicBrainz + "artist/?query=artist:" + name);
            }
            catch
            {
                return "ms-appx:///Assets/Default.png";
            }

            if (xml == null)
            {
                return "ms-appx:///Assets/Default.png";
            }

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            XmlNodeList nodes = doc.GetElementsByTagName("artist");

            string id = "";
            foreach (IXmlNode node in nodes)
            {
                XmlNamedNodeMap attrs = node.Attributes;
                IXmlNode idAttr = attrs.GetNamedItem("id");

                if (idAttr != null)
                {
                    id = idAttr.InnerText;
                    break;
                }
            }

            if (id != "")
            {
                xml = await WebHelpers.
                    CreateGETRequestAsync(URLs.MusicBrainz + "artist/" + id + "?inc=url-rels");

                doc.LoadXml(xml);

                nodes = doc.GetElementsByTagName("relation");

                string img;
                foreach (IXmlNode node in nodes)
                {
                    XmlNamedNodeMap attrs = node.Attributes;
                    IXmlNode type = attrs.GetNamedItem("type");

                    if (type.InnerText == "image")
                    {
                        img = node.FirstChild.InnerText;

                        string path = await WebHelpers.SaveImageFromURLAsync(img, $@"{name}.png");
                        Debug.WriteLine(path);

                        if (path != "/")
                        {
                            return $@"ms-appdata:///local/{path}.png";
                        }
                    }
                }
            }

            return "ms-appx:///Assets/Default.png";
        }

        /// <summary>
        /// Gets or sets the artist's song count.
        /// </summary>
        public int SongCount => App.MViewModel.Songs.Count(s => s.Model.Artist == Model.Name);
        public string Songs => SongCount.ToString() + " " + ResourceLoaders.MediaDataLoader.GetString("Songs");

        /// <summary>
        /// Gets or sets the artist's album count.
        /// </summary>
        public int AlbumCount => App.MViewModel.Albums.Count(a => a.Model.Artist == Model.Name);
        public string Albums => AlbumCount.ToString() + " " + ResourceLoaders.MediaDataLoader.GetString("Albums");

        /// <summary>
        /// Gets or sets a value that indicates whether the underlying model has been modified. 
        /// </summary>
        /// <remarks>
        /// Used to reduce load and only upsert the models that have changed.
        /// </remarks>
        public bool IsModified { get; set; }
        private bool _isLoading;

        /// <summary>
        /// Gets or sets a value that indicates whether to show a progress bar. 
        /// </summary>
        public bool IsLoading
        {
            get => _isLoading;
            set => Set(ref _isLoading, value);
        }

        private bool _isNewArtist;

        /// <summary>
        /// Gets or sets a value that indicates whether this is a new artist.
        /// </summary>
        public bool IsNewArtist
        {
            get => _isNewArtist;
            set => Set(ref _isNewArtist, value);
        }

        private bool _isInEdit = false;

        /// <summary>
        /// Gets or sets a value that indicates whether the artist data is being edited.
        /// </summary>
        public bool IsInEdit
        {
            get => _isInEdit;
            set => Set(ref _isInEdit, value);
        }

        /// <summary>
        /// Saves artist data that has been edited.
        /// </summary>
        public async Task SaveAsync()
        {
            IsInEdit = false;
            IsModified = false;
            Removed = false;

            if (IsNewArtist)
            {
                IsNewArtist = false;
                App.MViewModel.Artists.Add(this);
            }

            Picture = await GetPictureAsync();
            await App.Repository.Artists.UpsertAsync(Model).ConfigureAwait(false);
        }

        /// <summary>
        /// Checks whether or not the artist is available. If it's not,
        /// delete it.
        /// </summary>
        public async Task CheckAvailability()
        {
            if (SongCount == 0 && AlbumCount == 0)
            {
                await DeleteAsync();
                return;
            }
            Removed = false;
        }

        /// <summary>
        /// Delete artist from repository and MViewModel.
        /// </summary>
        public async Task DeleteAsync()
        {
            IsModified = true;
            Removed = true;

            App.MViewModel.Artists.Remove(this);
            await App.Repository.Artists.UpsertAsync(Model).ConfigureAwait(false);
            Debug.WriteLine("Artist removed!");
        }

        /// <summary>
        /// Raised when the user cancels the changes they've made to the artist data.
        /// </summary>
        public event EventHandler AddNewArtistCanceled;

        /// <summary>
        /// Cancels any in progress edits.
        /// </summary>
        public async Task CancelEditsAsync()
        {
            if (IsNewArtist)
            {
                AddNewArtistCanceled?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                await RevertChangesAsync();
            }
        }

        /// <summary>
        /// Discards any edits that have been made, restoring the original values.
        /// </summary>
        public async Task RevertChangesAsync()
        {
            IsInEdit = false;
            if (IsModified)
            {
                await RefreshArtistsAsync();
                IsModified = false;
            }
        }

        /// <summary>
        /// Enables edit mode.
        /// </summary>
        public void StartEdit() => IsInEdit = true;

        /// <summary>
        /// Reloads all of the artist data.
        /// </summary>
        public async Task RefreshArtistsAsync()
        {
            Model = await App.Repository.Artists.GetAsync(Model.Id);
        }

        /// <summary>
        /// Called when a bound DataGrid control causes the artist to enter edit mode.
        /// </summary>
        public void BeginEdit()
        {
            // Not used.
        }

        /// <summary>
        /// Called when a bound DataGrid control cancels the edits that have been made to an artist.
        /// </summary>
        public async void CancelEdit() => await CancelEditsAsync();

        /// <summary>
        /// Called when a bound DataGrid control commits the edits that have been made to an artist.
        /// </summary>
        public async void EndEdit() => await SaveAsync();
    }
}
