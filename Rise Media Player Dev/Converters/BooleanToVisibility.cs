using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Rise.App.Converters
{
    public class BooleanToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // Reversed result
            if (parameter is string param)
            {
                if (param == "Reverse")
                {
                    return (value is bool val && val) ? Visibility.Collapsed : Visibility.Visible;
                }
            }

            return (value is bool boolean && boolean) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            // Reversed result
            if (parameter is string param)
            {
                if (param == "0")
                {
                    return value is Visibility val && val == Visibility.Collapsed;
                }
            }

            return value is Visibility visibility && visibility == Visibility.Visible;
        }
    }

    public class BooleanToVisibility1 : IValueConverter
    {

        public bool Reverse
        {
            get; set;
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // Reversed result
            if (Reverse)
            {
                return (value is bool val && val) ? Visibility.Collapsed : Visibility.Visible;
            }

            return (value is bool boolean && boolean) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            // Reversed result
            if (parameter is string param)
            {
                if (param == "0")
                {
                    return value is Visibility val && val == Visibility.Collapsed;
                }
            }

            return value is Visibility visibility && visibility == Visibility.Visible;
        }
    }

    public class BooleanToArtistColumnVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // Reversed result
            if (parameter is string param)
            {
                if (param == "Reverse")
                {
                    return (value is bool val && val) ? new GridLength(0, GridUnitType.Star) : GridLength.Auto;
                }
            }

            return (value is bool boolean && boolean) ? GridLength.Auto : new GridLength(0, GridUnitType.Star);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException("Not implemented.");
        }
    }

    public static class BindlessBooleanToVisibility
    {
        public static Visibility AsVisibility(this bool value)
        {
            return value ? Visibility.Visible : Visibility.Collapsed;
        }

        public static bool AsBool(this Visibility value)
        {
            return value == Visibility.Visible;
        }
    }
}
