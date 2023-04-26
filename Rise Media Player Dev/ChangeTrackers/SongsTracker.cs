using Rise.App.ViewModels;
using Rise.Common.Enums;
using Rise.Common.Extensions;
using Rise.NewRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage.Search;

namespace Rise.App.ChangeTrackers
{
    public static class SongsTracker
    {
        private static MainViewModel ViewModel => App.MViewModel;

        /// <summary>
        /// Manage changes to the music library folders.
        /// </summary>
        public static async Task CheckDuplicatesAsync()
        {
            List<SongViewModel> songDuplicates = new();
            List<ArtistViewModel> artistDuplicates = new();
            List<AlbumViewModel> albumDuplicates = new();
            List<GenreViewModel> genreDuplicates = new();

            // Check for duplicates and remove if any duplicate is found.
            for (int i = 0; i < ViewModel.Songs.Count; i++)
            {
                for (int j = i + 1; j < ViewModel.Songs.Count; j++)
                {
                    if (ViewModel.Songs[i].Location == ViewModel.Songs[j].Location)
                        songDuplicates.Add(ViewModel.Songs[j]);
                }
            }

            for (int i = 0; i < ViewModel.Artists.Count; i++)
            {
                for (int j = i + 1; j < ViewModel.Artists.Count; j++)
                {
                    if (ViewModel.Artists[i].Name.Equals(ViewModel.Artists[j].Name))
                        artistDuplicates.Add(ViewModel.Artists[j]);
                }
            }

            for (int i = 0; i < ViewModel.Albums.Count; i++)
            {
                for (int j = i + 1; j < ViewModel.Albums.Count; j++)
                {
                    if (ViewModel.Albums[i].Title.Equals(ViewModel.Albums[j].Title))
                        albumDuplicates.Add(ViewModel.Albums[j]);
                }
            }

            for (int i = 0; i < ViewModel.Genres.Count; i++)
            {
                for (int j = i + 1; j < ViewModel.Genres.Count; j++)
                {
                    if (ViewModel.Genres[i].Name.Equals(ViewModel.Genres[j].Name))
                        genreDuplicates.Add(ViewModel.Genres[j]);
                }
            }

            foreach (var song in songDuplicates)
                await ViewModel.RemoveSongAsync(song, true);

            foreach (var artist in artistDuplicates)
            {
                _ = ViewModel.Artists.Remove(artist);
                _ = Repository.QueueRemove(artist.Model);
            }

            foreach (var album in albumDuplicates)
            {
                _ = ViewModel.Albums.Remove(album);
                _ = Repository.QueueRemove(album.Model);
            }

            foreach (var genre in genreDuplicates)
            {
                _ = ViewModel.Genres.Remove(genre);
                _ = Repository.QueueRemove(genre.Model);
            }
        }

        public static async Task HandleLibraryChangesAsync(bool queue = false)
        {
            await using var changes = await App.MusicLibrary.GetLibraryChangesAsync();

            if (changes.Status != StorageLibraryChangeStatus.HasChange)
                return;

            foreach (var addedItem in changes.AddedItems)
            {
                _ = await ViewModel.SaveMusicModelsAsync(addedItem, queue);
            }

            foreach (var removedItemPath in changes.RemovedItems)
            {
                if (string.IsNullOrEmpty(removedItemPath))
                    continue;

                var song = ViewModel.Songs.FirstOrDefault(s => s.Location.Equals(removedItemPath, StringComparison.OrdinalIgnoreCase));
                if (song != null)
                    await ViewModel.RemoveSongAsync(song, queue);
            }
        }
    }
}
