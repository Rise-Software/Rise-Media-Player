using System.Runtime.InteropServices;

namespace Rise.Common.Helpers
{
    public static class PathUtils
    {
        [DllImport("shlwapi.dll")]
        public static extern bool PathIsNetworkPath(string pszPath);
    }
}
