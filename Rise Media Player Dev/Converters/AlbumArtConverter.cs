using Rise.App.ViewModels;
using Rise.Common.Constants;
using System;
using Windows.Foundation;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace Rise.App.Converters
{
    /// <summary>
    /// Gets the album art from the provided album.
    /// </summary>
    public sealed class AlbumArtConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var img = new BitmapImage();
            if (value is AlbumViewModel album)
            {
                uint size = 134;
                if (parameter is string param)
                    size = uint.Parse(param);

                // Converted to async operation to get the Completed delegate
                // These callbacks are used to avoid locking the UI
                var getStrm = album.GetThumbnailAsync(ThumbnailMode.MusicView, size).AsAsyncOperation();
                getStrm.Completed = (info, result) =>
                {
                    // In case of an error, we just use the default thumbnail
                    if (info.Status == AsyncStatus.Completed)
                    {
                        var strm = info.GetResults();
                        var setOp = img.SetSourceAsync(strm);

                        setOp.Completed = (info, _) =>
                        {
                            strm.Dispose();
                            info.Close();
                        };
                    }
                    else
                    {
                        img.UriSource = new(URIs.AlbumThumb);
                    }

                    getStrm.Close();
                };
            }

            return img;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}