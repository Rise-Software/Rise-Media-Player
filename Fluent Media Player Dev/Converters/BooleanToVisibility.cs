using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Fluent_Media_Player_Dev.Converters
{
    public class BooleanToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // Reversed result
            if (parameter is string param)
            {
                if (param == "0")
                {
                    return (value is bool val && val) ? Visibility.Collapsed : Visibility.Visible;
                }
            }

            return (value is bool boolean && boolean) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value is Visibility visibility && visibility == Visibility.Visible;
        }
    }

    public class BindlessBooleanToVisibility
    {
        public static Visibility BindlessConvert(object value)
        {
            return (value is bool boolean && boolean) ? Visibility.Visible : Visibility.Collapsed;
        }

        public static bool BindlessConvertBack(object value)
        {
            return value is Visibility visibility && visibility == Visibility.Visible;
        }
    }
}
