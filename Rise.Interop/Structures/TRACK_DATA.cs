using System.Runtime.InteropServices;

namespace Rise.Interop.Structures
{
    [StructLayout(LayoutKind.Sequential)]
    public struct TRACK_DATA
    {
        public byte Reserved;
        private byte BitMapped;

        public byte Control
        {
            get => (byte)(BitMapped & 0x0F);
            set => BitMapped = (byte)((BitMapped & 0xF0) | (value & 0x0F));
        }

        public byte Adr
        {
            get => (byte)((BitMapped & 0xF0) >> 4);
            set => BitMapped = (byte)((BitMapped & 0x0F) | (value << 4));
        }

        public byte TrackNumber;
        public byte Reserved1;

        /// <summary>
        /// Don't use array to avoid array creation
        /// </summary>
        public byte Address_0;
        public byte Address_1;
        public byte Address_2;
        public byte Address_3;
    };
}
