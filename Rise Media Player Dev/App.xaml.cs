using Microsoft.Extensions.DependencyInjection;
using Microsoft.QueryStringDotNET;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Uwp.Notifications;
using Rise.App.ChangeTrackers;
using Rise.App.DbControllers;
using Rise.App.Services;
using Rise.App.ServicesImplementation;
using Rise.App.ViewModels;
using Rise.App.ViewModels.FileBrowser;
using Rise.App.Views;
using Rise.Common;
using Rise.Common.Extensions;
using Rise.Common.Helpers;
using Rise.Data.Sources;
using Rise.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.UI.Notifications;
using Windows.UI.ViewManagement;
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
        private IServiceProvider? ServiceProvider { get; set; }

        public static bool IsLoaded = false;

        private static TimeSpan _indexingInterval = TimeSpan.FromMinutes(5);

        public static TimeSpan IndexingInterval
        {
            get
            {
                return _indexingInterval;
            }
            set
            {
                if (value != _indexingInterval)
                {
                    _indexingInterval = value;
                }
                IndexingTimer = new(value.TotalMilliseconds)
                {
                    AutoReset = true
                };
            }
        }

        public static Timer IndexingTimer = new(IndexingInterval.TotalMilliseconds)
        {
            AutoReset = true
        };

        /// <summary>
        /// Gets the app-wide <see cref="PlaylistsBackendController"/> singleton instance.
        /// </summary>
        public static PlaylistsBackendController PBackendController { get; private set; }

        /// <summary>
        /// Gets the app-wide <see cref="NotificationsBackendController"/> singleton instance.
        /// </summary>
        public static NotificationsBackendController NBackendController { get; private set; }

        /// <summary>
        /// Gets the app-wide <see cref="FileBrowserPageViewModel"/> singleton instance.
        /// </summary>
        /// <remarks>This is a temporary store, subject to change at any point.</remarks>
        public static FileBrowserPageViewModel? FileBrowserPageViewModel { get; set; }

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
        /// Gets the app-wide <see cref="NavViewDataSource"/> singleton instance.
        /// </summary>
        public static NavViewDataSource NavDataSource { get; private set; }

        /// <summary>
        /// Gets the app-wide <see cref="LastFMViewModel"/> singleton instance.
        /// </summary>
        public static LastFMViewModel LMViewModel { get; private set; }

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
        #endregion

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            SViewModel = new();
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

            ServiceProvider = ConfigureServices();
            Ioc.Default.ConfigureServices(ServiceProvider);
        }

        private IServiceProvider ConfigureServices()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection
                .AddSingleton<IFileExplorerService, FileExplorerService>()
                .AddSingleton<IStorageService, UwpStorageService>();

            return serviceCollection.BuildServiceProvider();
        }

        private async void App_UnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
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

            string text = $"The exception {e.Exception.GetType()} happened last time the app was launched.\n\nStack trace:\n{e.Exception.Message}\n{e.Exception.StackTrace}\nSource: {e.Exception.Source}\nHResult: {e.Exception.HResult}";
            await NBackendController.AddNotificationAsync("Rise Media Player unexpectedly crashed.", "Here are some information on what happened:\n\n" + text + "\n\nYou could go to https://github.com/Rise-Software/Rise-Media-Player/issues to report this issue.", "");

            ToastNotification notification = new(content.GetXml());
            ToastNotificationManager.CreateToastNotifier().Show(notification);
        }

        protected async override void OnActivated(IActivatedEventArgs e)
        {
            switch (e.Kind)
            {
                case ActivationKind.StartupTask:
                    {
                        Frame rootFrame = await
                            InitializeWindowAsync(e.PreviousExecutionState);

                        if (rootFrame.Content == null)
                        {
                            _ = !SViewModel.SetupCompleted
                                ? rootFrame.Navigate(typeof(SetupPage))
                                : rootFrame.Navigate(typeof(MainPage));
                        }

                        Window.Current.Activate();
                        break;
                    }

                case ActivationKind.ToastNotification:
                    {
                        if (e is ToastNotificationActivatedEventArgs toastActivationArgs)
                        {
                            QueryString args = QueryString.Parse(toastActivationArgs.Argument);

                            string text = $"The exception {args["exceptionName"]} happened last time the app was launched.\n\nStack trace:\n{args["message"]}\n{args["stackTrace"]}\nSource: {args["source"]}\nHResult: {args["hresult"]}";

                            Frame rootFrame = await
                                InitializeWindowAsync(e.PreviousExecutionState);

                            if (rootFrame.Content == null)
                            {
                                _ = !SViewModel.SetupCompleted
                                    ? rootFrame.Navigate(typeof(SetupPage))
                                    : rootFrame.Navigate(typeof(MainPage));
                            }

                            Window.Current.Activate();
                            _ = typeof(CrashDetailsPage).
                                ShowInApplicationViewAsync(text, 600, 600);
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// Initializes the app's ViewModels.
        /// </summary>
        private async Task InitDataSourcesAsync()
        {
            await NewRepository.Repository.InitializeDatabaseAsync();

            MusicLibrary ??= await StorageLibrary.GetLibraryAsync(KnownLibraryId.Music);
            VideoLibrary ??= await StorageLibrary.GetLibraryAsync(KnownLibraryId.Videos);

            PBackendController ??= new PlaylistsBackendController();
            NBackendController ??= new NotificationsBackendController();

            MViewModel ??= new MainViewModel();
            LMViewModel ??= new LastFMViewModel();
            PViewModel ??= new PlaybackViewModel();
            NavDataSource ??= new NavViewDataSource();

            MusicLibrary.DefinitionChanged += MusicLibrary_DefinitionChanged;
            VideoLibrary.DefinitionChanged += MusicLibrary_DefinitionChanged;
        }

        private async void MusicLibrary_DefinitionChanged(StorageLibrary sender, object args)
        {
            // Prevent duplicate calls.
            if (IsLoaded)
            {
                Debug.WriteLine("Definition changes!");
                await Task.Run(async () => await MViewModel.StartFullCrawlAsync());
            }
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = await InitializeWindowAsync(e.PreviousExecutionState);
            if (!e.PrelaunchActivated)
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
            SuspendingDeferral deferral = null;
            try
            {
                deferral = e?.SuspendingOperation?.GetDeferral();

                if (NavDataSource != null)
                {
                    await NavDataSource.SerializeGroupsAsync();
                }

                await SuspensionManager.SaveAsync();
            }
            catch (SuspensionManagerException)
            {
            }
            finally
            {
                deferral?.Complete();
            }
        }

        protected override async void OnFileActivated(FileActivatedEventArgs args)
        {
            Frame rootFrame = await InitializeWindowAsync(args.PreviousExecutionState);
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
                ShowInApplicationViewAsync(AppWindowPresentationKind.Default, 320, 300);

            StorageApplicationPermissions.FutureAccessList.AddOrReplace("CurrentlyPlayingFile", args.Files[0] as StorageFile);
            try
            {
                var song = await Song.GetFromFileAsync(args.Files[0] as StorageFile);
                await PViewModel.PlaySongAsync(new SongViewModel(song));
            }
            catch (Exception)
            {

            }
            StorageApplicationPermissions.FutureAccessList.Remove("CurrentlyPlayingFile");
        }

        /// <summary>
        /// Initializes the main app window.
        /// </summary>
        /// <param name="previousState">Previous app execution state.</param>
        /// <returns>The app window's root frame.</returns>
        private async Task<Frame> InitializeWindowAsync(ApplicationExecutionState previousState)
        {
            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (Window.Current.Content is not Frame rootFrame)
            {
                await InitDataSourcesAsync();
                await MViewModel.GetListsAsync();

                StartIndexingTimer();

                // LeavingBackground += async (s, e) => await MViewModel.StartFullCrawlAsync();

                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();
                rootFrame.CacheSize = 1;
                rootFrame.NavigationFailed += OnNavigationFailed;

                // Associate the frame with a SuspensionManager key.
                SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

                if ((previousState == ApplicationExecutionState.Terminated) ||
                    (previousState == ApplicationExecutionState.ClosedByUser &&
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

                if (SViewModel.AutoIndexingEnabled)
                {
                    _ = await KnownFolders.MusicLibrary.
                        TrackForegroundAsync(QueryPresets.SongQueryOptions,
                        SongsTracker.MusicQueryResultChanged);

                    _ = await KnownFolders.VideosLibrary.
                        TrackForegroundAsync(QueryPresets.VideoQueryOptions,
                        VideosTracker.VideosLibrary_ContentsChanged);
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            IsLoaded = true;

            return rootFrame;
        }

        public static void StartIndexingTimer()
        {
            if (SViewModel.AutoIndexingEnabled)
            {
                if (!IndexingTimer.Enabled)
                {
                    switch (SViewModel.IndexingMode)
                    {
                        case -1:
                            return;
                        case 0:
                            IndexingInterval = TimeSpan.FromMinutes(1);
                            break;
                        case 1:
                            IndexingInterval = TimeSpan.FromMinutes(5);
                            break;
                        case 2:
                            IndexingInterval = TimeSpan.FromMinutes(10);
                            break;
                        case 3:
                            IndexingInterval = TimeSpan.FromMinutes(30);
                            break;
                        case 4:
                            IndexingInterval = TimeSpan.FromHours(1);
                            break;
                    }

                    IndexingTimer.Start();
                    IndexingTimer.Elapsed += IndexingTimer_Elapsed;
                }
                else
                {
                    IndexingTimer.Elapsed -= IndexingTimer_Elapsed;
                    IndexingTimer.Stop();

                    StartIndexingTimer();
                }
            }
        }

        private static async void IndexingTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                await Task.Run(async () => await MViewModel.StartFullCrawlAsync());
            }
            catch (Exception)
            {
                Debug.WriteLine("An error occured while indexing.");
            }
        }
    }
}
