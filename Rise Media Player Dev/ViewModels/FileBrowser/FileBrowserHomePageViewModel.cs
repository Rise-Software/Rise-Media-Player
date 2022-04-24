using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Messaging;
using Rise.App.Services;
using Rise.Storage;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Rise.App.ViewModels.FileBrowser
{
    public sealed class FileBrowserHomePageViewModel : BaseFileBrowserPageViewModel
    {
        private IStorageService StorageService { get; } = Ioc.Default.GetRequiredService<IStorageService>();

        public ObservableCollection<FileBrowserDriveItemViewModel> Drives { get; }

        public FileBrowserHomePageViewModel(IMessenger messenger)
            : base(messenger)
        {
        }

        public async Task EnumerateDrivesAsync()
        {
            var driveFoldersTasks = new List<Task<IFolder?>>();
            foreach (var item in StorageService.EnumerateDrives())
            {
                driveFoldersTasks.Add(item.GetRootFolderAsync());
            }

            var driveFolders = await Task.WhenAll<IFolder?>(driveFoldersTasks);
            foreach (var item in driveFolders)
            {
                if (item is not null)
                {
                    Drives.Add(new(item));
                }
            }
        }
    }
}
