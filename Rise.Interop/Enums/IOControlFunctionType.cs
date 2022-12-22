namespace Rise.Interop.Enums
{
    /// <summary>
    /// An enumeration of common IO control functions.
    /// </summary>
    public enum IOControlFunctionType : ushort
    {
        IOCTL_CDROM_READ_TOC = 0x9000,
        IOCTL_STORAGE_CHECK_VERIFY = unchecked((ushort)0x002D4800),
        IOCTL_CDROM_RAW_READ = unchecked((ushort)0x0002403E),
        IOCTL_STORAGE_MEDIA_REMOVAL = unchecked((ushort)0x002D4804),
        IOCTL_STORAGE_EJECT_MEDIA = unchecked((ushort)0x002D4808),
        IOCTL_STORAGE_LOAD_MEDIA = unchecked((ushort)0x002D480C),
    }
}
