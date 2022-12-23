using Rise.Interop.Enums;
using Windows.Devices.Custom;

namespace Rise.Interop.Helpers
{
    public static class CommonIOControlCodes
    {
        /// <summary>
        /// Gets the control code of a specified type.
        /// </summary>
        /// <remarks>Values supported in this method come from the ntddstor.h and ntddcdrm.h SDK headers.</remarks>
        /// <param name="type">The control type.</param>
        /// <returns>The control code using the specified control type.</returns>
        public static IIOControlCode GetControlCode(IOControlType type)
        {
            return type switch
            {
                IOControlType.IOCTL_CDROM_READ_TOC => new IOControlCode(2, 0x0000, IOControlAccessMode.Read, IOControlBufferingMethod.Buffered),
                IOControlType.IOCTL_CDROM_RAW_READ => new IOControlCode(2, 0x000F, IOControlAccessMode.Read, IOControlBufferingMethod.DirectOutput),
                IOControlType.IOCTL_STORAGE_CHECK_VERIFY => new IOControlCode(0x0000002d, 0x0200, IOControlAccessMode.Read, IOControlBufferingMethod.Buffered),
                IOControlType.IOCTL_STORAGE_MEDIA_REMOVAL => new IOControlCode(0x0000002d, 0x0201, IOControlAccessMode.Read, IOControlBufferingMethod.Buffered),
                IOControlType.IOCTL_STORAGE_LOAD_MEDIA => new IOControlCode(0x0000002d, 0x0203, IOControlAccessMode.Read, IOControlBufferingMethod.Buffered),
                IOControlType.IOCTL_STORAGE_EJECT_MEDIA => new IOControlCode(0x0000002d, 0x0202, IOControlAccessMode.Read, IOControlBufferingMethod.Buffered),
                _ => null,
            };
        }
    }
}
