using System;
using Rise.Common.Extensions.Markup;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

namespace Rise.App.Converters
{
    public class ResourceToString : IValueConverter
    {
        private static ResourceLoader _currLoader;
        private static string _loader = string.Empty;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string resource = value.ToString();
            if (parameter is string param)
            {
                if (param != _loader)
                {
                    _loader = param;
                    _currLoader = ResourceLoader.GetForViewIndependentUse(param);
                }

                return _currLoader.GetString(resource);
            }

            throw new ArgumentException("For resources in the default location, use the ResourceHelper markup extension.", nameof(parameter));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
