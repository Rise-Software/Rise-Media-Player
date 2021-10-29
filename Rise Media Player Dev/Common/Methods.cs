using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

namespace RMP.App.Common
{
    /// <summary>
    /// Methods that don't really fit anywhere else or can be used in multiple
    /// parts of the app, usually simple stuff like launching URIs and whatnot.
    /// </summary>
    public class Methods
    {
        /// <summary>
        /// Launchs an URI from a string.
        /// </summary>
        /// <param name="str">The URI string.</param>
        /// <returns>Whether or not the launch was successful.</returns>
        public static async Task<bool> LaunchURIAsync(string str)
        {
            return await Launcher.LaunchUriAsync(new Uri(str));
        }

        /// <summary>
        /// Creates a GET request.
        /// </summary>
        /// <param name="url">URL that's gonna get the request.</param>
        /// <returns>Response as a string.</returns>
        public static async Task<string> CreateGETRequestAsync(string url)
        {
            HttpClient client = new HttpClient();
            HttpRequestHeaderCollection headers = client.DefaultRequestHeaders;

            string httpResponseBody = null;

            string header = URLs.UserAgent;
            if (!headers.UserAgent.TryParseAdd(header))
            {
                throw new Exception("Invalid header value: " + header);
            }

            // Send the GET request asynchronously and retrieve the response as a string.
            try
            {
                // Send the GET request
                HttpResponseMessage httpResponse = await client.GetAsync(new Uri(url));
                httpResponse.EnsureSuccessStatusCode();
                httpResponseBody = await httpResponse.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message);
            }

            return httpResponseBody;
        }

        /// <summary>
        /// Checks whether or not the URL points to an image.
        /// </summary>
        /// <param name="url">URL to check</param>
        /// <returns>Whether or not the URL points to an image.</returns>
        public static async Task<bool> IsImageURLAsync(string url)
        {
            HttpClient client = new HttpClient();

            // Send the GET request asynchronously and retrieve the response as a string.
            try
            {
                // Send the GET request
                HttpResponseMessage httpResponse = await client.GetAsync(new Uri(url));
                httpResponse.EnsureSuccessStatusCode();
                HttpMediaTypeHeaderValue type = httpResponse.Content.Headers.ContentType;

                if (type.MediaType.ToLower(CultureInfo.InvariantCulture)
                    .StartsWith("image/"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }

            return false;
        }

        public static async Task SaveImage(string url, string filename)
        {
            HttpClient client = new HttpClient();
            IInputStream stream = await client.GetInputStreamAsync(new Uri(url));
            // BitmapImage bitmap = new BitmapImage(stream);
        }
    }
}
