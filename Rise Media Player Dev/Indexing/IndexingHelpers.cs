using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Search;

namespace Rise.App.Indexing
{
    public static class IndexingHelpers
    {
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
        /// <returns>Whether or not the registration was successful.</returns>
        public static async Task<bool> TrackBackgroundAsync(this StorageLibrary library,
            string taskName, string entryPoint = null)
        {
            // Check if there's access to the background.
            var requestStatus = await BackgroundExecutionManager.RequestAccessAsync();

            if (!(requestStatus == BackgroundAccessStatus.AllowedSubjectToSystemPolicy ||
                requestStatus == BackgroundAccessStatus.AlwaysAllowed))
            {
                return false;
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

            var libraryTrigger = StorageLibraryContentChangedTrigger.Create(library);

            builder.SetTrigger(libraryTrigger);
            _ = builder.Register();

            return true;
        }
        #endregion
    }
}
