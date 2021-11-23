using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Rise.App.Converters
{
    class IntToColorBrush : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var intValue = (int)value;
            intValue = intValue - 5 * (intValue / 5);

            Brush brush = new SolidColorBrush(Colors.Transparent);
            if (intValue < -2 || intValue > 4) return brush;

            var uiSettings = new Windows.UI.ViewManagement.UISettings();
            Color color = uiSettings.GetColorValue(Windows.UI.ViewManagement.UIColorType.Accent);

            switch (intValue)
            {
                case -2:
                    brush = new SolidColorBrush(Color.FromArgb(25, color.R, color.G, color.B));
                    break;
                case -1:
                    brush = new SolidColorBrush(Colors.Transparent);
                    break;
                case 0:
                    brush = new SolidColorBrush(Color.FromArgb(25, 205, 92, 92));
                    break;
                case 1:
                    brush = new SolidColorBrush(Color.FromArgb(25, 138, 43, 226));
                    break;
                case 2:
                    brush = new SolidColorBrush(Color.FromArgb(25, 143, 188, 143));
                    break;
                case 3:
                    brush = new SolidColorBrush(Color.FromArgb(25, 100, 149, 237));
                    break;
                case 4:
                    brush = new SolidColorBrush(Color.FromArgb(25, 184, 135, 11));
                    break;
            }
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
