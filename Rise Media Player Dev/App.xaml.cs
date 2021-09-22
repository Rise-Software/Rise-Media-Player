using Microsoft.EntityFrameworkCore;
using Rise.Repository;
using Rise.Repository.SQL;
using RMP.App.Settings;
using RMP.App.ViewModels;
using RMP.App.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Search;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace RMP.App
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Gets the app-wide MainViewModel singleton instance.
        /// </summary>
        public static MainViewModel ViewModel { get; private set; }

        /// <summary>
        /// Pipeline for interacting with backend service or database.
        /// </summary>
        public static IRepository Repository { get; private set; }

        /// <summary>
        /// Gets the music library.
        /// </summary>
        public static StorageLibrary MusicLibrary { get; private set; }

        /// <summary>
        /// Gets the video library.
        /// </summary>
        public static StorageLibrary VideoLibrary { get; private set; }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
        }

        /// <summary>
        /// Configures the app to use the SQLite data source.
        /// </summary>
        public static async void UseSQLite()
        {
            _ = await ApplicationData.Current.LocalCacheFolder.CreateFileAsync("Files.db", CreationCollisionOption.OpenIfExists);
            string dbPath = Path.Combine(ApplicationData.Current.LocalCacheFolder.Path, "Files.db");
            DbContextOptionsBuilder<Context> dbOptions = new DbContextOptionsBuilder<Context>().UseSqlite(
                "Data Source=" + dbPath);
            Repository = new SQLRepository(dbOptions);
            ViewModel = new MainViewModel();
        }

        /// <summary>
        /// Sets up the filesystem trackers for media library.
        /// </summary>
        public static async void SetupFileTrackers()
        {
            MusicLibrary = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Music);
            VideoLibrary = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Videos);

            StorageFolder music = KnownFolders.MusicLibrary;
            StorageFolder videos = KnownFolders.VideosLibrary;

            // Create a query containing all the files the app will be tracking
            QueryOptions musicOption = SongIndexer.SongQueryOptions;
            // QueryOptions videosOption = SongIndexer.VideoQueryOptions;

            // Optimize indexing performance by using the Windows Indexer
            musicOption.IndexerOption = IndexerOption.UseIndexerWhenAvailable;
            // videosOption.IndexerOption = IndexerOption.UseIndexerWhenAvailable;

            // Prefetch file properties
            musicOption.SetPropertyPrefetch(PropertyPrefetchOptions.MusicProperties,
                SongIndexer.SongProperties);

            // videosOption.SetPropertyPrefetch(PropertyPrefetchOptions.MusicProperties,
            // SongIndexer.VideoProperties);

            StorageFileQueryResult musicResultSet =
              music.CreateFileQueryWithOptions(musicOption);

            // StorageFileQueryResult videosResultSet =
            // videos.CreateFileQueryWithOptions(videosOption);

            // Indicate to the system the app is ready to change track
            _ = await musicResultSet.GetFilesAsync(0, 1);
            // _ = await videosResultSet.GetFilesAsync(0, 1);

            // Attach an event handler for when something changes on the system
            musicResultSet.ContentsChanged += MusicLibrary_ContentsChanged;
            MusicLibrary.DefinitionChanged += MusicLibrary_DefinitionChanged;
            Debug.WriteLine("Registered trackers!");
            // videosResultSet.ContentsChanged += VideosLibrary_ContentsChanged;
        }

        /// <summary>
        /// Handle folders being removed/added from the music library.
        /// </summary>
        private static async void MusicLibrary_DefinitionChanged(StorageLibrary sender, object args)
        {
            Debug.WriteLine("Folder changes!");

            await SongIndexer.IndexAllSongs();
            ChangeTracker.HandleMusicFolderChanges(sender.Folders);
        }

        /// <summary>
        /// Handle changes in the user's music library.
        /// </summary>
        private static async void MusicLibrary_ContentsChanged(IStorageQueryResultBase sender, object args)
        {
            Debug.WriteLine("New change!");
            StorageFolder changedFolder = sender.Folder;
            StorageLibraryChangeTracker folderTracker = changedFolder.TryGetChangeTracker();
            folderTracker.Enable();

            StorageLibraryChangeReader changeReader = folderTracker.GetChangeReader();
            IReadOnlyList<StorageLibraryChange> changes = await changeReader.ReadBatchAsync();

            foreach (StorageLibraryChange change in changes)
            {
                if (change.ChangeType == StorageLibraryChangeType.ChangeTrackingLost)
                {
                    // Change tracker is in an invalid state and must be reset
                    // This should be a very rare case, but must be handled
                    folderTracker.Reset();
                    return;
                }

                if (change.IsOfType(StorageItemTypes.File))
                {
                    await ChangeTracker.ManageSongChange(change);
                }
                else if (change.IsOfType(StorageItemTypes.Folder))
                {
                    // Not interested in folders
                }
                else
                {
                    if (change.ChangeType == StorageLibraryChangeType.Deleted)
                    {
                        for (int i = 0; i < ViewModel.Songs.Count; i++)
                        {
                            if (change.PreviousPath == ViewModel.Songs[i].Location)
                            {
                                ViewModel.Songs[i].Delete();
                            }
                        }
                    }
                }
            }

            // Mark that all the changes have been seen and for the change tracker
            // to never return these changes again
            await changeReader.AcceptChangesAsync();
        }

        /// <summary>
        /// Handle changes in the user's video library.
        /// </summary>
        /* private static async void VideosLibrary_ContentsChanged(IStorageQueryResultBase sender, object args)
        {
            StorageFolder changedFolder = sender.Folder;
            StorageLibraryChangeTracker folderTracker = changedFolder.TryGetChangeTracker();
            folderTracker.Enable();

            StorageLibraryChangeReader changeReader = folderTracker.GetChangeReader();
            IReadOnlyList<StorageLibraryChange> changes = await changeReader.ReadBatchAsync();

            foreach (StorageLibraryChange change in changes)
            {
                if (change.ChangeType == StorageLibraryChangeType.ChangeTrackingLost)
                {
                    // Change tracker is in an invalid state and must be reset
                    // This should be a very rare case, but must be handled
                    folderTracker.Reset();
                    return;
                }

                if (change.IsOfType(StorageItemTypes.File))
                {
                    await changeTracker.ManageVideoChange(change);
                }
                else if (change.IsOfType(StorageItemTypes.Folder))
                {
                    // Not interested in folders
                }
                else
                {
                    if (change.ChangeType == StorageLibraryChangeType.Deleted)
                    {
                        for (int i = 0; i < ViewModel.Videos.Count; i++)
                        {
                            if (change.PreviousPath == ViewModel.Videos[i].Location)
                            {
                                ViewModel.Videos[i].Delete();
                            }
                        }
                    }
                }
            }

            // Mark that all the changes have been seen and for the change tracker
            // to never return these changes again
            await changeReader.AcceptChangesAsync();
        } */

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
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
                    UseSQLite();
                    SetupFileTrackers();

                    _ = !SetupSettings.SetupCompleted
                        ? rootFrame.Navigate(typeof(SetupPage), e.Arguments)
                        : rootFrame.Navigate(typeof(MainPage), e.Arguments);

                    await SongIndexer.IndexAllSongs();
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
    }
}
