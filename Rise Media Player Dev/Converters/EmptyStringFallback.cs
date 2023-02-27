using System;
using Windows.UI.Xaml.Data;

namespace Rise.App.Converters;

/// <summary>
/// Returns the provided value if it's not an empty string,
/// returns the converter parameter otherwise.
/// </summary>
public sealed class EmptyStringFallback : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var str = value.ToString();

        if (string.IsNullOrEmpty(str) || int.TryParse(str, out int result) && result <= 0)
            return parameter;
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
