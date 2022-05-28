using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Messaging;
using Rise.App.Services;
using System.Collections.ObjectModel;

namespace Rise.App.ViewModels.FileBrowser.Pages
{
    public sealed class FileBrowserHomePageViewModel : BaseFileBrowserPageViewModel
    {
        private IStorageService StorageService { get; } = Ioc.Default.GetRequiredService<IStorageService>();

        public ObservableCollection<FileBrowserDriveItemViewModel> Drives { get; }

        public FileBrowserHomePageViewModel(IMessenger messenger)
            : base(messenger)
        {
            Drives = new();
        }

        public void EnumerateDrives()
        {
            foreach (var item in StorageService.EnumerateDrives())
            {
                Drives.Add(new(item, Messenger));
            }
        }
    }
}
