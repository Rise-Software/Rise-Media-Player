using Rise.App.Common;
using Rise.App.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Windows.Security.Authentication.Web;

namespace Rise.App.Helpers
{
    public class AccountsHelper
    {
        public static string MD5(string toHash)
        {
            byte[] textBytes = Encoding.UTF8.GetBytes(toHash);
            System.Security.Cryptography.MD5CryptoServiceProvider cryptHandler = new();
            byte[] hash = cryptHandler.ComputeHash(textBytes);
            return hash.Aggregate("", (current, a) => current + a.ToString("x2"));
        }
        public static string GetSignature(Dictionary<string, string> parameters)
        {
            string result = string.Empty;
            IOrderedEnumerable<KeyValuePair<string, string>> data = parameters.OrderBy(x => x.Key);
            foreach (KeyValuePair<string, string> s in data)
            {
                result += s.Key + s.Value;
            }
            System.Security.Cryptography.MD5CryptoServiceProvider cryptHandler = new();
            result += LastFM.secret;
            result = MD5(result);
            return result;
        }

        public static string GetCurrentUnixTimestamp()
        {
            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            return ((int)t.TotalSeconds).ToString();
        }
    }

    public class LastFMHelper
    {
        public static XmlDocument xmlDoc = new();
        public static Uri startUri;

        public static async Task LogIn()
        {
            string Token = await Task.Run(GetToken);
            string uriToLaunch = "https://www.last.fm/api/auth?api_key=" + LastFM.key + "&token=" + Token + "&redirect_uri=" + Uri.EscapeDataString("https://www.google.com");
            startUri = new Uri(uriToLaunch);
            Dictionary<string, string> args = new()
            {
                { "method", "auth.getSession" },
                { "api_key", LastFM.key },
                { "token", Token }
            };
            string url = GetSignedURI(args, true);
            Uri endUri = new(url);
            WebAuthenticationResult webAuthenticationResult =
                await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.UseTitle, startUri, endUri);
            WebClient wc = new();
            string xml = wc.DownloadString(url);
            string key = await Task.Run(() => GetSessionKey(xml));
            string name = await Task.Run(() => GetUserName(xml));
            App.LMViewModel.SessionKey = key;
            MainPage.Current.AccountMenuText = name;
            wc.Dispose();
            Windows.Security.Credentials.PasswordVault vault = new();
            vault.Add(new Windows.Security.Credentials.PasswordCredential("RiseMP - LastFM account", name, key));
        }

        public static Task<string> GetToken()
        {
            string m_strFilePath = URLs.LastFM + "auth.gettoken&api_key=" + LastFM.key;
            WebClient wc = new();
            string xmlStr = wc.DownloadString(m_strFilePath);
            xmlDoc.LoadXml(xmlStr);
            XmlNode node = xmlDoc.DocumentElement.SelectSingleNode("/lfm/token");
            wc.Dispose();
            return Task.FromResult(node.InnerText);
        }

        public static string SignCall(Dictionary<string, string> args)
        {
            IOrderedEnumerable<KeyValuePair<string, string>> sortedArgs = args.OrderBy(arg => arg.Key);
            string signature =
                sortedArgs.Select(pair => pair.Key + pair.Value).
                Aggregate((first, second) => first + second);
            return AccountsHelper.MD5(signature + LastFM.secret);
        }
        public static string GetSignedURI(Dictionary<string, string> args, bool get)
        {
            StringBuilder stringBuilder = new();
            if (get)
            {
                _ = stringBuilder.Append("https://ws.audioscrobbler.com/2.0/?");
            }
            foreach (KeyValuePair<string, string> kvp in args)
            {
                _ = stringBuilder.AppendFormat("{0}={1}&", kvp.Key, kvp.Value);
            }
            _ = stringBuilder.Append("api_sig=" + SignCall(args));
            return stringBuilder.ToString();
        }

        public static async Task<bool> GetFileStatus()
        {
            Windows.Storage.StorageFolder appFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.IStorageItem file = await appFolder.TryGetItemAsync("userid.txt");
            return file != null;
        }
        public static Task<string> GetSessionKey(string stringxml)
        {
            xmlDoc.LoadXml(stringxml);
            XmlNode node = xmlDoc.DocumentElement.SelectSingleNode("/lfm/session/key");
            return Task.FromResult(node.InnerText);
        }
        public static Task<string> GetUserName(string stringxml)
        {
            xmlDoc.LoadXml(stringxml);
            XmlNode node = xmlDoc.DocumentElement.SelectSingleNode("/lfm/session/name");
            return Task.FromResult(node.InnerText);
        }

        public static void ScrobbleTrack(string artist, string track, string sessionKey, Action<string> onCompletion)
        {
            string currentTimestamp = AccountsHelper.GetCurrentUnixTimestamp();

            Dictionary<string, string> parameters = new();
            parameters.Add("artist[0]", artist);
            parameters.Add("track[0]", track);
            parameters.Add("timestamp[0]", currentTimestamp);
            parameters.Add("method", "track.scrobble");
            parameters.Add("api_key", LastFM.key);
            parameters.Add("sk", sessionKey);

            string signature = AccountsHelper.GetSignature(parameters);

            string comboUrl = string.Concat("https://ws.audioscrobbler.com/2.0/", "?method=track.scrobble", "&api_key=", LastFM.key,
            "&artist[0]=", artist, "&track[0]=", track, "&sk=", sessionKey,
            "&timestamp[0]=", currentTimestamp,
            "&api_sig=", signature);

            var client = new WebClient();
            client.UploadStringAsync(new Uri(comboUrl), string.Empty);
            client.UploadStringCompleted += (s, e) =>
            {
                try
                {
                    onCompletion(e.Result);
                }
                catch (WebException ex)
                {
                    HttpWebResponse response = (HttpWebResponse)ex.Response;
                    using StreamReader reader = new(response.GetResponseStream());
                    Debug.WriteLine(reader.ReadToEnd());
                }
            };
            client.Dispose();
        }
    }
}
