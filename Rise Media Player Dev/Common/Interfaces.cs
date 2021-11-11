using System.Threading;
using System.Threading.Tasks;

namespace Rise.App.Common
{
    /// <summary>
    /// Allows for Tasks to be cancelled while running.
    /// </summary>
    /// <example>
    /// <code>
    /// public async Task CancellableTask(CancellationToken token)
    /// {
    ///     while (!CanContinue)
    ///     {
    ///         // Wait here, allows for some cleanup if needed.
    ///         await Task.Delay(30);
    ///     }
    ///     
    ///     bool work = true;
    ///     while (work)
    ///     {
    ///         if (token.IsCancellationRequested)
    ///         {
    ///             Cleanup();
    ///             work = false;
    ///         }
    ///         await DoWork();
    ///     }
    ///     
    ///     CanContinue = true;
    /// }
    /// </code>
    /// </example>
    public interface ICancellableTask
    {
        /// <summary>
        /// Used to be able to cancel a task.
        /// </summary>
        CancellationTokenSource CTS { get; set; }

        /// <summary>
        /// The <see cref="CancellationToken"/> for the task.
        /// Must return <see cref="CancellationTokenSource.Token"/>.
        /// </summary>
        CancellationToken Token { get; }

        /// <summary>
        /// Whether or not the task can continue.
        /// </summary>
        bool CanContinue { get; set; }

        /// <summary>
        /// Cancels the currently running <see cref="Task"/>.
        /// </summary>
        /// <example>
        /// <code>
        /// public void CancelTask()
        /// {
        ///     CTS.Cancel();
        ///     CTS = new CancellationTokenSource();
        /// }
        /// </code>
        /// </example>
        void CancelTask();
    }
}
