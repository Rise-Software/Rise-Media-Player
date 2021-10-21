using RMP.App.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Search;

namespace RMP.App.ChangeTrackers
{
    /* public class VideosTracker
    {
        /// <summary>
        /// Gets the app-wide MViewModel instance.
        /// </summary>
        public static MainViewModel MViewModel => App.MViewModel;

        /// <summary>
        /// Manage changes to the videos library.
        /// </summary>
        /// <param name="change">Change that ocurred.</param>
        public static async Task ManageVideoChange(StorageLibraryChange change)
        {
            StorageFile file;

            // Temp variable used for instantiating StorageFiles for sorting if needed later
            switch (change.ChangeType)
            {
                // New File in the Library
                case StorageLibraryChangeType.Created:
                    // Song was created..?
                    file = (StorageFile)await change.GetStorageItemAsync();
                    await VideoIndexer.AddVideo(file);
                    break;

                case StorageLibraryChangeType.MovedIntoLibrary:
                    // Song was moved into the library
                    file = (StorageFile)await change.GetStorageItemAsync();
                    await VideoIndexer.AddVideo(file);
                    break;

                case StorageLibraryChangeType.MovedOrRenamed:
                    // Song was renamed/moved
                    file = (StorageFile)await change.GetStorageItemAsync();
                    for (int i = 0; i < MViewModel.Videos.Count; i++)
                    {
                        if (change.PreviousPath == MViewModel.Videos[i].Location)
                        {
                            MViewModel.Videos[i].Delete();
                            await VideoIndexer.AddVideo(file);
                        }
                    }
                    break;

                // File Removed From Library
                case StorageLibraryChangeType.Deleted:
                    // Song was deleted
                    for (int i = 0; i < MViewModel.Videos.Count; i++)
                    {
                        if (change.PreviousPath == MViewModel.Videos[i].Location)
                        {
                            MViewModel.Videos[i].Delete();
                        }
                    }
                    break;

                case StorageLibraryChangeType.MovedOutOfLibrary:
                    // Song got moved out of the library
                    for (int i = 0; i < MViewModel.Videos.Count; i++)
                    {
                        if (change.PreviousPath == MViewModel.Videos[i].Location)
                        {
                            MViewModel.Videos[i].Delete();
                        }
                    }
                    break;

                // Modified Contents
                case StorageLibraryChangeType.ContentsChanged:
                    // Song content was modified..?
                    file = (StorageFile)await change.GetStorageItemAsync();
                    for (int i = 0; i < MViewModel.Videos.Count; i++)
                    {
                        if (change.PreviousPath == MViewModel.Videos[i].Location)
                        {
                            MViewModel.Videos[i].Delete();
                            await VideoIndexer.AddVideo(file);
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
        /// Sets up the filesystem tracker for videos library.
        /// </summary>
        public static async void SetupVideoTracker()
        {
            App.VideoLibrary = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Videos);
            StorageFolder videos = KnownFolders.VideosLibrary;

            // Create a query containing all the files the app will be tracking
            QueryOptions videosOption = VideoIndexer.VideoQueryOptions;

            // Optimize indexing performance by using the Windows Indexer
            videosOption.IndexerOption = IndexerOption.UseIndexerWhenAvailable;

            // Prefetch file properties
            videosOption.SetPropertyPrefetch(PropertyPrefetchOptions.VideoProperties,
                VideoIndexer.VideoProperties);

            StorageFileQueryResult videosResultSet =
                videos.CreateFileQueryWithOptions(videosOption);

            // Indicate to the system the app is ready to change track
            _ = await videosResultSet.GetFilesAsync(0, 1);

            // Attach an event handler for when something changes on the system
            videosResultSet.ContentsChanged += VideosLibrary_ContentsChanged;
            App.VideoLibrary.DefinitionChanged += VideosLibrary_DefinitionChanged;
            Debug.WriteLine("Registered video tracker!");
        }

        /// <summary>
        /// Handle folders being removed/added from the video library.
        /// </summary>
        private static async void VideosLibrary_DefinitionChanged(StorageLibrary sender, object args)
        {
            Debug.WriteLine("Video folder changes!");

            await VideoIndexer.IndexAllVideos();
            HandleVideosFolderChanges(sender.Folders);
            MViewModel.Sync();
        }

        /// <summary>
        /// Manage changes to the videos library folders.
        /// </summary>
        /// <param name="folders">Folder changes.</param>
        public static void HandleVideosFolderChanges(IObservableVector<StorageFolder> folders)
        {
            bool isInFolder = false;
            foreach (VideoViewModel video in MViewModel.Videos)
            {
                foreach (StorageFolder folder in folders)
                {
                    if (video.Location.StartsWith(folder.Path))
                    {
                        isInFolder = true;
                        break;
                    }
                }

                if (!isInFolder)
                {
                    video.Delete();
                }

                isInFolder = false;
            }
        }

        /// <summary>
        /// Handle changes in the user's video library.
        /// </summary>
        private static async void VideosLibrary_ContentsChanged(IStorageQueryResultBase sender, object args)
        {
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
                    return;
                }

                if (change.IsOfType(StorageItemTypes.File))
                {
                    await ManageVideoChange(change);
                }
                else if (change.IsOfType(StorageItemTypes.Folder))
                {
                    // Not interested in folders
                }
                else
                {
                    if (change.ChangeType == StorageLibraryChangeType.Deleted)
                    {
                        for (int i = 0; i < MViewModel.Videos.Count; i++)
                        {
                            if (change.PreviousPath == MViewModel.Videos[i].Location)
                            {
                                MViewModel.Videos[i].Delete();
                            }
                        }
                    }
                }
            }

            // Mark that all the changes have been seen and for the change tracker
            // to never return these changes again
            await changeReader.AcceptChangesAsync();
        }
    } */
}
