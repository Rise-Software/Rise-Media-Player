using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage;

namespace Rise.Tasks
{
    /*public sealed class MusicIndexingTask : IBackgroundTask
    {
        private BackgroundTaskDeferral deferral;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            this.deferral = taskInstance.GetDeferral();

            var tracker = await GetChangeTrackerAsync();
            await ProcessChangesAsync(tracker);

            this.deferral.Complete();
        }

        private async Task<StorageLibraryChangeTracker> GetChangeTrackerAsync()
        {
            StorageLibrary library = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Music);
            library.ChangeTracker.Enable();

            return library.ChangeTracker;
        }

        private async Task ProcessChangesAsync(StorageLibraryChangeTracker changeTracker)
        {
            var reader = changeTracker.GetChangeReader();
            var changeSet = await reader.ReadBatchAsync();

            foreach (StorageLibraryChange change in changeSet)
            {
                if (change.ChangeType == StorageLibraryChangeType.ChangeTrackingLost)
                {
                    changeTracker.Reset();
                    return;
                }

                if (change.IsOfType(StorageItemTypes.Folder))
                {

                }
                else if (change.IsOfType(StorageItemTypes.File))
                {

                }
                else if (change.IsOfType(StorageItemTypes.None))
                {
                    if (change.ChangeType == StorageLibraryChangeType.Deleted)
                    {

                    }
                }
            }

            await reader.AcceptChangesAsync();
        }
    }*/
}
