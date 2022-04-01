using Rise.App.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;

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
            folderTracker.Enable();

            StorageLibraryChangeReader changeReader = folderTracker.GetChangeReader();
            IReadOnlyList<StorageLibraryChange> changes = await changeReader.ReadBatchAsync();

            List<SongViewModel> songRemoveQueue = new();

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
                    await ManageSongChange(change);
                }
                else if (change.IsOfType(StorageItemTypes.Folder))
                {
                    //await ViewModel.StartFullCrawlAsync();
                }
                else
                {
                    if (change.ChangeType == StorageLibraryChangeType.Deleted)
                    {
                        try
                        {
                            foreach (SongViewModel song in ViewModel.Songs)
                            {
                                if (change.PreviousPath == song.Location)
                                {
                                    songRemoveQueue.Add(song);
                                }
                            }
                        } finally
                        {
                            foreach (SongViewModel song in songRemoveQueue)
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

        /// <summary>
        /// Manage changes to the music library.
        /// </summary>
        /// <param name="change">Change that ocurred.</param>
        public static async Task ManageSongChange(StorageLibraryChange change)
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
        /// Manage changes to the music library folders.
        /// </summary>
        public static async Task HandleMusicFolderChanges()
        {
            List<SongViewModel> toRemove = new();
            /*foreach (SongViewModel song in ViewModel.Songs)
            {
                if (!File.Exists(song.Location))
                {
                    await song.DeleteAsync();
                }
            }*/

            try
            {
                for (int i = 0; i < ViewModel.Songs.Count; i++)
                {
                    if (await StorageFile.GetFileFromPathAsync(ViewModel.Songs[i].Location) == null)
                    {
                        toRemove.Add(ViewModel.Songs[i]);
                    }
                }
            }
            finally
            {
                foreach (SongViewModel song in toRemove)
                {
                    await song.DeleteAsync();
                }
            }
        }
    }
}
