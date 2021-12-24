using Microsoft.EntityFrameworkCore;
using Microsoft.QueryStringDotNET;
using Microsoft.Toolkit.Uwp.Notifications;
using Rise.App.ChangeTrackers;
using Rise.App.Common;
using Rise.App.Indexing;
using Rise.App.ViewModels;
using Rise.App.Views;
using Rise.Repository;
using Rise.Repository.SQL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Rise.App
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
        #region Variables

        public static StorageFolder PlaylistsFolder { get; set; }

        /// <summary>
        /// Gets the app-wide <see cref="MainViewModel"/> singleton instance.
        /// </summary>
        public static MainViewModel MViewModel { get; private set; }

        /// <summary>
        /// Gets the app-wide <see cref="PlaybackViewModel"/> singleton instance.
        /// </summary>
        public static PlaybackViewModel PViewModel { get; private set; }

        /// <summary>
        /// Gets the app-wide <see cref="SettingsViewModel"/> singleton instance.
        /// </summary>
        public static SettingsViewModel SViewModel { get; private set; }

        /// <summary>
        /// Gets the app-wide <see cref="SidebarViewModel"/> singleton instance.
        /// </summary>
        public static SidebarViewModel SBViewModel { get; private set; }

        /// <summary>
        /// Pipeline for interacting with backend service or database.
        /// </summary>
        public static IRepository Repository { get; private set; }

        /// <summary>
        /// Gets the music library.
        /// </summary>
        public static StorageLibrary MusicLibrary { get; private set; }

        /// <summary>
        /// Gets all the folders in the music library.
        /// </summary>
        public static List<StorageFolder> MusicFolders { get; private set; }

        /// <summary>
        /// Gets the video library.
        /// </summary>
        public static StorageLibrary VideoLibrary { get; private set; }

        /// <summary>
        /// Gets all the folders in the videos library.
        /// </summary>
        public static List<StorageFolder> VideoFolders { get; private set; }

        private static List<StorageLibraryChange> Changes { get; set; }
            = new List<StorageLibraryChange>();

        public static IndexingHelper Indexer { get; private set; }
            = new IndexingHelper();
        #endregion

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            SViewModel = new SettingsViewModel();
            switch (SViewModel.Theme)
            {
                case 0:
                    RequestedTheme = ApplicationTheme.Light;
                    break;

                case 1:
                    RequestedTheme = ApplicationTheme.Dark;
                    break;
            }

            InitializeComponent();
            Suspending += OnSuspending;
            UnhandledException += App_UnhandledException;
        }

        private void App_UnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            ToastContent content = new ToastContentBuilder()
                .AddToastActivationInfo(new QueryString()
                {
                     { "stackTrace", e.Exception.StackTrace },
                     { "message", e.Exception.Message },
                     { "exceptionName", e.Exception.GetType().ToString() },
                     { "source", e.Exception.Source },
                     { "hresult", $"{e.Exception.HResult}" }
                }.ToString(), ToastActivationType.Foreground)
                .AddText("An error occured!")
                .AddText("Unfortunately, Rise Media Player crashed. Click to view stack trace.")
                .GetToastContent();

            ToastNotification notification = new ToastNotification(content.GetXml());
            ToastNotificationManager.CreateToastNotifier().Show(notification);
        }

        protected override async void OnActivated(IActivatedEventArgs e)
        {
            if (e is ToastNotificationActivatedEventArgs toastActivationArgs)
            {
                QueryString args = QueryString.Parse(toastActivationArgs.Argument);

                string text = $"The exception {args["exceptionName"]} happened last time the app was launched.\n\nStack trace:\n{args["message"]}\n{args["stackTrace"]}\nSource: {args["source"]}\nHResult: {args["hresult"]}";

                Frame rootFrame = await InitializeWindowAsync(e);
                if (rootFrame.Content == null)
                {
                    _ = !SViewModel.SetupCompleted
                        ? rootFrame.Navigate(typeof(SetupPage))
                        : rootFrame.Navigate(typeof(MainPage));
                }

                Window.Current.Activate();
                _ = typeof(CrashDetailsPage).PlaceInWindowAsync(Windows.UI.ViewManagement.ApplicationViewMode.Default, 600, 600, true, text);
            }
        }

        /// <summary>
        /// Initializes the app's database and ViewModels.
        /// </summary>
        private async Task InitDatabase()
        {
            _ = await ApplicationData.Current.LocalCacheFolder.CreateFileAsync("Files.db", CreationCollisionOption.OpenIfExists);
            string dbPath = Path.Combine(ApplicationData.Current.LocalCacheFolder.Path, "Files.db");
            DbContextOptionsBuilder<Context> dbOptions = new DbContextOptionsBuilder<Context>().UseSqlite(
                "Data Source=" + dbPath);

            Repository = new SQLRepository(dbOptions);

            MusicLibrary = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Music);
            VideoLibrary = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Videos);

            MViewModel = new MainViewModel();
            PViewModel = new PlaybackViewModel();
            SBViewModel = new SidebarViewModel();

            MusicLibrary.DefinitionChanged += MusicLibrary_DefinitionChanged;
        }

        private async void MusicLibrary_DefinitionChanged(StorageLibrary sender, object args)
        {
            Debug.WriteLine("Definition changes!");
            await MViewModel.StartFullCrawlAsync();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = await InitializeWindowAsync(e);
            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    _ = !SViewModel.SetupCompleted
                        ? rootFrame.Navigate(typeof(SetupPage), e.Arguments)
                        : rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }

                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            SuspendingDeferral deferral = e.SuspendingOperation.GetDeferral();
            await SBViewModel.SerializeItemsAsync();
            try
            {
                await SuspensionManager.SaveAsync();
            }
            catch (SuspensionManagerException)
            {

            }

            deferral.Complete();
        }

        protected override async void OnFileActivated(FileActivatedEventArgs args)
        {
            Frame rootFrame = await InitializeWindowAsync(args);
            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                _ = !SViewModel.SetupCompleted
                    ? rootFrame.Navigate(typeof(SetupPage))
                    : rootFrame.Navigate(typeof(MainPage));
            }

            // Ensure the current window is active
            Window.Current.Activate();

            _ = await typeof(NowPlaying).
                PlaceInWindowAsync(AppWindowPresentationKind.Default, 320, 300);

            await PViewModel.StartMusicPlaybackAsync(args.Files.GetEnumerator(), 0, args.Files.Count);
        }

        /// <summary>
        /// Initializes the main app window.
        /// </summary>
        /// <param name="args">Event args, must be of type
        /// <see cref="FileActivatedEventArgs"/> or <see cref="LaunchActivatedEventArgs"/>.</param>
        /// <returns>The app window's root frame.</returns>
        private async Task<Frame> InitializeWindowAsync(dynamic args)
        {
#if DEBUG
            if (Debugger.IsAttached)
            {
                // DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (!(Window.Current.Content is Frame rootFrame))
            {
                await InitDatabase();
                await MViewModel.GetListsAsync();

                LeavingBackground += async (s, e) =>
                    await MViewModel.StartFullCrawlAsync();

                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame
                {
                    CacheSize = 1
                };
                rootFrame.NavigationFailed += OnNavigationFailed;

                // Associate the frame with a SuspensionManager key.
                SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

                if ((args.PreviousExecutionState == ApplicationExecutionState.Terminated) ||
                    (args.PreviousExecutionState == ApplicationExecutionState.ClosedByUser &&
                    SViewModel.PickUp))
                {
                    // Restore the saved session state only when appropriate.
                    try
                    {
                        await SuspensionManager.RestoreAsync();
                    }
                    catch (SuspensionManagerException)
                    {
                        // Something went wrong restoring state.
                        // Assume there is no state and continue.
                    }
                }

                _ = await KnownFolders.MusicLibrary.
                    TrackForegroundAsync(QueryPresets.SongQueryOptions,
                    SongsTracker.MusicQueryResultChanged);
                _ = await KnownFolders.VideosLibrary.
                    TrackForegroundAsync(QueryPresets.VideoQueryOptions,
                    SongsTracker.MusicQueryResultChanged);

                // await MViewModel.StartFullCrawlAsync();

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            return rootFrame;
        }
    }
}
