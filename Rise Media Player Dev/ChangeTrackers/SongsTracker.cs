using Rise.App.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Rise.Common.Extensions;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;
using System.IO;
using System.Threading;
using System.Linq;
using Rise.Common;

namespace Rise.App.ChangeTrackers
{
    public class SongsTracker
    {
        /// <summary>
        /// Gets the app-wide MViewModel instance.
        /// </summary>
        private static MainViewModel ViewModel => App.MViewModel;

        public static async void MusicQueryResultChanged(IStorageQueryResultBase sender, object args)
        {
            sender.ContentsChanged -= MusicQueryResultChanged;

            Debug.WriteLine("New changes!");
            StorageFolder changedFolder = sender.Folder;
            StorageLibraryChangeTracker folderTracker = changedFolder.TryGetChangeTracker();

            if (folderTracker != null)
            {
                folderTracker.Enable();

                StorageLibraryChangeReader changeReader = folderTracker.GetChangeReader();
                IReadOnlyList<StorageLibraryChange> changes = await changeReader.ReadBatchAsync();

                foreach (StorageLibraryChange change in changes)
                {
                    if (change.ChangeType == StorageLibraryChangeType.ChangeTrackingLost)
                    {
                        // Change tracker is in an invalid state and must be reset
                        // This should be a very rare case, but must be handled
                        folderTracker.Reset();
                        await ViewModel.StartFullCrawlAsync();
                        return;
                    }

                    if (change.IsOfType(StorageItemTypes.File))
                    {
                        await ManageSongChangeAsync(change);
                    }
                    else if (change.IsOfType(StorageItemTypes.Folder))
                    {
                        //await ViewModel.StartFullCrawlAsync();
                    }
                    else
                    {
                        if (change.ChangeType == StorageLibraryChangeType.Deleted)
                        {
                            foreach (SongViewModel song in ViewModel.Songs)
                            {
                                if (change.PreviousPath == song.Location)
                                {
                                    await song.DeleteAsync();
                                }
                            }
                        }
                    }
                }

                // Mark that all the changes have been seen and for the change tracker
                // to never return these changes again
                await changeReader.AcceptChangesAsync();

                sender.ContentsChanged += MusicQueryResultChanged;
            }
        }

        public static async Task HandleLibraryChangesAsync()
        {
            Debug.WriteLine("New changes!");
            StorageLibraryChangeTracker folderTracker = App.MusicLibrary.ChangeTracker;

            if (folderTracker != null)
            {
                StorageLibraryChangeReader changeReader = folderTracker.GetChangeReader();
                IReadOnlyList<StorageLibraryChange> changes = await changeReader.ReadBatchAsync();

                foreach (StorageLibraryChange change in changes)
                {
                    if (change.ChangeType == StorageLibraryChangeType.ChangeTrackingLost)
                    {
                        // Change tracker is in an invalid state and must be reset.
                        // This should be a very rare case, but must be handled.
                        folderTracker.Reset();
                        await ViewModel.StartFullCrawlAsync();
                        return;
                    }

                    if (change.IsOfType(StorageItemTypes.File))
                    {
                        await ManageSongChangeAsync(change);
                    }
                    else if (change.IsOfType(StorageItemTypes.Folder))
                    {
                        await ManageFolderChangeAsync(change);
                    }
                    else
                    {
                        if (change.ChangeType == StorageLibraryChangeType.Deleted)
                        {
                            var songOccurrences = ViewModel.Songs.Where(s => s.Location == change.PreviousPath);

                            foreach (var song in songOccurrences)
                                await song.DeleteAsync();
                        }
                    }
                }

                // Mark that all the changes have been seen and for the change tracker
                // to never return these changes again
                await changeReader.AcceptChangesAsync();
            }
        }

        /// <summary>
        /// Manage changes to a song using the <see cref="StorageLibraryChange" /> provided.
        /// </summary>
        /// <param name="change">Change that occurred.</param>
        public static async Task ManageSongChangeAsync(StorageLibraryChange change)
        {
            StorageFile file;

            switch (change.ChangeType)
            {
                // New File in the Library
                case StorageLibraryChangeType.Created:
                    // Song was created..?
                    file = (StorageFile)await change.GetStorageItemAsync();
                    await ViewModel.SaveMusicModelsAsync(file);
                    break;

                case StorageLibraryChangeType.MovedIntoLibrary:
                    // Song was moved into the library
                    file = (StorageFile)await change.GetStorageItemAsync();
                    await ViewModel.SaveMusicModelsAsync(file);
                    break;

                case StorageLibraryChangeType.MovedOrRenamed:
                    // Song was renamed/moved
                    file = (StorageFile)await change.GetStorageItemAsync();
                    for (int i = 0; i < ViewModel.Songs.Count; i++)
                    {
                        if (change.PreviousPath == ViewModel.Songs[i].Location)
                        {
                            ViewModel.Songs[i].Location = file.Path;
                            await ViewModel.Songs[i].SaveAsync();
                        }
                    }
                    break;

                // File Removed From Library
                case StorageLibraryChangeType.Deleted:
                    // Song was deleted
                    for (int i = 0; i < ViewModel.Songs.Count; i++)
                    {
                        if (change.PreviousPath == ViewModel.Songs[i].Location)
                        {
                            await ViewModel.Songs[i].DeleteAsync();
                        }
                    }
                    break;

                case StorageLibraryChangeType.MovedOutOfLibrary:
                    // Song got moved out of the library
                    for (int i = 0; i < ViewModel.Songs.Count; i++)
                    {
                        if (change.PreviousPath == ViewModel.Songs[i].Location)
                        {
                            await ViewModel.Songs[i].DeleteAsync();
                        }
                    }
                    break;

                // Modified Contents
                case StorageLibraryChangeType.ContentsChanged:
                    // Song content was modified..?
                    file = (StorageFile)await change.GetStorageItemAsync();
                    for (int i = 0; i < ViewModel.Songs.Count; i++)
                    {
                        if (change.PreviousPath == ViewModel.Songs[i].Location)
                        {
                            await ViewModel.Songs[i].DeleteAsync();
                            await ViewModel.SaveMusicModelsAsync(file);
                        }
                    }
                    break;

                // Ignored Cases
                case StorageLibraryChangeType.EncryptionChanged:
                case StorageLibraryChangeType.ContentsReplaced:
                case StorageLibraryChangeType.IndexingStatusChanged:
                default:
                    // These are safe to ignore, I think
                    break;
            }
        }

        /// <summary>
        /// Manage changes to a folder in the music library using the <see cref="StorageLibraryChange" /> provided.
        /// </summary>
        /// <param name="change">Change that occurred.</param>
        public static async Task ManageFolderChangeAsync(StorageLibraryChange change)
        {
            StorageFolder folder = await change.GetStorageItemAsync() as StorageFolder;

            switch (change.ChangeType)
            {
                case StorageLibraryChangeType.MovedIntoLibrary:
                    await foreach (var file in folder.IndexAsync(QueryPresets.SongQueryOptions))
                        await ViewModel.SaveMusicModelsAsync(file);
                    break;
                case StorageLibraryChangeType.MovedOutOfLibrary:
                case StorageLibraryChangeType.Deleted:
                    for (int i = 0; i < ViewModel.Songs.Count; i++)
                    {
                        string folderPath = Path.GetDirectoryName(ViewModel.Songs[i].Location);

                        if (change.PreviousPath == folderPath)
                            await ViewModel.Songs[i].DeleteAsync();
                    }
                    break;
                case StorageLibraryChangeType.MovedOrRenamed:
                    for (int i = 0; i < ViewModel.Songs.Count; i++)
                    {
                        string folderPath = Path.GetDirectoryName(ViewModel.Songs[i].Location);

                        if (change.PreviousPath == folderPath)
                        {
                            ViewModel.Songs[i].Location = Path.Combine(folder.Path, ViewModel.Songs[i].FileName);
                            await ViewModel.Songs[i].SaveAsync();
                        }
                    }
                    break;
            }
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
    }
}
