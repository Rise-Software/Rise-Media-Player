using Microsoft.EntityFrameworkCore;
using Rise.Repository;
using Rise.Repository.SQL;
using RMP.App.Settings.ViewModels;
using RMP.App.ViewModels;
using RMP.App.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace RMP.App
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
        #region Variables
        /// <summary>
        /// Gets the app-wide MainViewModel singleton instance.
        /// </summary>
        public static MainViewModel MViewModel { get; private set; }

        /// <summary>
        /// Gets the app-wide NowPlayingViewModel singleton instance.
        /// </summary>
        public static PlaybackViewModel PViewModel { get; private set; }

        /// <summary>
        /// Gets the app-wide SettingsViewModel singleton instance.
        /// </summary>
        public static SettingsViewModel SViewModel { get; private set; }

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
            InitDatabase();
        }

        /// <summary>
        /// Initializes the app's database and ViewModels.
        /// </summary>
        private async void InitDatabase()
        {
            _ = await ApplicationData.Current.LocalCacheFolder.CreateFileAsync("Files.db", CreationCollisionOption.OpenIfExists);
            string dbPath = Path.Combine(ApplicationData.Current.LocalCacheFolder.Path, "Files.db");
            DbContextOptionsBuilder<Context> dbOptions = new DbContextOptionsBuilder<Context>().UseSqlite(
                "Data Source=" + dbPath);

            Repository = new SQLRepository(dbOptions);

            MusicLibrary = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Music);
            MusicFolders = new List<StorageFolder>();

            await RefreshMusicLibrary();

            MViewModel = new MainViewModel();
            PViewModel = new PlaybackViewModel();
        }

        public static async Task RefreshMusicLibrary()
        {
            MusicFolders.Clear();
            foreach (StorageFolder folder in MusicLibrary.Folders)
            {
                MusicFolders.Add(folder);
            }

            foreach (StorageFolder folder in
                await KnownFolders.MusicLibrary.GetFoldersAsync())
            {
                MusicFolders.Add(folder);
            }
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (Debugger.IsAttached)
            {
                DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (!(Window.Current.Content is Frame rootFrame))
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

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
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            SuspendingDeferral deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        protected override async void OnFileActivated(FileActivatedEventArgs args)
        {
            if (!(Window.Current.Content is Frame rootFrame))
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();
                rootFrame.NavigationFailed += OnNavigationFailed;

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

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
                OpenInWindowAsync(AppWindowPresentationKind.Default, 320, 300);

            await PViewModel.StartPlayback(args.Files.GetEnumerator(), 0, args.Files.Count);
        }
    }
}
