using Rise.App.Common;
using Rise.App.Dialogs;
using Rise.App.ViewModels;
using Rise.App.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Windows.Security.Authentication.Web;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using static Rise.App.Common.Enums;

namespace Rise.App.Settings
{
    public sealed partial class MediaLibraryPage : Page
    {
        #region Variables

        XmlDocument xmlDoc = new();
        private SettingsViewModel ViewModel => App.SViewModel;

        public static MediaLibraryPage Current;

        private readonly FoldersDialog Dialog = new FoldersDialog();
        private readonly VFoldersDialog VDialog = new VFoldersDialog();

        public ContentDialog FolderDialog = new ContentDialog
        {
            Title = ResourceLoaders.MediaLibraryLoader.GetString("Folders"),
        };

        public ContentDialog VFolderDialog = new ContentDialog
        {
            Title = ResourceLoaders.MediaLibraryLoader.GetString("Folders"),
        };

        private readonly List<string> Deletion = new List<string>
        {
            ResourceLoaders.MediaLibraryLoader.GetString("OnlyApp"),
            ResourceLoaders.MediaLibraryLoader.GetString("Device")
        };
        #endregion

        public MediaLibraryPage()
        {
            InitializeComponent();
            Current = this;

            FolderDialog.Content = Dialog;
            VFolderDialog.Content = VDialog;

            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        private async void ChooseFolders_Click(object sender, RoutedEventArgs e)
            => _ = await FolderDialog.ShowAsync(ExistingDialogOptions.CloseExisting);

        private async void VChooseFolders_Click(object sender, RoutedEventArgs e)
            => _ = await VFolderDialog.ShowAsync(ExistingDialogOptions.CloseExisting);

        private void CommandBarButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            switch (button.Tag.ToString())
            {
                case "Insider":
                    _ = Frame.Navigate(typeof(InsiderPage));
                    SettingsDialogContainer.Breadcrumbs.Add
                        (ResourceLoaders.SidebarLoader.GetString("Ins"));
                    break;

                case "OnlineServices":
                    break;

            }
        }

        private void AdaptiveItemPane_Loaded(object sender, RoutedEventArgs e)
        {

        }
        public Task<string> GetToken()
        {
            string m_strFilePath = URLs.LastFM + "auth.gettoken&api_key=" + LastFM.key;
            string xmlStr;
            WebClient wc = new();
            xmlStr = wc.DownloadString(m_strFilePath);
            xmlDoc.LoadXml(xmlStr);
            XmlNode node = xmlDoc.DocumentElement.SelectSingleNode("/lfm/token");
            string okay = node.InnerText;
            return Task.FromResult(okay);
        }
        private Uri starturi;
        public static string SignCall(Dictionary<string, string> args)
        {
            IOrderedEnumerable<KeyValuePair<string, string>> sortedArgs = args.OrderBy(arg => arg.Key);
            string signature =
                sortedArgs.Select(pair => pair.Key + pair.Value).
                Aggregate((first, second) => first + second);
            return MD5(signature + LastFM.secret);
        }
        public static string GetSignedURI(Dictionary<string, string> args, bool get)
        {
            StringBuilder stringBuilder = new();
            if (get)
            {
                _ = stringBuilder.Append("http://ws.audioscrobbler.com/2.0/?");
            }
            foreach (KeyValuePair<string, string> kvp in args)
            {
                _ = stringBuilder.AppendFormat("{0}={1}&", kvp.Key, kvp.Value);
            }
            _ = stringBuilder.Append("api_sig=" + SignCall(args));
            return stringBuilder.ToString();
        }
        public static string MD5(string toHash)
        {
            byte[] textBytes = Encoding.UTF8.GetBytes(toHash);
            System.Security.Cryptography.MD5CryptoServiceProvider cryptHandler = new();
            byte[] hash = cryptHandler.ComputeHash(textBytes);
            return hash.Aggregate("", (current, a) => current + a.ToString("x2"));
        }
        private async Task AddTextToFile(string textToSave)
        {
            Windows.Storage.StorageFolder appFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile file = await appFolder.CreateFileAsync("userid.txt",
                Windows.Storage.CreationCollisionOption.OpenIfExists);
            await Windows.Storage.FileIO.AppendTextAsync(file, textToSave);
        }
        private async Task AddTextToFile2(string textToSave)
        {
            Windows.Storage.StorageFolder appFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile file = await appFolder.CreateFileAsync("signature.txt",
                Windows.Storage.CreationCollisionOption.OpenIfExists);
            await Windows.Storage.FileIO.AppendTextAsync(file, textToSave);
        }

        private async Task AddTextToFile3(string textToSave)
        {
            Windows.Storage.StorageFolder appFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile file = await appFolder.CreateFileAsync("name.txt",
                Windows.Storage.CreationCollisionOption.OpenIfExists);
            await Windows.Storage.FileIO.AppendTextAsync(file, textToSave);
        }
        private async Task<bool> GetFileStatus()
        {
            Windows.Storage.StorageFolder appFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.IStorageItem file = await appFolder.TryGetItemAsync("userid.txt");
            return file != null;
        }
        private Task<string> GetSessionKey(string stringxml)
        {
            xmlDoc.LoadXml(stringxml);
            XmlNode node = xmlDoc.DocumentElement.SelectSingleNode("/lfm/session/key");
            return Task.FromResult(node.InnerText);
        }
        private Task<string> GetUserName(string stringxml)
        {
            xmlDoc.LoadXml(stringxml);
            XmlNode node = xmlDoc.DocumentElement.SelectSingleNode("/lfm/session/name");
            return Task.FromResult(node.InnerText);
        }

        private async void LastFmFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            bool fileexists = await GetFileStatus();
            if (fileexists) { }
            else
            {
                string Token = await Task.Run(GetToken);
                string uriToLaunch = "https://www.last.fm/api/auth?api_key=" + LastFM.key + "&token=" + Token + "&redirect_uri=" + Uri.EscapeDataString("https://www.google.com");
                starturi = new Uri(uriToLaunch);
                App app = Application.Current as App;
                /*
                app.urilink = uri;
                LoginDialog loginDialog = new();
                _ = await loginDialog.ShowAsync(ExistingDialogOptions.CloseExisting);
                */

                Dictionary<string, string> args = new()
                {
                    { "method", "auth.getSession" },
                    { "api_key", LastFM.key },
                    { "token", Token }
                };
                string url = GetSignedURI(args, true);
                Uri endUri = new(url);
                WebAuthenticationResult webAuthenticationResult =
                    await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.UseTitle, starturi, endUri);
                string signature = SignCall(args);
                WebClient wc = new();
                string xml = wc.DownloadString(url);
                string key = await Task.Run(() => GetSessionKey(xml));
                string name = await Task.Run(() => GetUserName(xml));
                await AddTextToFile(key);
                await AddTextToFile2(signature);
                await AddTextToFile3(name);
                app.sessionkey = key;
                app.signature = signature;
                MainPage.Current.AccountMenuText = name;
            }
        }
    }
}
