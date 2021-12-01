using System;
using Windows.UI.Xaml.Data;

namespace Rise.App.Converters
{
    public class TimeSpanToString : IValueConverter
    {
        public static string Convert(TimeSpan value, string param = null)
        {
            if (param == null)
            {
                return value.ToString("mm\\:ss");
            }

            string time = "";
            switch (param)
            {
                case "D-S":
                    if (value.Days > 0)
                    {
                        if (value.Days == 1)
                        {
                            time += value.Days + " day, ";
                        }
                        else
                        {
                            time += value.Days + " days, ";
                        }
                    }

                    if (value.Hours > 0)
                    {
                        if (value.Hours == 1)
                        {
                            time += value.Hours + " hour, ";
                        }
                        else
                        {
                            time += value.Hours + " hours, ";
                        }
                    }

                    if (value.Minutes > 0)
                    {
                        if (value.Minutes == 1)
                        {
                            time += value.Minutes + " minute, ";
                        }
                        else
                        {
                            time += value.Minutes + " minutes, ";
                        }
                    }

                    if (value.Seconds == 1)
                    {
                        time += value.Seconds + " second";
                    }
                    else
                    {
                        time += value.Seconds + " seconds";
                    }
                    break;

                default:
                    time = value.ToString("mm\\:ss");
                    break;
            }

            return time;
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            TimeSpan span = (TimeSpan)value;
            if (parameter == null)
            {
                return span.ToString("mm\\:ss");
            }

            return Convert(span, parameter.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
