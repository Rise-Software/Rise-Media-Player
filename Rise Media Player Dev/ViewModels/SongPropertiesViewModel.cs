using Rise.Common.Constants;
using Rise.Common.Extensions;
using Rise.Data.ViewModels;
using Rise.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace Rise.App.ViewModels
{
    public class SongPropertiesViewModel : ViewModel<SongViewModel>
    {
        public BasicProperties FileProps { get; set; }

        public SongPropertiesViewModel(SongViewModel song, DateTimeOffset creationDate)
        {
            Model = song;
            Created = creationDate.Date.ToString("d");
            _filename = Path.GetFileName(Location);
        }

        public string Title
        {
            get => Model.Title;
            set => Model.Title = value;
        }

        public string Artist
        {
            get => Model.Artist;
            set => Model.Artist = value;
        }

        public uint Track
        {
            get => Model.Track;
            set => Model.Track = value;
        }

        public int Disc
        {
            get => Model.Disc;
            set => Model.Disc = value;
        }

        public uint Bitrate => Model.Bitrate;

        public string Album
        {
            get => Model.Album;
            set => Model.Album = value;
        }

        public string AlbumArtist
        {
            get => Model.AlbumArtist;
            set => Model.AlbumArtist = value;
        }

        public string Genres
        {
            get => Model.Genres;
            set => Model.Genres = value;
        }

        public uint Year
        {
            get => Model.Year;
            set => Model.Year = value;
        }

        public uint Rating
        {
            get => Model.Rating / 20;
            set => Model.Rating = value * 20;
        }

        public string Thumbnail
        {
            get => Model.Thumbnail;
            set => Model.Thumbnail = value;
        }

        public string Location => Model.Location;

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
                // Get WinRT music properties
                var musicProps = await songFile.Properties.GetMusicPropertiesAsync();

                musicProps.Title = Title;
                musicProps.Artist = Artist;
                musicProps.TrackNumber = Track;
                musicProps.Album = Album;
                musicProps.AlbumArtist = AlbumArtist;
                musicProps.Year = Year;
                musicProps.Rating = Rating * 20;

                foreach (var genre in Genres.Split("; "))
                    _ = musicProps.Genre.AddIfNotExists(genre);

                try
                {
                    await musicProps.SavePropertiesAsync();

                    result = true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);

                    await Model.CancelEditsAsync();
                    result = false;
                }
                finally
                {
                    // The rename operation will likely complete either way
                    await songFile.RenameAsync(Filename, NameCollisionOption.GenerateUniqueName);
                    Model.Location = songFile.Path;

                    var ogSong = await NewRepository.Repository.GetItemAsync<Song>(Model.Model.Id);
                    await NewRepository.Repository.DeleteAsync(ogSong);

                    await Model.SaveAsync();
                }
            }

            return result;
        }
    }
}
