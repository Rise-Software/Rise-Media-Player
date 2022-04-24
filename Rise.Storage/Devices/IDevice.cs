using System.Threading.Tasks;

namespace Rise.Storage.Devices
{
    // TODO: Parts of this interface are not complete.
    public interface IDevice
    {
        string Name { get; }

        /// <summary>
        /// Pings the device for response.
        /// </summary>
        /// <returns>true if the device can be accessed, otherwise false.</returns>
        Task<bool> PingAsync();

        // TODO: This API might not be ideal
        Task<IDeviceRoot?> GetRootAsync();
    }
}
