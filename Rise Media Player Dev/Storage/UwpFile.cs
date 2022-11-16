using Rise.Storage;
using System.Threading.Tasks;
using System;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Rise.Common.Extensions;
using Windows.Storage.Streams;

namespace Rise.App.Storage
{
    internal sealed class UwpFile : UwpBaseStorage<StorageFile>, IFile
    {
        private string? _Extension;

        public string Extension
            => _Extension ??= System.IO.Path.GetExtension(storage.Name);

        public MusicProperties MusicProperties => storage.Properties.GetMusicPropertiesAsync().Get();

        public VideoProperties VideoProperties => storage.Properties.GetVideoPropertiesAsync().Get();

        public IRandomAccessStream Thumbnail => storage.GetThumbnailAsync(ThumbnailMode.ListView, 500).Get();

        public IRandomAccessStream MusicThumbnail => storage.GetThumbnailAsync(ThumbnailMode.MusicView, 500).Get();

        public IRandomAccessStream VideoThumbnail => storage.GetThumbnailAsync(ThumbnailMode.VideosView, 500).Get();

        public UwpFile(StorageFile storage)
            : base(storage)
        {
        }

        public override async Task<IFolder?> GetParentAsync()
        {
            try
            {
                var parent = await storage.GetParentAsync();
                return new UwpFolder(parent);
            }
            catch
            {
                return null;
            }
        }

        public Task<MusicProperties> GetMusicPropertiesAsync()
            => storage.Properties.GetMusicPropertiesAsync().AsTask();

        public Task<VideoProperties> GetVideoPropertiesAsync()
            => storage.Properties.GetVideoPropertiesAsync().AsTask();
    }
}
