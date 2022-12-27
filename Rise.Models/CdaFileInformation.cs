using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Rise.Models
{
    public class CdaFileInformation
    {
        public string Identifier { get; } = "CDDA";

        public TimeSpan RangePosition { get; private set; }

        public TimeSpan Duration { get; private set; }

        public static async Task<CdaFileInformation> GetFromFileAsync(IStorageFile file)
        {
            using var stream = await file.OpenReadAsync();
            var basicProperties = await file.GetBasicPropertiesAsync();

            byte[] buffer = new byte[basicProperties.Size];

            _ = await stream.ReadAsync(buffer.AsBuffer(), (uint)buffer.Length, InputStreamOptions.None);

            return new CdaFileInformation()
            {
                RangePosition = new TimeSpan(0, buffer[0x26], buffer[0x25]),
                Duration = new TimeSpan(0, buffer[0x2a], buffer[0x29])
            };
        }
    }
}
