using Rise.Interop.Enums;
using System.Runtime.InteropServices;

namespace Rise.Interop.Structures
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RAW_READ_INFO
    {
        public long DiskOffset = 0;
        public uint SectorCount = 0;
        public TRACK_MODE_TYPE TrackMode = TRACK_MODE_TYPE.CDDA;

        public RAW_READ_INFO() {}
    }
}
