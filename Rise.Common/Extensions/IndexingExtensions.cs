﻿using Rise.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Search;

namespace Rise.Common.Extensions
{
    public static class IndexingExtensions
    {
        #region Indexing
        /// <summary>
        /// Indexes a library's contents based on personalized
        /// query options.
        /// </summary>
        /// <param name="library">Library to index.</param>
        /// <param name="queryOptions">Query options.</param>
        /// <param name="prefetchOptions">What options to prefetch.</param>
        /// <param name="extraProps">Extra properties to prefetch.</param>
        public static async IAsyncEnumerable<StorageFile> IndexAsync(this StorageLibrary library,
            QueryOptions queryOptions,
            PropertyPrefetchOptions prefetchOptions = PropertyPrefetchOptions.BasicProperties,
            IEnumerable<string> extraProps = null)
        {
            // Prefetch file properties.
            queryOptions.SetPropertyPrefetch(prefetchOptions, extraProps);

            // Index library.
            foreach (StorageFolder folder in library.Folders)
            {
                await foreach (var file in folder.IndexAsync(queryOptions).ConfigureAwait(false))
                {
                    yield return file;
                }
            }
        }

        /// <summary>
        /// Indexes a folder's contents based on personalized
        /// query options.
        /// </summary>
        /// <param name="folder">Folder to index.</param>
        /// <param name="options">Query options.</param>
        /// <param name="stepSize">The step size. This allows for
        /// the files to be indexed and processed in batches. Must
        /// be 1 or greater.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when
        /// an invalid <paramref name="stepSize"/> is specified.</exception>
        public static async IAsyncEnumerable<StorageFile> IndexAsync(this StorageFolder folder,
            QueryOptions options,
            uint stepSize = 10)
        {
            if (stepSize < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(stepSize));
            }

            int indexedFiles = 0;

            // Prepare the query
            StorageFileQueryResult folderQueryResult = folder.CreateFileQueryWithOptions(options);

            // Index by steps
            uint index = 0;

            IReadOnlyList<StorageFile> fileList = await folderQueryResult.GetFilesAsync(index, stepSize);
            index += stepSize;

            // Start crawling data
            while (fileList.Count != 0)
            {
                // Process files
                foreach (StorageFile file in fileList)
                {
                    indexedFiles++;
                    yield return file;
                }

                fileList = await folderQueryResult.GetFilesAsync(index, stepSize).AsTask().ConfigureAwait(false);
                index += stepSize;
            }
        }
        #endregion

        #region Tracking
        /// <summary>
        /// Registers a <see cref="StorageFolder"/> for foreground change tracking.
        /// </summary>
        /// <param name="folder"><see cref="StorageFolder"/> to register.</param>
        /// <param name="queryOptions"><see cref="QueryOptions"/> to use.
        /// The tracker will only show changes that fit the query.</param>
        /// <param name="queryEventHandler">Event handler to control the changes.</param>
        /// <returns>The <see cref="StorageFileQueryResult"/>, ready for
        /// change tracking.</returns>
        public static async Task<StorageFileQueryResult>
            TrackForegroundAsync(this StorageFolder folder,
            QueryOptions queryOptions,
            TypedEventHandler<IStorageQueryResultBase, object> queryEventHandler)
        {
            // This is important because you are going to use indexer for notifications
            queryOptions.IndexerOption = IndexerOption.UseIndexerWhenAvailable;

            StorageFileQueryResult resultSet =
                folder.CreateFileQueryWithOptions(queryOptions);

            // Indicate to the system the app is ready to change track
            await resultSet.GetFilesAsync(0, 1);

            // Attach an event handler for when something changes on the system
            resultSet.ContentsChanged += queryEventHandler;
            return resultSet;
        }

        /// <summary>
        /// Registers a background <see cref="StorageLibraryChangeTracker"/>.
        /// </summary>
        /// <param name="library">Library to track.</param>
        /// <param name="taskName">Preferred background task name.</param>
        /// <param name="entryPoint">The <see cref="BackgroundTaskBuilder.TaskEntryPoint"/>.
        /// If not provided, the single process model will be used for the background
        /// task.</param>
        /// <returns>A <see cref="BackgroundTaskRegistrationStatus" /> which represents the status.</returns>
        public static async Task<BackgroundTaskRegistrationStatus> TrackBackgroundAsync(this StorageLibrary library,
            string taskName, string entryPoint = null)
        {
            // Check if there's access to the background.
            var requestStatus = await BackgroundExecutionManager.RequestAccessAsync();

            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == taskName)
                    task.Value.Unregister(false);
            }

            if (!(requestStatus == BackgroundAccessStatus.AllowedSubjectToSystemPolicy ||
                requestStatus == BackgroundAccessStatus.AlwaysAllowed))
            {
                return BackgroundTaskRegistrationStatus.NotAllowed;
            }

            // Build up the trigger to fire when something changes in the library.
            var builder = new BackgroundTaskBuilder
            {
                Name = taskName
            };

            if (entryPoint != null)
            {
                builder.TaskEntryPoint = entryPoint;
            }

            library.ChangeTracker.Enable();

            var libraryTrigger = StorageLibraryContentChangedTrigger.Create(library);

            builder.SetTrigger(libraryTrigger);
            _ = builder.Register();

            return BackgroundTaskRegistrationStatus.Successful;
        }
        #endregion
    }
}
