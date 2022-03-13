namespace Rise.Common.Helpers
{
    public static class BindingHelpers
    {
        /* TODO: add useful binding functions and variables here */

        public static bool IsNotNull(object obj)
        {
            return obj != null;
        }

        public static bool IsNull(object obj)
        {
            return obj == null;
        }

        public static bool Reverse(bool boolean)
        {
            return !boolean;
        }
    }
}
