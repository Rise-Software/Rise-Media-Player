using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

namespace Rise.App.Converters
{
    public class ResourceToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string resource = value.ToString();
            if (parameter != null)
            {
                string loader = parameter.ToString();
                return ResourceLoader.GetForViewIndependentUse(loader).GetString(resource);
            }

            return ResourceLoader.GetForViewIndependentUse().GetString(resource);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
