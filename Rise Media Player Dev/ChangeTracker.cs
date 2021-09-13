using RMP.App.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace RMP.App
{
    public class ChangeTracker
    {
        public async Task ManageChange(StorageLibraryChange change)
        {
            StorageFile file = null;

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
                            App.ViewModel.Songs.RemoveAt(i);
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
                            App.ViewModel.Songs.RemoveAt(i);
                        }
                    }
                    break;

                case StorageLibraryChangeType.MovedOutOfLibrary:
                    // Song got moved out of the library
                    for (int i = 0; i < App.ViewModel.Songs.Count; i++)
                    {
                        if (change.PreviousPath == App.ViewModel.Songs[i].Location)
                        {
                            App.ViewModel.Songs.RemoveAt(i);
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
                            App.ViewModel.Songs.RemoveAt(i);
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
    }
}
