using Rise.Common.Constants;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;

namespace Rise.Common.Extensions
{
    public static class FileExtensions
    {
        /// <summary>
        /// Launchs an <see cref="Uri"/> from a <see cref="string"/>.
        /// </summary>
        /// <param name="str">The <see cref="Uri"/> <see cref="string"/>.</param>
        /// <returns>Whether or not the launch was successful.</returns>
        public static async Task<bool> LaunchAsync(this string str)
        {
            if (str.IsValidUri())
            {
                return await Launcher.LaunchUriAsync(new Uri(str));
            }

            return false;
        }

        /// <summary>
        /// Launchs an <see cref="Uri"/>.
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> to launch.</param>
        /// <returns>Whether or not the launch was successful.</returns>
        public static async Task<bool> LaunchAsync(this Uri uri)
            => await Launcher.LaunchUriAsync(uri);

        /// <summary>
        /// Checks whether or not the provided <see cref="string"/> is
        /// a valid <see cref="Uri"/>.
        /// </summary>
        /// <param name="str"><see cref="string"/> to check.</param>
        /// <param name="kind">The kind of <see cref="Uri"/> to check.
        /// Won't check for a specific type by default.</param>
        /// <returns>Whether or not the <see cref="string"/> is
        /// a valid <see cref="Uri"/>.</returns>
        public static bool IsValidUri(this string str,
            UriKind kind = UriKind.RelativeOrAbsolute)
            => Uri.TryCreate(str, kind, out _);

        /// <summary>
        /// Uses the provided <paramref name="baseUri"/> to make <paramref name="relativeUri"/>
        /// an absolute URI.
        /// If <paramref name="relativeUri"/> is already an absolute URI, no change is made.
        /// </summary>
        public static Uri ToAbsoluteUri(this Uri relativeUri, Uri baseUri)
        {
            return relativeUri.IsAbsoluteUri
                ? relativeUri
                : new Uri(baseUri, relativeUri);
        }

        /// <summary>
        /// Replaces characters in <c>text</c> that are not allowed in 
        /// file names with the specified replacement character.
        /// </summary>
        /// <param name="text">Text to make into a valid filename. The same string is returned if it is valid already.</param>
        /// <param name="replacement">Replacement character, or null to simply remove bad characters.</param>
        /// <returns>A string that can be used as a filename. If the output string would otherwise be empty, returns "_".</returns>
        public static string AsValidFileName(this string text, char? replacement = '_')
        {
            StringBuilder sb = new(text.Length);
            char[] invalids = InvalidChars.Invalids ??= Path.GetInvalidFileNameChars();
            bool changed = false;

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (invalids.Contains(c))
                {
                    changed = true;
                    char repl = replacement ?? '\0';
                    if (repl != '\0')
                    {
                        _ = sb.Append(repl);
                    }
                }
                else
                {
                    _ = sb.Append(c);
                }
            }

            return sb.Length == 0 ? "_" : changed ? sb.ToString() : text;
        }
    }
}
