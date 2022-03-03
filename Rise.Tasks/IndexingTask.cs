using Windows.ApplicationModel.Background;

namespace Rise.Tasks
{
    public sealed class IndexingTask : IBackgroundTask
    {
        private BackgroundTaskDeferral deferral;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            this.deferral = taskInstance.GetDeferral();

            this.deferral.Complete();
        }
    }
}
