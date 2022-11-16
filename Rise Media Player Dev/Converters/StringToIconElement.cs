using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Rise.App.Converters
{
    public sealed class StringToIconElement : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string str)
            {
                if (Uri.TryCreate(str, UriKind.Absolute, out var uri))
                {
                    return new BitmapIcon
                    {
                        ShowAsMonochrome = false,
                        UriSource = uri
                    };
                }
                return new FontIcon { Glyph = str };
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
