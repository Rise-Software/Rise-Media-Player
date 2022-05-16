namespace Rise.Common.Helpers
{
    public static class BindingHelpers
    {
        /* TODO: add useful binding functions and variables here */

        public static bool IsNotNull(object obj)
        {
            return obj != null;
        }

        public static Windows.UI.Xaml.Visibility IsNotNullVis(object obj)
        {
            return obj != null ? Windows.UI.Xaml.Visibility.Visible : Windows.UI.Xaml.Visibility.Collapsed;
        }

        public static bool IsNull(object obj)
        {
            return obj == null;
        }

        public static bool Reverse(bool boolean)
        {
            return !boolean;
        }

        public static string ConcatString(string str1, string str2)
        {
            return $"{str1} {str2}";
        }

        public static string ConcatString(int integer, string str2)
        {
            return $"{integer} {str2}";
        }
    }
}
