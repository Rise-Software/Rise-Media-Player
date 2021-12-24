﻿using System;
using Windows.UI.Xaml.Data;

namespace Rise.App.Converters
{
    public class DecimalPointToPercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double actualValue = (double)value;
            bool isPercentageVisible = false;
            if (parameter is string param)
            {
                if (param == "WithPercentage")
                {
                    isPercentageVisible = true;
                }
            }
            return $"{Math.Floor(actualValue * 100)}{(isPercentageVisible ? "%" : "")}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class UintToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            uint actualValue = (uint)value;
            string str;

            if (actualValue == 0)
            {
                str = "Unknown";
            }
            else str = actualValue.ToString();

            return str;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
