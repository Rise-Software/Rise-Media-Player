using System;
using Windows.UI.Xaml.Data;

namespace RMP.App.Converters
{
    public class NullToBoolean : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter == null)
            {
                return value != null;
            }

            if (parameter.ToString() == "Reverse")
            {
                return value == null;
            }

            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
