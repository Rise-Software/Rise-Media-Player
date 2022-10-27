using System.Threading.Tasks;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;

namespace Rise.Storage
{
    public interface IFile : IBaseStorage
    {
        string Extension { get; }

        IRandomAccessStream Thumbnail { get; }

        MusicProperties MusicProperties { get; }

        VideoProperties VideoProperties { get; }

        Task<MusicProperties> GetMusicPropertiesAsync();

        Task<VideoProperties> GetVideoPropertiesAsync();
    }
}
