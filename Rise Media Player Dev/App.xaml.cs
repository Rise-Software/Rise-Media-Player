using System;
using System.IO;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using Rise.Repository.SQL;
using Microsoft.EntityFrameworkCore;
using Rise.Repository;
using RMP.App.ViewModels;
using Windows.ApplicationModel.Background;
using System.Diagnostics;
using Windows.Storage.Search;
using Windows.Storage.FileProperties;
using System.Collections.Generic;
using RMP.App.Settings;
using RMP.App.Dialogs;
using RMP.App.Views;

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

        private static readonly ChangeTracker changeTracker = new ChangeTracker();

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;

            UseSQLite();
            SetupFileTrackers();
        }

        /// <summary>
        /// Configures the app to use the SQLite data source.
        /// </summary>
        public static async void UseSQLite()
        {
            await ApplicationData.Current.LocalCacheFolder.CreateFileAsync("Files.db", CreationCollisionOption.OpenIfExists);
            string dbPath = Path.Combine(ApplicationData.Current.LocalCacheFolder.Path, "Files.db");
            var dbOptions = new DbContextOptionsBuilder<Context>().UseSqlite(
                "Data Source=" + dbPath);
            Repository = new SQLRepository(dbOptions);
            ViewModel = new MainViewModel();
        }

        /// <summary>
        /// Sets up the filesystem trackers for media library and future access list.
        /// </summary>
        public static async void SetupFileTrackers()
        {
            StorageFolder music = KnownFolders.MusicLibrary;

            // Create a query containing all the files the app will be tracking
            QueryOptions option = SongIndexer.SongQueryOptions;

            // Optimize indexing performance by using the Windows Indexer
            option.IndexerOption = IndexerOption.UseIndexerWhenAvailable;

            // Prefetch file properties
            option.SetPropertyPrefetch(PropertyPrefetchOptions.MusicProperties,
                SongIndexer.SongProperties);

            StorageFileQueryResult resultSet =
              music.CreateFileQueryWithOptions(option);

            // Indicate to the system the app is ready to change track
            await resultSet.GetFilesAsync(0, 1);

            // Attach an event handler for when something changes on the system
            resultSet.ContentsChanged += MediaLibrary_ContentsChanged;
            Debug.WriteLine("Registered!");
        }

        private async static void MediaLibrary_ContentsChanged(IStorageQueryResultBase sender, object args)
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
                    await changeTracker.ManageChange(change);
                }
                else if (change.IsOfType(StorageItemTypes.Folder))
                {
                    // No-op; not interested in folders
                }
                else
                {
                    if (change.ChangeType == StorageLibraryChangeType.Deleted)
                    {
                        await Repository.Songs.DeleteAsync(change.PreviousPath);
                    }
                }
            }

            // Mark that all the changes have been seen and for the change tracker
            // to never return these changes again
            await changeReader.AcceptChangesAsync();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
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
                    if (SetupSettings.SetupCompleted)
                    {
                        rootFrame.Navigate(typeof(SetupPage), e.Arguments);
                    }
                    else
                    {
                        rootFrame.Navigate(typeof(MainPage), e.Arguments);
                    }

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
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
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
