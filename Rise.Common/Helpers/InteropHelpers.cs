using System.Runtime.InteropServices;
using System;

namespace Rise.Common.Helpers
{
    public static class InteropHelpers
    {
        public static unsafe T DeserializeByteArray<T>(this byte[] bytes) where T : struct
        {
            fixed (byte* structure = &bytes[0])
                return *(T*)structure;
        }

        public static byte[] SerializeToByteArray<T>(T obj)
        {
            int size = Marshal.SizeOf(obj);
            byte[] arr = new byte[size];

            IntPtr ptr = IntPtr.Zero;

            try
            {
                ptr = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(obj, ptr, true);
                Marshal.Copy(ptr, arr, 0, size);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }

            return arr;
        }
    }
}
