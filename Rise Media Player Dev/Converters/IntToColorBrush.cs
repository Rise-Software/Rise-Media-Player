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
            if (intValue < 0 || intValue > 4) return brush;

            switch (intValue)
            {
                case 0:
                    brush = new SolidColorBrush(Color.FromArgb(10, 205, 92, 92));
                    break;
                case 1:
                    brush = new SolidColorBrush(Color.FromArgb(10, 138, 43, 226));
                    break;
                case 2:
                    brush = new SolidColorBrush(Color.FromArgb(10, 143, 188, 143));
                    break;
                case 3:
                    brush = new SolidColorBrush(Color.FromArgb(10, 100, 149, 237));
                    break;
                case 4:
                    brush = new SolidColorBrush(Color.FromArgb(10, 184, 135, 11));
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
