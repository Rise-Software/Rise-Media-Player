using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Search;

namespace RMP.App.Indexing
{
    public class Indexer
    {
        #region Events and delegates
        public delegate void IndexingStarted();

        public event IndexingStarted Started;
        public event EventHandler<StorageFile> FileIndexed;
        public event EventHandler<int> Finished;
        #endregion

        #region Event handlers
        protected virtual void OnFileIndexed(StorageFile file)
            => FileIndexed?.Invoke(this, file);

        protected virtual void OnFinished(int files)
            => Finished?.Invoke(this, files);
        #endregion

        /// <summary>
        /// Indexes a <see cref="StorageLibrary"/> based on personalized
        /// query options.
        /// </summary>
        /// <param name="library"><see cref="StorageLibrary"/> to index.</param>
        /// <param name="queryOptions"><see cref="QueryOptions"/> to use.</param>
        /// <param name="indexerOption">Whether or not to use the system index.</param>
        /// <param name="prefetchOptions">What options to prefetch.</param>
        /// <param name="extraProps">Extra properties to prefetch.</param>
        public async Task IndexLibraryAsync(StorageLibrary library,
            QueryOptions queryOptions,
            IndexerOption indexerOption = IndexerOption.UseIndexerWhenAvailable,
            PropertyPrefetchOptions prefetchOptions = PropertyPrefetchOptions.BasicProperties,
            IEnumerable<string> extraProps = null)
        {
            int indexedFiles = 0;

            // Optimize indexing performance by using the Windows Indexer.
            queryOptions.IndexerOption = indexerOption;

            // Prefetch file properties.
            queryOptions.SetPropertyPrefetch(prefetchOptions, extraProps);

            // Index library.
            Started?.Invoke();
            foreach (StorageFolder folder in library.Folders)
            {
                indexedFiles += await IndexFolderAsync(folder, queryOptions);
            }

            OnFinished(indexedFiles);
        }

        /// <summary>
        /// Index a folder's contents.
        /// </summary>
        /// <param name="folder">Folder to index.</param>
        /// <param name="options">Query options.</param>
        /// <returns>The amount of files indexed.</returns>
        private async Task<int> IndexFolderAsync(StorageFolder folder, QueryOptions options)
        {
            int indexedFiles = 0;

            // Prepare the query
            StorageFileQueryResult folderQueryResult = folder.CreateFileQueryWithOptions(options);

            // Index by steps
            uint index = 0, stepSize = 10;
            IReadOnlyList<StorageFile> fileList = await folderQueryResult.GetFilesAsync(index, stepSize);
            index += 10;

            // Start crawling data
            while (fileList.Count != 0)
            {
                Task<IReadOnlyList<StorageFile>> fileTask =
                    folderQueryResult.GetFilesAsync(index, stepSize).AsTask();

                // Process files
                foreach (StorageFile file in fileList)
                {
                    indexedFiles++;
                    OnFileIndexed(file);
                }

                fileList = await fileTask;
                index += 10;
            }

            return indexedFiles;
        }

        /// <summary>
        /// Index a folder's contents.
        /// </summary>
        /// <param name="folder">Folder to index.</param>
        /// <param name="options">Query options.</param>
        /// <param name="stepSize">The step size. This allows for
        /// the files to be indexed and processed in batches. Must
        /// be 1 or greater.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when
        /// an invalid <paramref name="stepSize"/> is specified.</exception>
        public async Task IndexFolderAsync(StorageFolder folder,
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
            index += 10;

            // Start crawling data
            Started?.Invoke();
            while (fileList.Count != 0)
            {
                Task<IReadOnlyList<StorageFile>> fileTask =
                    folderQueryResult.GetFilesAsync(index, stepSize).AsTask();

                // Process files
                foreach (StorageFile file in fileList)
                {
                    indexedFiles++;
                    OnFileIndexed(file);
                }

                fileList = await fileTask;
                index += 10;
            }

            OnFinished(indexedFiles);
        }
    }

    public static class IndexerExtensions
    {
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
    }
}
