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
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.Media.Playback;
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
        private static Timer IndexingTimer;

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
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            await ActivateAsync(e.PreviousExecutionState, e.PrelaunchActivated);
        }

        protected override async void OnActivated(IActivatedEventArgs e)
        {
            switch (e.Kind)
            {
                case ActivationKind.StartupTask:
                    {
                        await ActivateAsync(e.PreviousExecutionState, false);
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

        protected override async void OnFileActivated(FileActivatedEventArgs args)
        {
            await ActivateAsync(args.PreviousExecutionState, false);

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
            var window = Window.Current;
            if (window.Content is not Frame rootFrame)
            {
                await Repository.InitializeDatabaseAsync();
                await MViewModel.GetListsAsync();

                rootFrame = new Frame();
                rootFrame.NavigationFailed += OnNavigationFailed;
                rootFrame.AllowDrop = true;
                rootFrame.DragOver += OnDragOver;
                rootFrame.Drop += OnDrop;

                SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

                // Restore the saved session state only when appropriate
                if ((previousState == ApplicationExecutionState.Terminated) ||
                    (previousState == ApplicationExecutionState.ClosedByUser &&
                    SViewModel.PickUp))
                {
                    try
                    {
                        await SuspensionManager.RestoreAsync();
                    }
                    catch (Exception e)
                    {
                        e.WriteToOutput();
                    }
                }

                window.Content = rootFrame;
            }

            return rootFrame;
        }

        private async void OnDrop(object sender, DragEventArgs e)
        {
            if (!e.DataView.Contains(StandardDataFormats.StorageItems))
                return;

            var files = (await e.DataView.GetStorageItemsAsync()).OfType<StorageFile>();

            int i = 0;
            foreach (var file in files)
            {
                MediaPlaybackItem itm = null;
                if (QueryPresets.SongQueryOptions.FileTypeFilter.Contains(file.FileType))
                    itm = await file.GetSongAsync();
                else if (QueryPresets.VideoQueryOptions.FileTypeFilter.Contains(file.FileType))
                    itm = await file.GetVideoAsync();

                if (itm != null)
                {
                    if (i == 0)
                    {
                        MPViewModel.ResetPlayback();
                        MPViewModel.Player.Play();

                        i++;
                    }

                    MPViewModel.PlaybackList.Items.Add(itm);
                }
            }
        }

        private void OnDragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Link;

            if (e.DragUIOverride != null)
            {
                e.DragUIOverride.Caption = "Play media";
                e.DragUIOverride.IsContentVisible = true;
            }
        }

        /// <summary>
        /// Activates the app's window and puts content in there
        /// if necessary.
        /// </summary>
        /// <param name="previousState">Previous app execution state.</param>
        /// <param name="prelaunched">Whether the app was prelaunched.</param>
        private async Task ActivateAsync(ApplicationExecutionState previousState, bool prelaunched)
        {
            var rootFrame = await InitializeWindowAsync(previousState);
            if (!prelaunched)
            {
                CoreApplication.EnablePrelaunch(true);
                if (rootFrame.Content == null)
                {
                    _ = !SViewModel.SetupCompleted
                        ? rootFrame.Navigate(typeof(SetupPage))
                        : rootFrame.Navigate(typeof(MainPage));
                }

                Window.Current.Activate();
            }
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
            var deferral = e.SuspendingOperation.GetDeferral();
            try
            {
                if (_navDataSource.IsValueCreated)
                    await NavDataSource.SerializeGroupsAsync();

                await SuspensionManager.SaveAsync();
            }
            catch (Exception ex)
            {
                ex.WriteToOutput();
            }
            finally
            {
                deferral.Complete();
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

    // Indexing
    public sealed partial class App
    {
        public static void RestartIndexingTimer()
        {
            if (IndexingTimer != null && IndexingTimer.Enabled)
                IndexingTimer.Stop();

            if (!SViewModel.IndexingTimerEnabled)
                return;

            var span = TimeSpan.FromMinutes(SViewModel.IndexingTimerInterval);
            IndexingTimer = new(span.TotalMilliseconds)
            {
                AutoReset = true
            };

            IndexingTimer.Elapsed += IndexingTimer_Elapsed;
            IndexingTimer.Start();
        }

        public static async Task InitializeChangeTrackingAsync()
        {
            RestartIndexingTimer();
            _ = await KnownFolders.MusicLibrary.
                TrackForegroundAsync(QueryPresets.SongQueryOptions,
                SongsTracker.MusicQueryResultChanged);

            _ = await KnownFolders.VideosLibrary.
                TrackForegroundAsync(QueryPresets.VideoQueryOptions,
                VideosTracker.VideosLibrary_ContentsChanged);

            MusicLibrary.DefinitionChanged += OnLibraryDefinitionChanged;
            VideoLibrary.DefinitionChanged += OnLibraryDefinitionChanged;
        }

        private static async void OnLibraryDefinitionChanged(StorageLibrary sender, object args)
        {
            await Task.Run(async () => await MViewModel.StartFullCrawlAsync());
        }

        private static async void IndexingTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                await Task.Run(async () => await MViewModel.StartFullCrawlAsync());
            }
            catch (Exception ex)
            {
                ex.WriteToOutput();
            }
        }
    }

    // Error handling
    public sealed partial class App
    {
        /// <summary>
        /// Shows a toast with the provided exception data.
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

            ToastNotification notification = new(content.GetXml());
            ToastNotificationManager.CreateToastNotifier().Show(notification);
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

        private void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            e.Exception.WriteToOutput();
            ShowExceptionToast(e.Exception);
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails.
        /// </summary>
        /// <param name="sender">The Frame which failed navigation.</param>
        /// <param name="e">Details about the navigation failure.</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }
    }
}
