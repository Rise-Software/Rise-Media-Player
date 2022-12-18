using System.Runtime.InteropServices;

namespace Rise.Interop.Structures
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CDROM_TOC
    {
        public ushort Length;
        public byte FirstTrack = 0;
        public byte LastTrack = 0;

        public TrackDataList TrackData;

        public CDROM_TOC()
        {
            TrackData = new();
            Length = (ushort)Marshal.SizeOf(this);
        }
    }
}
