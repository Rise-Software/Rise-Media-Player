using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace Rise.App.Helpers
{
    public static class ImageExtensions
    {
        /// <summary>
        /// Converts a <see cref="StorageItemThumbnail"/> to an <see cref="SoftwareBitmap"/>.
        /// </summary>
        /// <param name="thumbnail"><see cref="StorageItemThumbnail"/> to convert.</param>
        /// <param name="pixelFormat">The pixel format to use. By default it uses Bgra8
        /// to allow converting to a <see cref="SoftwareBitmapSource"/> easily.</param>
        /// <param name="alphaMode">The alpha mode to use. By default it uses Premultiplied
        /// to allow converting to a <see cref="SoftwareBitmapSource"/> easily.</param>
        /// <returns>The <see cref="SoftwareBitmap"/>. If the item has no thumbnail, returns
        /// null.</returns>
        public static async Task<SoftwareBitmap> AsSoftwareBitmapAsync(this StorageItemThumbnail thumbnail)
        {
            return await AsSoftwareBitmap(thumbnail);
        }

        private static async Task<SoftwareBitmap> AsSoftwareBitmap(StorageItemThumbnail thumbnail)
        {
            // Create the decoder from the stream
            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(thumbnail);

            // Get the SoftwareBitmap representation of the file
            return await decoder.GetSoftwareBitmapAsync();
        }

        /// <summary>
        /// Converts a <see cref="SoftwareBitmap"/> to an <see cref="SoftwareBitmapSource"/>.
        /// </summary>
        /// <param name="softwareBitmap"><see cref="SoftwareBitmap"/> to convert.</param>
        /// <returns>The <see cref="SoftwareBitmapSource"/>.</returns>
        public static async Task<SoftwareBitmapSource> AsBitmapSourceAsync(this SoftwareBitmap softwareBitmap)
        {
            return await AsBitmapSource(softwareBitmap);
        }

        private static async Task<SoftwareBitmapSource> AsBitmapSource(SoftwareBitmap softwareBitmap)
        {
            if (softwareBitmap.BitmapPixelFormat != BitmapPixelFormat.Bgra8 ||
                softwareBitmap.BitmapAlphaMode == BitmapAlphaMode.Straight)
            {
                softwareBitmap = SoftwareBitmap.Convert(softwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
            }

            var source = new SoftwareBitmapSource();
            await source.SetBitmapAsync(softwareBitmap);

            return source;
        }

        /// <summary>
        /// Creates an <see cref="IRandomAccessStream"/> from a <see cref="SoftwareBitmap"/>.
        /// </summary>
        /// <param name="softwareBitmap"><see cref="SoftwareBitmap"/> to convert.</param>
        /// <returns>The resulting stream.</returns>
        public static async Task<IRandomAccessStream> GetStreamAsync(this SoftwareBitmap softwareBitmap)
        {
            return await GetStream(softwareBitmap);
        }

        private static async Task<IRandomAccessStream> GetStream(SoftwareBitmap softwareBitmap)
        {
            IRandomAccessStream stream = new MemoryStream().AsRandomAccessStream();

            // Create an encoder with the desired format
            BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);

            // Set the software bitmap
            encoder.SetSoftwareBitmap(softwareBitmap);

            // Set additional encoding parameters, if needed
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
                        throw;
                }
            }

            if (encoder.IsThumbnailGenerated == false)
            {
                await encoder.FlushAsync();
            }

            return stream;
        }

        /// <summary>
        /// Creates a <see cref="byte[]"/> from a <see cref="SoftwareBitmap"/>.
        /// </summary>
        /// <param name="soft"><see cref="SoftwareBitmap"/> to convert.</param>
        /// <returns>The resulting byte array.</returns>
        public static async Task<byte[]> AsByteArrayAsync(this SoftwareBitmap soft)
        {
            return await AsByteArray(soft);
        }

        private static async Task<byte[]> AsByteArray(SoftwareBitmap soft)
        {
            byte[] array = null;
            using (var ms = new InMemoryRandomAccessStream())
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, ms);
                encoder.SetSoftwareBitmap(soft);

                try
                {
                    await encoder.FlushAsync();
                }
                catch (Exception)
                {
                    return new byte[0];
                }

                array = new byte[ms.Size];
                await ms.ReadAsync(array.AsBuffer(), (uint)ms.Size, InputStreamOptions.None);
            }

            return array;
        }

        /// <summary>
        /// Creates a <see cref="BitmapImage"/> from a <see cref="byte[]"/>.
        /// </summary>
        /// <param name="bytes"></param>
        /// <remarks>This is mostly for views and controls. If possible, don't
        /// create bitmaps outside the UI thread or fire async operations on
        /// value converters.</remarks>
        public static async Task<BitmapImage> ToBitmapImageAsync(this byte[] bytes)
        {
            return await ToBitmapImage(bytes);
        }

        private static async Task<BitmapImage> ToBitmapImage(byte[] bytes)
        {
            BitmapImage image = new BitmapImage();
            using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                await stream.WriteAsync(bytes.AsBuffer());
                stream.Seek(0);
                await image.SetSourceAsync(stream);
            }

            return image;
        }

        /// <summary>
        /// Creates a <see cref="BitmapImage"/> from a <see cref="byte[]"/>.
        /// </summary>
        /// <param name="bytes"></param>
        /// <remarks>This is mostly for value converters. If possible, use the
        /// <see cref="ToBitmapImageAsync(byte[])"/> method instead.</remarks>
        public static BitmapImage AsBitmapImage(this byte[] bytes)
        {
            BitmapImage bitmap = new BitmapImage();
            var buff = bytes.AsBuffer();

            using (var stream = buff.AsStream())
            {
                bitmap.SetSource(stream.AsRandomAccessStream());
            }

            return bitmap;
        }
    }
}
