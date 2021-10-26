using RMP.App.Common;
using RMP.App.Props;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace RMP.App.ViewModels
{
    public class SongPropertiesViewModel : BaseViewModel
    {
        private static SongViewModel Song { get; set; }
        public BasicProperties FileProps { get; set; }

        public SongPropertiesViewModel(SongViewModel song, DateTimeOffset creationDate)
        {
            Song = song;
            Created = creationDate.Date.ToString("d");
            _filename = Path.GetFileName(Location);
        }

        public string Title
        {
            get => Song.Title;
            set => Song.Title = value;
        }

        public string Artist
        {
            get => Song.Artist;
            set => Song.Artist = value;
        }

        public uint Track
        {
            get => Song.Track;
            set => Song.Track = value;
        }

        public int Disc
        {
            get => Song.Disc;
            set => Song.Disc = value;
        }

        public string Album
        {
            get => Song.Album;
            set => Song.Album = value;
        }

        public string AlbumArtist
        {
            get => Song.AlbumArtist;
            set => Song.AlbumArtist = value;
        }

        public string Genres
        {
            get => Song.Genres;
            set => Song.Genres = value;
        }

        public uint Year
        {
            get => Song.Year;
            set => Song.Year = value;
        }

        public uint Rating
        {
            get => Song.Rating;
            set => Song.Rating = value;
        }

        public string Thumbnail => Song.Thumbnail;

        public string Location => Song.Location;

        private string _filename;
        public string Filename
        {
            get => _filename;
            set => Set(ref _filename, value);
        }

        public string Extension => Path.GetExtension(Location);

        public double MBSize => FileProps.Size / (double)1000000;
        public string Size => MBSize.ToString("N2") + " MB";
        public string Created { get; set; }
        public string Modified => FileProps.DateModified.Date.ToString("d");

        public async Task<bool> SaveChangesAsync()
        {
            bool result = false;

            // Get the file. If this doesn't work, it was likely removed/moved.
            StorageFile songFile;
            try
            {
                songFile = await StorageFile.GetFileFromPathAsync(Location);
            }
            catch
            {
                return false;
            }

            if (songFile != null)
            {
                // Get song properties.
                var props = await songFile.Properties.RetrievePropertiesAsync(Properties.ViewModelProperties);

                // Apply properties.
                props["System.Title"] = Title;
                props[SystemMusic.Artist] = Artist;
                props[SystemMusic.TrackNumber] = Track;
                props[SystemMusic.AlbumTitle] = Album;
                props[SystemMusic.AlbumArtist] = AlbumArtist;
                props["System.Media.Year"] = Year;
                props["System.Rating"] = Rating;

                await songFile.RenameAsync(Filename, NameCollisionOption.GenerateUniqueName);
                Song.Location = songFile.Path;

                try
                {
                    await songFile.Properties.SavePropertiesAsync(props);
                    result = true;
                }
                catch (Exception ex)
                {
                    await Song.CancelEditsAsync();
                    Debug.WriteLine(ex.Message);
                    result = false;
                }

                await Song.SaveAsync();
            }

            return result;
        }
    }
}
