using RMP.App.ViewModels;
using System;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Storage;

namespace RMP.App
{
    public class ChangeTracker
    {
        public static async Task ManageSongChange(StorageLibraryChange change)
        {
            StorageFile file;

            // Temp variable used for instantiating StorageFiles for sorting if needed later
            switch (change.ChangeType)
            {
                // New File in the Library
                case StorageLibraryChangeType.Created:
                    // Song was created..?
                    file = (StorageFile)await change.GetStorageItemAsync();
                    await SongIndexer.AddSong(file);
                    break;

                case StorageLibraryChangeType.MovedIntoLibrary:
                    // Song was moved into the library
                    file = (StorageFile)await change.GetStorageItemAsync();
                    await SongIndexer.AddSong(file);
                    break;

                case StorageLibraryChangeType.MovedOrRenamed:
                    // Song was renamed/moved
                    file = (StorageFile)await change.GetStorageItemAsync();
                    for (int i = 0; i < App.ViewModel.Songs.Count; i++)
                    {
                        if (change.PreviousPath == App.ViewModel.Songs[i].Location)
                        {
                            App.ViewModel.Songs[i].Delete();
                            await SongIndexer.AddSong(file);
                        }
                    }
                    break;

                // File Removed From Library
                case StorageLibraryChangeType.Deleted:
                    // Song was deleted
                    for (int i = 0; i < App.ViewModel.Songs.Count; i++)
                    {
                        if (change.PreviousPath == App.ViewModel.Songs[i].Location)
                        {
                            App.ViewModel.Songs[i].Delete();
                        }
                    }
                    break;

                case StorageLibraryChangeType.MovedOutOfLibrary:
                    // Song got moved out of the library
                    for (int i = 0; i < App.ViewModel.Songs.Count; i++)
                    {
                        if (change.PreviousPath == App.ViewModel.Songs[i].Location)
                        {
                            App.ViewModel.Songs[i].Delete();
                        }
                    }
                    break;

                // Modified Contents
                case StorageLibraryChangeType.ContentsChanged:
                    // Song content was modified..?
                    file = (StorageFile)await change.GetStorageItemAsync();
                    for (int i = 0; i < App.ViewModel.Songs.Count; i++)
                    {
                        if (change.PreviousPath == App.ViewModel.Songs[i].Location)
                        {
                            App.ViewModel.Songs[i].Delete();
                            await SongIndexer.AddSong(file);
                        }
                    }
                    break;

                // Ignored Cases
                case StorageLibraryChangeType.EncryptionChanged:
                case StorageLibraryChangeType.ContentsReplaced:
                case StorageLibraryChangeType.IndexingStatusChanged:
                default:
                    // These are safe to ignore, I think
                    break;
            }
        }

        public static void HandleMusicFolderChanges(IObservableVector<StorageFolder> folders)
        {
            bool isInFolder = false;
            foreach (SongViewModel song in App.ViewModel.Songs)
            {
                foreach (StorageFolder folder in folders)
                {
                    if (song.Location.StartsWith(folder.Path))
                    {
                        isInFolder = true;
                        break;
                    }
                }

                if (!isInFolder)
                {
                    song.Delete();
                }

                isInFolder = false;
            }
        }
    }
}
