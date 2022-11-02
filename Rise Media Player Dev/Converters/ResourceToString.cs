using Rise.Common.Extensions.Markup;
using System;
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
            if (parameter != null)
            {
                string loader = parameter.ToString();
                if (loader != _loader)
                {
                    _loader = loader;
                    _currLoader = ResourceLoader.GetForViewIndependentUse(loader);
                }

                return _currLoader.GetString(resource);
            }

            return ResourceHelper.GetString(resource);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
