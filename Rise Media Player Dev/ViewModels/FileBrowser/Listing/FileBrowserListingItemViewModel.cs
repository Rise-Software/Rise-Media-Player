using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using Rise.App.Messages.FileBrowser;
using Rise.App.Services;
using Rise.Storage;
using System.Threading.Tasks;
using Rise.Common.Enums;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Rise.App.Storage;
using System;

namespace Rise.App.ViewModels.FileBrowser.Listing
{
    [ObservableObject]
    public sealed partial class FileBrowserListingItemViewModel
    {
        private readonly IMessenger _messenger;
        private readonly IBaseStorage _storage;

        private IFileSystemShellService FileSystemShellService { get; } = Ioc.Default.GetRequiredService<IFileSystemShellService>();

        [ObservableProperty]
        private FileBrowserSectionType _SectionType;

        [ObservableProperty]
        private string _Name;

        public MusicProperties MusicProperties => (_storage as IFile).MusicProperties;

        public VideoProperties VideoProperties => (_storage as IFile).VideoProperties;

        public IRandomAccessStream Thumbnail
        {
            get
            {
                if (_storage is not UwpFile file)
                    throw new NotSupportedException();

                if (_SectionType == FileBrowserSectionType.Music)
                    return file.MusicThumbnail;

                if (_SectionType == FileBrowserSectionType.Videos)
                    return file.VideoThumbnail;

                return file.Thumbnail;
            }
        }

        public FileBrowserListingItemViewModel(IBaseStorage storage, IMessenger messenger, FileBrowserSectionType sectionType)
        {
            _storage = storage;
            _messenger = messenger;
            _SectionType = sectionType;
            _Name = storage.Name;
        }

        public Task OpenAsync(CancellationToken cancellationToken = default)
        {
            if (_storage is IFile file)
                return FileSystemShellService.OpenFileAsync(file, cancellationToken); 

            if (_storage is IFolder folder)
                _messenger.Send(new FileBrowserDirectoryNavigationRequestedMessage(folder));

            return Task.CompletedTask;
        }
    }
}
