using Rise.App.ViewModels;
using Rise.Common;
using Rise.Common.Constants;
using Rise.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;

namespace Rise.App.ChangeTrackers
{
    public static class SongsTracker
    {
        public static MainViewModel ViewModel => App.MViewModel;

        public static async void MusicQueryResultChanged(IStorageQueryResultBase sender, object args)
            => await HandleLibraryChangesAsync();

        public static async Task<StorageLibraryChangeResult> GetLibraryChangesAsync(this StorageLibrary library)
        {
            var changeTracker = library.ChangeTracker;

            // Ensure that the change tracker is always enabled.
            changeTracker.Enable();

            var changeReader = changeTracker.GetChangeReader();
            var changes = await changeReader.ReadBatchAsync();

            ulong lastChangeId = changeReader.GetLastChangeId();

            var addedItems = new List<StorageFile>();
            var removedItems = new List<string>();

            if (lastChangeId == StorageLibraryLastChangeId.Unknown)
            {
                changeTracker.Reset();
                return new StorageLibraryChangeResult(StorageLibraryChangeStatus.Unknown);
            }

            foreach (StorageLibraryChange change in changes)
            {
                if (change.ChangeType == StorageLibraryChangeType.ChangeTrackingLost ||
                    !change.IsOfType(StorageItemTypes.File))
                {
                    changeTracker.Reset();
                    return new StorageLibraryChangeResult(StorageLibraryChangeStatus.Unknown);
                }

                switch (change.ChangeType)
                {
                    case StorageLibraryChangeType.MovedIntoLibrary:
                    case StorageLibraryChangeType.Created:
                        {
                            StorageFile file = (StorageFile)await change.GetStorageItemAsync();

                            if (!SupportedFileTypes.MediaFiles.Contains(file.FileType.ToLowerInvariant()))
                                continue;

                            addedItems.Add(file);
                            break;
                        }

                    case StorageLibraryChangeType.MovedOutOfLibrary:
                    case StorageLibraryChangeType.Deleted:
                        {
                            removedItems.Add(change.PreviousPath);
                            break;
                        }

                    case StorageLibraryChangeType.MovedOrRenamed:
                    case StorageLibraryChangeType.ContentsChanged:
                    case StorageLibraryChangeType.ContentsReplaced:
                        {
                            StorageFile file = (StorageFile)await change.GetStorageItemAsync();

                            if (!SupportedFileTypes.MediaFiles.Contains(file.FileType.ToLowerInvariant()))
                                continue;

                            removedItems.Add(change.PreviousPath ?? file.Path);
                            addedItems.Add(file);
                            break;
                        }

                    case StorageLibraryChangeType.IndexingStatusChanged:
                    case StorageLibraryChangeType.EncryptionChanged:
                    case StorageLibraryChangeType.ChangeTrackingLost:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return new StorageLibraryChangeResult(changeReader, addedItems, removedItems);
        }

        /// <summary>
        /// Manage changes to the music library folders.
        /// </summary>
        public static async Task CheckDuplicatesAsync(CancellationToken token = default)
        {
            if (token.IsCancellationRequested)
                return;

            List<SongViewModel> songDuplicates = new();
            List<ArtistViewModel> artistDuplicates = new();
            List<AlbumViewModel> albumDuplicates = new();
            List<GenreViewModel> genreDuplicates = new();

            // Check for duplicates and remove if any duplicate is found.
            for (int i = 0; i < ViewModel.Songs.Count; i++)
            {
                if (token.IsCancellationRequested)
                    return;

                for (int j = i + 1; j < ViewModel.Songs.Count; j++)
                {
                    if (token.IsCancellationRequested)
                        return;

                    if (ViewModel.Songs[i].Location == ViewModel.Songs[j].Location)
                        songDuplicates.Add(ViewModel.Songs[j]);
                }
            }

            for (int i = 0; i < ViewModel.Artists.Count; i++)
            {
                if (token.IsCancellationRequested)
                    return;

                for (int j = i + 1; j < ViewModel.Artists.Count; j++)
                {
                    if (token.IsCancellationRequested)
                        return;

                    if (ViewModel.Artists[i].Name.Equals(ViewModel.Artists[j].Name))
                        artistDuplicates.Add(ViewModel.Artists[j]);
                }
            }

            for (int i = 0; i < ViewModel.Albums.Count; i++)
            {
                if (token.IsCancellationRequested)
                    return;

                for (int j = i + 1; j < ViewModel.Albums.Count; j++)
                {
                    if (token.IsCancellationRequested)
                        return;

                    if (ViewModel.Albums[i].Title.Equals(ViewModel.Albums[j].Title))
                        albumDuplicates.Add(ViewModel.Albums[j]);
                }
            }

            for (int i = 0; i < ViewModel.Genres.Count; i++)
            {
                if (token.IsCancellationRequested)
                    return;

                for (int j = i + 1; j < ViewModel.Genres.Count; j++)
                {
                    if (token.IsCancellationRequested)
                        return;

                    if (ViewModel.Genres[i].Name.Equals(ViewModel.Genres[j].Name))
                        genreDuplicates.Add(ViewModel.Genres[j]);
                }
            }

            foreach (SongViewModel song in songDuplicates)
            {
                if (token.IsCancellationRequested)
                    return;

                await song.DeleteAsync(true);
            }

            foreach (ArtistViewModel artist in artistDuplicates)
            {
                if (token.IsCancellationRequested)
                    return;

                await artist.DeleteAsync(true);
            }

            foreach (AlbumViewModel album in albumDuplicates)
            {
                if (token.IsCancellationRequested)
                    return;

                await album.DeleteAsync(true);
            }

            foreach (GenreViewModel genre in genreDuplicates)
            {
                if (token.IsCancellationRequested)
                    return;

                await genre.DeleteAsync(true);
            }
        }

        public static async Task HandleLibraryChangesAsync()
        {
            var changes = await App.MusicLibrary.GetLibraryChangesAsync();

            if (changes.Status != StorageLibraryChangeStatus.HasChange)
                return;

            foreach (var addedItem in changes.AddedItems)
            {
                _ = await ViewModel.SaveMusicModelsAsync(addedItem, true);
            }

            foreach (var removedItemPath in changes.RemovedItems)
            {
                if (string.IsNullOrEmpty(removedItemPath))
                    continue;

                var song = App.MViewModel.Songs.FirstOrDefault(s => s.Location.Equals(removedItemPath, StringComparison.OrdinalIgnoreCase));

                if (song == null)
                    continue;

                await song.DeleteAsync(true);
            }
        }
    }
}
