using Rise.Common.Enums;
using System;
using Windows.UI.Xaml.Data;

namespace Rise.App.Converters
{
    public class DisplayModeToBoolean : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter is not string)
                return true;

            return (((DisplayMode)value) == DisplayMode.List && parameter is string param1 && param1 == "List") || (((DisplayMode)value) == DisplayMode.Grid && parameter is string param2 && param2 == "Grid");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
