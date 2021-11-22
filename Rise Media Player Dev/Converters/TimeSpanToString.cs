using System;
using Windows.UI.Xaml.Data;

namespace Rise.App.Converters
{
    public class TimeSpanToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            TimeSpan span = (TimeSpan)value;
            if (parameter == null)
            {
                return span.ToString("mm\\:ss");
            }

            string time = "";
            switch (parameter.ToString())
            {
                case "D-S":
                    if (span.Days > 0)
                    {
                        if (span.Days == 1)
                        {
                            time += span.Days + " day, ";
                        }
                        else
                        {
                            time += span.Days + " days, ";
                        }
                    }

                    if (span.Hours > 0)
                    {
                        if (span.Hours == 1)
                        {
                            time += span.Hours + " hour, ";
                        }
                        else
                        {
                            time += span.Hours + " hours, ";
                        }
                    }

                    if (span.Minutes > 0)
                    {
                        if (span.Minutes == 1)
                        {
                            time += span.Minutes + " minute, ";
                        }
                        else
                        {
                            time += span.Minutes + " minutes, ";
                        }
                    }

                    if (span.Seconds == 1)
                    {
                        time += span.Seconds + " second";
                    }
                    else
                    {
                        time += span.Seconds + " seconds";
                    }
                    break;

                default:
                    time = span.ToString("mm\\:ss");
                    break;
            }

            return time;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
