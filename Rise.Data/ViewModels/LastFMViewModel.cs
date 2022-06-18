using Rise.Common.Constants;
using Rise.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Windows.Security.Authentication.Web;
using Windows.Web.Http;

namespace Rise.Data.ViewModels
{
    public class LastFMViewModel : ViewModel
    {
        /// <summary>
        /// Session key used for the LastFM API.
        /// </summary>
        private string _sessionKey;

        private string _username;
        /// <summary>
        /// Username for the currently logged in user.
        /// </summary>
        public string Username
        {
            get => _username;
            private set => Set(ref _username, value);
        }

        /// <summary>
        /// Attempts to authenticate the user to last.fm.
        /// </summary>
        /// <returns>true if the authentication is successful,
        /// false otherwise.</returns>
        public async Task<bool> TryAuthenticateAsync()
        {
            var token = await GetTokenAsync();

            var uriBuilder = new StringBuilder();
            _ = uriBuilder.Append("https://www.last.fm/api/auth?api_key=");
            _ = uriBuilder.Append(LastFM.key);
            _ = uriBuilder.Append("&token=");
            _ = uriBuilder.Append(token);
            _ = uriBuilder.Append("&redirect_uri=");
            _ = uriBuilder.Append(Uri.EscapeDataString("https://www.google.com"));

            var startUri = new Uri(uriBuilder.ToString());
            Dictionary<string, string> args = new()
            {
                { "method", "auth.getSession" },
                { "api_key", LastFM.key },
                { "token", token }
            };

            var endUri = GetSignedUri(args);
            var result = await WebAuthenticationBroker.
                AuthenticateAsync(WebAuthenticationOptions.UseTitle, startUri, endUri);

            if (result.ResponseStatus != WebAuthenticationStatus.Success)
                return false;

            string response;
            using (var client = new HttpClient())
            {
                try
                {
                    response = await client.GetStringAsync(endUri);
                }
                catch (Exception)
                {
                    return false;
                }
            }

            var doc = new XmlDocument();
            doc.LoadXml(response);

            this._sessionKey = GetNodeFromResponse(doc, "/lfm/session/key");
            this.Username = GetNodeFromResponse(doc, "/lfm/session/name");
            return true;
        }

        private async Task<string> GetTokenAsync()
        {
            string m_strFilePath = URLs.LastFM + "auth.gettoken&api_key=" + LastFM.key;

            string response;
            using (var client = new HttpClient())
            {
                try
                {
                    response = await client.GetStringAsync(new Uri(m_strFilePath));
                }
                catch (Exception)
                {
                    return null;
                }
            }

            var doc = new XmlDocument();
            doc.LoadXml(response);
            return GetNodeFromResponse(doc, "/lfm/token");
        }

        private Uri GetSignedUri(Dictionary<string, string> args)
        {
            StringBuilder stringBuilder = new();
            _ = stringBuilder.Append("https://ws.audioscrobbler.com/2.0/?");
            
            foreach (var kvp in args)
                _ = stringBuilder.AppendFormat("{0}={1}&", kvp.Key, kvp.Value);

            _ = stringBuilder.Append("api_sig=" + SignCall(args));
            return new Uri(stringBuilder.ToString());
        }

        private string SignCall(Dictionary<string, string> args)
        {
            var sortedArgs = args.OrderBy(arg => arg.Key);

            string signature = sortedArgs.Select(pair => pair.Key + pair.Value).
                Aggregate((first, second) => first + second);
            signature += LastFM.secret;

            return signature.GetEncodedHash("MD5");
        }

        private string GetNodeFromResponse(XmlDocument doc, string node)
        {
            var selected = doc.DocumentElement.SelectSingleNode(node);
            return selected.InnerText;
        }
    }
}
