using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Rise.Interop.Structures
{
    [StructLayout(LayoutKind.Sequential)]
    public struct TrackDataList : IEnumerable<TRACK_DATA>, IEnumerable
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.MaximumNumTracks * 8)]
        private byte[] Data;

        public TRACK_DATA this[int i]
        {
            get
            {
                if ((i < 0) | (i >= Constants.MaximumNumTracks))
                    throw new IndexOutOfRangeException();

                TRACK_DATA res;
                GCHandle handle = GCHandle.Alloc(Data, GCHandleType.Pinned);
                try
                {
                    IntPtr buffer = handle.AddrOfPinnedObject();
                    buffer = (IntPtr)(buffer.ToInt32() + (i * Marshal.SizeOf(typeof(TRACK_DATA))));
                    res = (TRACK_DATA)Marshal.PtrToStructure(buffer, typeof(TRACK_DATA));
                }
                finally
                {
                    handle.Free();
                }
                return res;
            }
        }
        public TrackDataList()
            => Data = new byte[Constants.MaximumNumTracks * Marshal.SizeOf(typeof(TRACK_DATA))];

        public IEnumerator<TRACK_DATA> GetEnumerator()
        {
            for (int i = 0; i < Data.Length * Marshal.SizeOf(typeof(TRACK_DATA)); i++)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
