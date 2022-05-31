using System.Threading.Tasks;

namespace Rise.Storage.Devices
{
    public interface IDrive
    {
        string Name { get; }

        string VolumeLabel { get; }

        long AvailableFreeSpace { get; }

        long TotalFreeSpace { get; }

        long TotalSize { get; }

        Task<IFolder?> GetRootFolderAsync();
    }
}
