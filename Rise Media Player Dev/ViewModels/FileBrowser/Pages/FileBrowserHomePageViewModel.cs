using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using Rise.App.Services;
using System.Collections.ObjectModel;

namespace Rise.App.ViewModels.FileBrowser.Pages
{
    public sealed class FileBrowserHomePageViewModel : BaseFileBrowserPageViewModel
    {
        private static FileBrowserHomePageViewModel? Instance { get; set; }

        private IStorageService StorageService { get; } = Ioc.Default.GetRequiredService<IStorageService>();

        public ObservableCollection<FileBrowserDriveItemViewModel> Drives { get; }

        private FileBrowserHomePageViewModel(IMessenger messenger)
            : base(messenger)
        {
            Drives = new();
        }

        public void EnumerateDrives()
        {
            Drives.Clear();
            foreach (var item in StorageService.EnumerateDrives())
            {
                Drives.Add(new(item, Messenger));
            }
        }

        public static FileBrowserHomePageViewModel GetOrCreate(IMessenger messenger)
            => Instance ??= new(messenger);
    }
}
