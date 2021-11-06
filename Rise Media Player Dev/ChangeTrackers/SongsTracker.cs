using Rise.Models;
using RMP.App.Indexers;
using RMP.App.Props;
using RMP.App.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Search;

namespace RMP.App.ChangeTrackers
{
    public class SongsTracker
    {
        /// <summary>
        /// Gets the app-wide MViewModel instance.
        /// </summary>
        private static MainViewModel ViewModel => App.MViewModel;

        /// <summary>
        /// Sets up the filesystem tracker for music library.
        /// </summary>
        public static async Task SetupMusicTracker()
        {
            StorageFolder music = KnownFolders.MusicLibrary;

            // Create a query containing all the files the app will be tracking
            QueryOptions musicOption = SongIndexer.SongQueryOptions;

            // Optimize indexing performance by using the Windows Indexer
            musicOption.IndexerOption = IndexerOption.UseIndexerWhenAvailable;

            // Prefetch file properties
            musicOption.SetPropertyPrefetch(PropertyPrefetchOptions.MusicProperties,
                Properties.DiscProperties);

            StorageFileQueryResult musicResultSet =
              music.CreateFileQueryWithOptions(musicOption);

            // Indicate to the system the app is ready to change track
            _ = await musicResultSet.GetFilesAsync(0, 1);

            // Attach an event handler for when something changes on the system
            musicResultSet.ContentsChanged += MusicLibrary_ContentsChanged;
            App.MusicLibrary.DefinitionChanged += MusicLibrary_DefinitionChanged;
            Debug.WriteLine("Registered music tracker!");
        }

        /// <summary>
        /// Handle changes in the user's music library.
        /// </summary>
        private static async void MusicLibrary_ContentsChanged(IStorageQueryResultBase sender, object args)
        {
            Debug.WriteLine("New change!");
            StorageFolder changedFolder = sender.Folder;
            StorageLibraryChangeTracker folderTracker = changedFolder.TryGetChangeTracker();
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
                    await SongIndexer.IndexAllSongsAsync();
                    await HandleMusicFolderChanges(App.MusicFolders);
                    return;
                }

                if (change.IsOfType(StorageItemTypes.File))
                {
                    await ManageSongChange(change);
                }
                else if (change.IsOfType(StorageItemTypes.Folder))
                {
                    // Not interested in folders
                }
                else
                {
                    if (change.ChangeType == StorageLibraryChangeType.Deleted)
                    {
                        for (int i = 0; i < ViewModel.Songs.Count; i++)
                        {
                            if (change.PreviousPath == ViewModel.Songs[i].Location)
                            {
                                await ViewModel.Songs[i].DeleteAsync();
                            }
                        }
                    }
                }
            }

            // Mark that all the changes have been seen and for the change tracker
            // to never return these changes again
            await changeReader.AcceptChangesAsync();
        }

        /// <summary>
        /// Handle folders being removed/added from the music library.
        /// </summary>
        private static async void MusicLibrary_DefinitionChanged(StorageLibrary sender, object args)
        {
            Debug.WriteLine("Music folder changes!");
            await App.RefreshMusicLibrary();

            await SongIndexer.IndexAllSongsAsync();
            await HandleMusicFolderChanges(App.MusicFolders);
        }

        /// <summary>
        /// Manage changes to the music library.
        /// </summary>
        /// <param name="change">Change that ocurred.</param>
        public static async Task ManageSongChange(StorageLibraryChange change)
        {
            StorageFile file;
            Song newSong;

            switch (change.ChangeType)
            {
                // New File in the Library
                case StorageLibraryChangeType.Created:
                    // Song was created..?
                    file = (StorageFile)await change.GetStorageItemAsync();
                    newSong = await SongIndexer.CreateModelAsync(file);
                    await SongIndexer.SaveModelsAsync(newSong, file);
                    break;

                case StorageLibraryChangeType.MovedIntoLibrary:
                    // Song was moved into the library
                    file = (StorageFile)await change.GetStorageItemAsync();
                    newSong = await SongIndexer.CreateModelAsync(file);
                    await SongIndexer.SaveModelsAsync(newSong, file);
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
                            newSong = await SongIndexer.CreateModelAsync(file);
                            await SongIndexer.SaveModelsAsync(newSong, file);
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
        /// Manage changes to the music library folders.
        /// </summary>
        /// <param name="folders">Folder changes.</param>
        public static async Task HandleMusicFolderChanges(List<StorageFolder> folders)
        {
            List<SongViewModel> toRemove = new List<SongViewModel>();
            foreach (SongViewModel song in ViewModel.Songs)
            {
                bool isInFolder = false;
                foreach (StorageFolder folder in folders)
                {
                    if (song.Location == folder.Path + @"\" + Path.GetFileName(song.Location))
                    {
                        isInFolder = true;
                        break;
                    }
                }

                if (!isInFolder)
                {
                    toRemove.Add(song);
                }
            }

            foreach (SongViewModel song in toRemove)
            {
                await song.DeleteAsync();
            }

            toRemove.Clear();
            toRemove.TrimExcess();
        }
    }
}
