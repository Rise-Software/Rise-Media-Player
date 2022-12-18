using Rise.Common.Helpers;
using Rise.Interop.Enums;
using Rise.Interop.Structures;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System;
using Windows.Devices.Custom;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;
using System.IO;

namespace Rise.Interop.Helpers
{
    /// <summary>
    /// Represents a CD-ROM drive.
    /// </summary>
    public sealed partial class CdRomDevice
    {
        private CustomDevice _customDevice;
        private CDROM_TOC _cdToc;
        private bool _tocValid;

        private const int NSECTORS = 13;
        private const int CB_CDDASECTOR = 2368;
        private const int CB_QSUBCHANNEL = 16;
        private const int CB_CDROMSECTOR = 2048;
        private const int CB_AUDIO = (CB_CDDASECTOR - CB_QSUBCHANNEL);

        /// <summary>
        /// Gets the underlying custom device.
        /// </summary>
        public CustomDevice Device => _customDevice;

        public CdRomDevice(CustomDevice device)
        {
            _customDevice = device;
        }

        /// <summary>
        /// Locks the CD drive.
        /// </summary>
        /// <returns>Whether the operation has succeeded or not.</returns>
        public async Task<bool> LockAsync()
        {
            IBuffer inputBuffer = new Windows.Storage.Streams.Buffer((uint)Marshal.SizeOf(typeof(PREVENT_MEDIA_REMOVAL)));

            var preventMediaRemoval = new PREVENT_MEDIA_REMOVAL()
            {
                PreventMediaRemoval = 1
            };

            await inputBuffer.AsStream().WriteAsync(InteropHelpers.SerializeToByteArray(preventMediaRemoval), 0, Marshal.SizeOf<PREVENT_MEDIA_REMOVAL>());

            var code = new IOControlCode(KnownDeviceTypes.Unknown,
                (ushort)ControlCode.IOCTL_STORAGE_MEDIA_REMOVAL,
                IOControlAccessMode.Read,
                IOControlBufferingMethod.DirectInput);

            return await _customDevice.TrySendIOControlAsync(code, inputBuffer, null);
        }

        /// <summary>
        /// Unlocks the CD drive.
        /// </summary>
        /// <returns>Whether the operation has succeeded or not.</returns>
        public async Task<bool> UnlockAsync()
        {
            IBuffer inputBuffer = new Windows.Storage.Streams.Buffer((uint)Marshal.SizeOf(typeof(PREVENT_MEDIA_REMOVAL)));

            var preventMediaRemoval = new PREVENT_MEDIA_REMOVAL()
            {
                PreventMediaRemoval = 0
            };

            await inputBuffer.AsStream().WriteAsync(InteropHelpers.SerializeToByteArray(preventMediaRemoval), 0, Marshal.SizeOf<PREVENT_MEDIA_REMOVAL>());

            var code = new IOControlCode(KnownDeviceTypes.Unknown,
                (ushort)ControlCode.IOCTL_STORAGE_MEDIA_REMOVAL,
                IOControlAccessMode.Read,
                IOControlBufferingMethod.DirectInput);

            return await _customDevice.TrySendIOControlAsync(code, inputBuffer, null);
        }

        /// <summary>
        /// Gets the CD-ROM table of contents.
        /// </summary>
        /// <param name="device">The device to send the IO controls to.</param>
        /// <returns>The table of contents.</returns>
        /// <exception cref="ArgumentNullException" />
        public async Task<CDROM_TOC?> GetTableOfContentsAsync()
        {
            IBuffer outputBuffer = new Windows.Storage.Streams.Buffer((uint)Marshal.SizeOf<CDROM_TOC>());

            var code = new IOControlCode(KnownDeviceTypes.Unknown,
                (ushort)ControlCode.IOCTL_CDROM_READ_TOC,
                IOControlAccessMode.Read,
                IOControlBufferingMethod.DirectOutput);

            if (await _customDevice.TrySendIOControlAsync(code, null, outputBuffer))
            {
                _cdToc = InteropHelpers.DeserializeByteArray<CDROM_TOC>(outputBuffer.ToArray());
                _tocValid = true;
            }

            return _cdToc;
        }

        /// <summary>
        /// Reads the CD-ROM sectors from the specified range.
        /// </summary>
        /// <param name="device">The device to send the IO controls to.</param>
        /// <returns>The bytes read.</returns>
        public async Task<byte[]> ReadSectorAsync(int sectorOffset, int numSectors, int bufferLength)
        {
            if (!_tocValid || (sectorOffset + numSectors) > GetEndSector(_cdToc.LastTrack) || bufferLength < CB_AUDIO * numSectors)
                return Array.Empty<byte>();

            var readInfo = new RAW_READ_INFO()
            {
                TrackMode = TRACK_MODE_TYPE.CDDA,
                SectorCount = (uint)(numSectors * CB_CDROMSECTOR),
                DiskOffset = sectorOffset * CB_CDROMSECTOR
            };

            IBuffer inputBuffer = new Windows.Storage.Streams.Buffer((uint)Marshal.SizeOf(typeof(RAW_READ_INFO)));

            await inputBuffer.AsStream().WriteAsync(InteropHelpers.SerializeToByteArray(readInfo), 0, Marshal.SizeOf<RAW_READ_INFO>());

            IBuffer outputBuffer = new Windows.Storage.Streams.Buffer((uint)numSectors * CB_AUDIO);

            var code = new IOControlCode(KnownDeviceTypes.Unknown,
                (ushort)ControlCode.IOCTL_CDROM_RAW_READ,
                IOControlAccessMode.Read,
                IOControlBufferingMethod.Buffered);

            _ = await _customDevice.TrySendIOControlAsync(code, inputBuffer, outputBuffer);

            return Array.Empty<byte>();
        }

        /// <summary>
        /// Reads the track as a stream.
        /// </summary>
        /// <param name="track">The track index.</param>
        /// <param name="start">The start time.</param>
        /// <param name="end">The end time.</param>
        /// <returns>The stream that contains the raw track audio.</returns>
        public Task<IRandomAccessStream> GetTrackStreamAsync(int track, TimeSpan start = default, TimeSpan end = default)
        {
            if (!_tocValid || track < _cdToc.FirstTrack || track > _cdToc.LastTrack)
                return null;

            var data = Array.Empty<byte>();

            var startSect = GetStartSector(track);
            var endSect = GetEndSector(track);

            if ((startSect += (int)start.TotalSeconds * 75) >= endSect)
                startSect -= (int)start.TotalSeconds * 75;

            if ((end.TotalSeconds > 0) && ((int)(startSect + end.TotalSeconds * 75) < endSect))
                endSect = startSect + (int)end.TotalSeconds * 75;

            if (data.Length >= (uint)(endSect - startSect) * CB_AUDIO)
                return ReadTrackImpl(track, start, end);
            else
                return Task.FromResult(new InMemoryRandomAccessStream() as IRandomAccessStream);
        }
    }

    // Helper methods
    public partial class CdRomDevice
    {
        private Task<IRandomAccessStream> ReadTrackImpl(int track, TimeSpan start = default, TimeSpan end = default)
        {
            if (!_tocValid || track < _cdToc.FirstTrack || track > _cdToc.LastTrack)
                return Task.FromResult<IRandomAccessStream>(null);

            return Task.Run(async () =>
            {
                var startSect = GetStartSector(track);
                var endSect = GetEndSector(track);

                if ((startSect += (int)start.TotalSeconds * 75) >= endSect)
                    startSect -= (int)start.TotalSeconds * 75;

                if ((end.TotalSeconds > 0) && ((int)(startSect + end.TotalSeconds * 75) < endSect))
                    endSect = startSect + (int)end.TotalSeconds * 75;

                var buffer = new Windows.Storage.Streams.Buffer((uint)(endSect * CB_AUDIO));

                uint bytesToRead = (uint)(endSect - startSect) * CB_AUDIO;
                byte[] data = new byte[CB_AUDIO * NSECTORS];
                bool readOk = true;
                int writePosition = 0;

                byte[] trackData = Array.Empty<byte>();

                for (int sector = startSect; (sector < endSect) && readOk; sector += NSECTORS)
                {
                    int sectorsToRead = ((sector + NSECTORS) < endSect) ? NSECTORS : (endSect - sector);
                    var dataSize = CB_AUDIO * sectorsToRead;

                    var readTrack = await ReadSectorAsync(sector, sectorsToRead, dataSize);

                    if (readTrack.Length > 0)
                    {
                        System.Buffer.BlockCopy(data, 0, trackData, writePosition, dataSize);
                        writePosition += dataSize;
                    }
                }

                using var memoryStream = new MemoryStream(trackData);
                using var stream = memoryStream.AsRandomAccessStream();
                stream.Seek(0);

                return stream;
            });
        }

        /// <summary>
        /// Returns the number of audio tracks on the CD
        /// </summary>
        /// <returns>The number of audio tracks on the CD, or -1 if the Table of Contents hasn't been read yet.</returns>
        public int GetNumAudioTracks()
        {
            if (!_tocValid)
                return -1;

            int tracks = 0;
            for (int i = _cdToc.FirstTrack - 1; i < _cdToc.LastTrack; i++)
            {
                if (_cdToc.TrackData[i].Control == 0)
                    tracks++;
            }
            return tracks;
        }

        /// <summary>
        /// Gets whether if the track in the specified index is a valid audio track.
        /// </summary>
        /// <remarks>
        /// The CD-ROM Table of Contents must be retrieved at least once before calling this method, or else it will return false regardless of what's available on the CD.
        /// </remarks>
        /// <param name="track">The index of the track.</param>
        /// <returns>
        /// Whether the track in the specified index is a valid audio track.
        /// </returns>
        public bool IsAudioTrack(int track)
        {
            if (_tocValid && (track >= _cdToc.FirstTrack) && (track <= _cdToc.LastTrack))
                return (_cdToc.TrackData[track - 1].Control & 4) == 0;

            return false;
        }

        /// <summary>
        /// Gets the location of the track's start sector.
        /// </summary>
        /// <param name="track">The track index.</param>
        /// <returns>The location of the start sector on the CD.</returns>
        private int GetStartSector(int track)
        {
            if (_tocValid && (track >= _cdToc.FirstTrack) && (track <= _cdToc.LastTrack))
            {
                TRACK_DATA td = _cdToc.TrackData[track - 1];
                return (td.Address_1 * 60 * 75 + td.Address_2 * 75 + td.Address_3) - 150;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Gets the location of the track's end sector.
        /// </summary>
        /// <param name="track">The track index.</param>
        /// <returns>The location of the start sector on the CD.</returns>
        private int GetEndSector(int track)
        {
            if (_tocValid && (track >= _cdToc.FirstTrack) && (track <= _cdToc.LastTrack))
            {
                TRACK_DATA td = _cdToc.TrackData[track];
                return (td.Address_1 * 60 * 75 + td.Address_2 * 75 + td.Address_3) - 151;
            }
            else
                return -1;
        }
    }
}
