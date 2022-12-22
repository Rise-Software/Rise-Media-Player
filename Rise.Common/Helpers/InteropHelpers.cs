using System.Runtime.InteropServices;
using System;

namespace Rise.Common.Helpers
{
    public static class InteropHelpers
    {
        public static T DeserializeByteArray<T>(this byte[] bytes) where T : struct
        {
            T returnStruct;
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);

            try
            {
                returnStruct = Marshal.PtrToStructure<T>(handle.AddrOfPinnedObject());
            }
            finally
            {
                handle.Free();
            }

            return returnStruct;
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
