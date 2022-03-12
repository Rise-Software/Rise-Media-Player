using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Buffer = Windows.Storage.Streams.Buffer;

namespace Rise.Common.Extensions
{
    public static class ImageExtensions
    {
        /// <summary>
        /// Saves a <see cref="SoftwareBitmap"/> to a <see cref="StorageFile"/>.
        /// </summary>
        /// <param name="softwareBitmap"><see cref="SoftwareBitmap"/> to save.</param>
        /// <param name="outputFile"><see cref="StorageFile"/> where the <see cref="SoftwareBitmap"/>
        /// should be stored.</param>
        /// <returns>Whether or not the operation was successful.</returns>
        public static async Task<bool> SaveToFileAsync(this SoftwareBitmap softwareBitmap, StorageFile outputFile)
        {
            using IRandomAccessStream stream = await outputFile.OpenAsync(FileAccessMode.ReadWrite);

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

        /// <summary>
        /// Converts a <see cref="StorageItemThumbnail"/> to a <see cref="Buffer"/> and saves it.
        /// </summary>
        /// <param name="thumbnail"><see cref="StorageItemThumbnail"/> to convert.</param>
        /// <param name="filename">Filename of output image.</param>
        /// <returns>The image's filename. If the item has no thumbnail, returns "/".</returns>
        public static async Task<string> SaveToFileAsync(this StorageItemThumbnail thumbnail,
            string filename,
            CreationCollisionOption collisionOption = CreationCollisionOption.ReplaceExisting)
        {
            if (thumbnail != null && thumbnail.Type == ThumbnailType.Image)
            {
                StorageFile destinationFile = await ApplicationData.Current.LocalFolder.
                    CreateFileAsync(filename, collisionOption);

                Buffer buffer = new(Convert.ToUInt32(thumbnail.Size));
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
}
