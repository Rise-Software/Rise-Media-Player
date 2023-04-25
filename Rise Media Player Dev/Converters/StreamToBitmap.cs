using Rise.Common.Extensions;
using System;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace Rise.App.Converters
{
    public sealed class StreamToBitmap : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            IRandomAccessStream strm;
            if (value is IRandomAccessStream rand)
                strm = rand;
            else if (value is IRandomAccessStreamReference randRef)
                strm = randRef.OpenReadAsync().Get();
            else
                throw new ArgumentException($"The provided value must be of type {typeof(IRandomAccessStream)}.", nameof(value));

            var img = new BitmapImage();
            _ = img.SetSourceAsync(strm);

            return img;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
