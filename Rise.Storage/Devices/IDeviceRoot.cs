using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rise.Storage.Devices
{
    public interface IDeviceRoot
    {
        /// <summary>
        /// Gets unknown root of the device.
        /// </summary>
        /// <returns>Device root</returns>
        Task<object> GetRootAsync();

        /// <summary>
        /// Tries to get root of the device as collection of possible storage objects.
        /// </summary>
        /// <returns>A collection of storage objects.</returns>
        Task<IEnumerable<IBaseStorage>?> DangerousGetRootAsync();
    }
}
