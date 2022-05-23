using Rise.App.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Rise.Common.Extensions;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;
using System.IO;

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
        public static async Task HandleMusicFolderChangesAsync()
        {
            List<SongViewModel> toRemove = new();

            // Check if the song doesn't exist anymore, or if we don't have access to it at all,
            // if so queue it then remove.
            try
            {
                for (int i = 0; i < ViewModel.Songs.Count; i++)
                {
                    try
                    {
                        _ = await StorageFile.GetFileFromPathAsync(ViewModel.Songs[i].Location);
                    } catch (FileNotFoundException e)
                    {
                        toRemove.Add(ViewModel.Songs[i]);
                        e.WriteToOutput();
                    }
                    catch (FileLoadException e)
                    {
                        toRemove.Add(ViewModel.Songs[i]);
                        e.WriteToOutput();
                    }
                    catch (UnauthorizedAccessException e)
                    {
                        toRemove.Add(ViewModel.Songs[i]);
                        e.WriteToOutput();
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

            List<SongViewModel> duplicates = new();

            // Check for duplicates and remove if any duplicate is found.
            try
            {
                for (int i = 0; i < ViewModel.Songs.Count; i++)
                {
                    for (int j = i + 1; j < ViewModel.Songs.Count; j++)
                    {
                        if (ViewModel.Songs[i].Location == ViewModel.Songs[j].Location)
                        {
                            duplicates.Add(ViewModel.Songs[j]);
                        }
                    }
                }
            }
            finally
            {
                foreach (SongViewModel song in duplicates)
                {
                    await song.DeleteAsync();
                }
            }
        }
    }
}
