using Rise.App.ViewModels;
using System;
using System.IO;
using Windows.UI.Xaml.Data;

namespace Rise.App.Converters
{
    public class VideoToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string data = "";
            if (value is VideoViewModel vid)
            {
                data += vid.Title + "\n";
                data += Path.GetExtension(vid.Location) + " file\n";
                data += TimeSpanToString.Convert(vid.Length, "D-S");
            }

            return data;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
