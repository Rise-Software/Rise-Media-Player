using System.Runtime.InteropServices;

namespace Rise.Interop.Structures
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PREVENT_MEDIA_REMOVAL
    {
        public byte PreventMediaRemoval = 0;

        public PREVENT_MEDIA_REMOVAL() {}
    }
}
