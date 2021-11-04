using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.System;
using Buffer = Windows.Storage.Streams.Buffer;

namespace RMP.App.Common
{
    public class FileHelpers
    {
        /// <summary>
        /// Launchs an <see cref="Uri"/> from a <see cref="string"/>.
        /// </summary>
        /// <param name="str">The <see cref="Uri"/> <see cref="string"/>.</param>
        /// <returns>Whether or not the launch was successful.</returns>
        public static async Task<bool> LaunchUriAsync(string str)
            => await Launcher.LaunchUriAsync(new Uri(str));

        /// <summary>
        /// Saves a <see cref="SoftwareBitmap"/> to a <see cref="StorageFile"/>.
        /// </summary>
        /// <param name="softwareBitmap"><see cref="SoftwareBitmap"/> to save.</param>
        /// <param name="outputFile"><see cref="StorageFile"/> where the <see cref="SoftwareBitmap"/>
        /// should be stored.</param>
        /// <returns>Whether or not the operation was successful.</returns>
        public static async Task<bool> SaveSoftwareBitmapToFile(SoftwareBitmap softwareBitmap, StorageFile outputFile)
        {
            using (IRandomAccessStream stream = await outputFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                // Create an encoder with the desired format
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);

                // Set the software bitmap
                encoder.SetSoftwareBitmap(softwareBitmap);
                encoder.IsThumbnailGenerated = true;

                try
                {
                    await encoder.FlushAsync();
                }
                catch (Exception err)
                {
                    const int WINCODEC_ERR_UNSUPPORTEDOPERATION = unchecked((int)0x88982F81);
                    switch (err.HResult)
                    {
                        case WINCODEC_ERR_UNSUPPORTEDOPERATION:
                            // If the encoder does not support writing a thumbnail, then try again
                            // but disable thumbnail generation.
                            encoder.IsThumbnailGenerated = false;
                            break;
                        default:
                            return false;
                    }
                }

                if (encoder.IsThumbnailGenerated == false)
                {
                    await encoder.FlushAsync();
                }

                return true;
            }
        }

        /// <summary>
        /// Convert StorageItemThumbnail to a BitmapImage and save it.
        /// </summary>
        /// <param name="thumbnail">StorageItemThumbnail to convert.</param>
        /// <param name="filename">Filename of output image.</param>
        /// <returns>The image's filename. If the item has no thumbnail, returns "/".</returns>
        public static async Task<string> SaveBitmapFromThumbnailAsync(StorageItemThumbnail thumbnail, string filename)
        {
            if (thumbnail != null && thumbnail.Type == ThumbnailType.Image)
            {
                StorageFile destinationFile = await ApplicationData.Current.LocalFolder.
                    CreateFileAsync(filename, CreationCollisionOption.GenerateUniqueName);

                Buffer buffer = new Buffer(Convert.ToUInt32(thumbnail.Size));

                IBuffer iBuf = await thumbnail.ReadAsync(buffer,
                    buffer.Capacity, InputStreamOptions.None);

                using (IRandomAccessStream strm = await
                    destinationFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    _ = await strm.WriteAsync(iBuf);
                }

                return Path.GetFileNameWithoutExtension(destinationFile.Path);
            }

            return "/";
        }
    }

    public static class FileExtensions
    {
        /// <summary>
        /// List of invalid characters in a filename.
        /// </summary>
        private static char[] _invalids = new char[] { '"', '<', '>', '|',
            '\0', '\x0001', '\x0002', '\x0003', '\x0004', '\x0005',
            '\x0006', '\a', '\b', '\t', '\n', '\v', '\f', '\r',
            '\x000e', '\x000f', '\x0010', '\x0011', '\x0012',
            '\x0013', '\x0014', '\x0015', '\x0016', '\x0017',
            '\x0018', '\x0019', '\x001a', '\x001b', '\x001c',
            '\x001d', '\x001e', '\x001f', ':', '*', '?', '\\', '/' };

        /// <summary>
        /// Replaces characters in <c>text</c> that are not allowed in 
        /// file names with the specified replacement character.
        /// </summary>
        /// <param name="text">Text to make into a valid filename. The same string is returned if it is valid already.</param>
        /// <param name="replacement">Replacement character, or null to simply remove bad characters.</param>
        /// <returns>A string that can be used as a filename. If the output string would otherwise be empty, returns "_".</returns>
        public static string AsValidFileName(this string text, char? replacement = '_')
        {
            StringBuilder sb = new StringBuilder(text.Length);
            char[] invalids = _invalids ?? (_invalids = Path.GetInvalidFileNameChars());
            bool changed = false;

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (invalids.Contains(c))
                {
                    changed = true;
                    char repl = replacement ?? '\0';
                    if (repl != '\0')
                    {
                        _ = sb.Append(repl);
                    }
                }
                else
                {
                    _ = sb.Append(c);
                }
            }

            return sb.Length == 0 ? "_" : changed ? sb.ToString() : text;
        }
    }
}
