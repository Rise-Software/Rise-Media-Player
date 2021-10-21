using System;
using System.Threading.Tasks;
using Windows.System;

namespace RMP.App
{
    /// <summary>
    /// Methods that don't really fit anywhere else, usually really
    /// simple stuff like launching URIs and whatnot.
    /// </summary>
    public class Methods
    {
        public static async Task<bool> LaunchURI(string str)
        {
            return await Launcher.LaunchUriAsync(new Uri(str));
        }
    }
}
