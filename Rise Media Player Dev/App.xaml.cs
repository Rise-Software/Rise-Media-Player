using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.QueryStringDotNET;
using Microsoft.Toolkit.Uwp.Notifications;
using Rise.App.ChangeTrackers;
using Rise.App.DbControllers;
using Rise.App.ViewModels;
using Rise.App.Views;
using Rise.Common;
using Rise.Common.Constants;
using Rise.Common.Enums;
using Rise.Common.Extensions;
using Rise.Common.Helpers;
using Rise.Data.Sources;
using Rise.Data.ViewModels;
using Rise.Effects;
using Rise.Models;
using Rise.NewRepository;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.UI;
using Windows.UI.Notifications;
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
        public static bool IsLoaded;

        public static bool MainPageLoaded;

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

        // ViewModels
        private readonly static Lazy<MainViewModel> _mViewModel
            = new(() => new MainViewModel());
        public static MainViewModel MViewModel => _mViewModel.Value;

        private readonly static Lazy<MediaPlaybackViewModel> _mpViewModel
            = new(OnMPViewModelRequested);
        public static MediaPlaybackViewModel MPViewModel => _mpViewModel.Value;

        private readonly static Lazy<SettingsViewModel> _sViewModel
            = new(() => new SettingsViewModel());
        public static SettingsViewModel SViewModel => _sViewModel.Value;

        private readonly static Lazy<LastFMViewModel> _lmViewModel
            = new(OnLFMRequested);
        public static LastFMViewModel LMViewModel => _lmViewModel.Value;

        // Backend controllers
        private readonly static Lazy<PlaylistsBackendController> _pBackendController
            = new(() => new PlaylistsBackendController());
        public static PlaylistsBackendController PBackendController => _pBackendController.Value;

        private readonly static Lazy<NotificationsBackendController> _nBackendController
            = new(() => new NotificationsBackendController());
        public static NotificationsBackendController NBackendController => _nBackendController.Value;

        private readonly static Lazy<WidgetsBackendController> _wBackendController
            = new(() => new WidgetsBackendController());
        public static WidgetsBackendController WBackendController => _wBackendController.Value;

        // Data sources
        private readonly static Lazy<NavViewDataSource> _navDataSource
            = new(() => new NavViewDataSource());
        public static NavViewDataSource NavDataSource => _navDataSource.Value;

        // Libraries
        private readonly static Lazy<StorageLibrary> _musicLibrary
            = new(OnStorageLibraryRequested(KnownLibraryId.Music));
        public static StorageLibrary MusicLibrary => _musicLibrary.Value;

        private readonly static Lazy<StorageLibrary> _videoLibrary
            = new(OnStorageLibraryRequested(KnownLibraryId.Videos));
        public static StorageLibrary VideoLibrary => _videoLibrary.Value;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            if (SViewModel.Theme == 0)
                RequestedTheme = ApplicationTheme.Light;
            else if (SViewModel.Theme == 1)
                RequestedTheme = ApplicationTheme.Dark;

            // Reset the glaze color before startup if necessary
            if (SViewModel.SelectedGlaze == GlazeTypes.MediaThumbnail)
                SViewModel.GlazeColors = Colors.Transparent;

            InitializeComponent();

            Suspending += OnSuspending;
            UnhandledException += OnUnhandledException;

            AppDomain.CurrentDomain.UnhandledException += OnCurrentDomainUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            e.Exception.WriteToOutput();
            ShowExceptionToast(e.Exception);
        }

        private void OnUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            e.Exception.WriteToOutput();
            ShowExceptionToast(e.Exception);
        }

        private void OnCurrentDomainUnhandledException(object sender, System.UnhandledExceptionEventArgs e)
        {
            (e.ExceptionObject as Exception).WriteToOutput();
            ShowExceptionToast(e.ExceptionObject as Exception);
        }

        protected override async void OnActivated(IActivatedEventArgs e)
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

                            // If the exception name equals to null,
                            // then the toast likely isn't popping up
                            // as a result of an app crash.
                            if (args["exceptionName"] != null)
                            {
                                string text = $"The exception {args["exceptionName"]} happened last time the app was launched.\n\nStack trace:\n{args["message"]}\n{args["stackTrace"]}\nSource: {args["source"]}\nHResult: {args["hresult"]}";

                                _ = typeof(CrashDetailsPage).
                                    ShowInApplicationViewAsync(text, 600, 600);
                            }
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// Shows a toast when an exception is thrown.
        /// </summary>
        private void ShowExceptionToast(Exception e)
        {
            ToastContent content = new ToastContentBuilder()
                .AddToastActivationInfo(new QueryString()
                {
                     { "stackTrace", e.StackTrace },
                     { "message", e.Message },
                     { "exceptionName", e.GetType().ToString() },
                     { "source", e.Source },
                     { "hresult", $"{e.HResult}" }
                }.ToString(), ToastActivationType.Foreground)
                .AddText("An error occured!")
                .AddText("Unfortunately, Rise Media Player crashed. Click to view stack trace.")
                .GetToastContent();

            //string text = $"The exception {e.GetType()} happened last time the app was launched.\n\nStack trace:\n{e.Message}\n{e.StackTrace}\nSource: {e.Source}\nHResult: {e.HResult}";

            //await NBackendController.AddNotificationAsync("Rise Media Player unexpectedly crashed.", "Here is some information on what happened:\n\n" + text + "\n\nYou could go to https://github.com/Rise-Software/Rise-Media-Player/issues to report this issue.", "");

            ToastNotification notification = new(content.GetXml());
            ToastNotificationManager.CreateToastNotifier().Show(notification);
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
                CoreApplication.EnablePrelaunch(true);
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

                if (_navDataSource.IsValueCreated)
                    await NavDataSource.SerializeGroupsAsync();

                await SuspensionManager.SaveAsync();
            }
            catch (SuspensionManagerException ex)
            {
                ex.WriteToOutput();
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

            _ = await typeof(NowPlayingPage).
                ShowInApplicationViewAsync(null, 320, 300);

            StorageApplicationPermissions.FutureAccessList.AddOrReplace("CurrentlyPlayingFile", args.Files[0] as StorageFile);
            try
            {
                var song = await Song.GetFromFileAsync(args.Files[0] as StorageFile);
                await MPViewModel.PlaySingleItemAsync(new SongViewModel(song));
            }
            catch (Exception ex)
            {
                ex.WriteToOutput();
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
                await Repository.InitializeDatabaseAsync();
                await MViewModel.GetListsAsync();

                StartIndexingTimer();

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

                    MusicLibrary.DefinitionChanged += OnLibraryDefinitionChanged;
                    VideoLibrary.DefinitionChanged += OnLibraryDefinitionChanged;
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            IsLoaded = true;

            return rootFrame;
        }

        private static async void OnLibraryDefinitionChanged(StorageLibrary sender, object args)
        {
            // Prevent duplicate calls.
            if (IsLoaded)
                await Task.Run(async () => await MViewModel.StartFullCrawlAsync());
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

    // Data source/ViewModel initialization
    public sealed partial class App
    {
        private static LastFMViewModel OnLFMRequested()
        {
            var lfm = new LastFMViewModel(LastFM.Key, LastFM.Secret);
            lfm.TryLoadCredentials(LastFM.VaultResource);
            return lfm;
        }

        private static MediaPlaybackViewModel OnMPViewModelRequested()
        {
            var mpvm = new MediaPlaybackViewModel();

            if (!EqualizerEffect.Initialized)
            {
                var eq = EqualizerEffect.Current;
                eq.InitializeBands(SViewModel.EqualizerGain);
                eq.IsEnabled = SViewModel.EqualizerEnabled;
            }

            mpvm.AddEffect(new(typeof(EqualizerEffect), false, true, null));
            return mpvm;
        }

        private static StorageLibrary OnStorageLibraryRequested(KnownLibraryId id)
        {
            var library = StorageLibrary.GetLibraryAsync(id).Get();
            return library;
        }
    }
}
