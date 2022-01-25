using Rise.App.Common;
using Rise.App.Dialogs;
using Rise.App.Helpers;
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

        private async void LastFmFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            bool fileExists = await LastFMHelper.GetFileStatus();
            if (!fileExists)
            {
                string Token = await Task.Run(LastFMHelper.GetToken);
                string uriToLaunch = "https://www.last.fm/api/auth?api_key=" + LastFM.key + "&token=" + Token + "&redirect_uri=" + Uri.EscapeDataString("https://www.google.com");
                LastFMHelper.startUri = new Uri(uriToLaunch);
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
                string url = LastFMHelper.GetSignedURI(args, true);
                Uri endUri = new(url);
                WebAuthenticationResult webAuthenticationResult =
                    await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.UseTitle, LastFMHelper.startUri, endUri);
                string signature = LastFMHelper.SignCall(args);
                WebClient wc = new();
                string xml = wc.DownloadString(url);
                string key = await Task.Run(() => LastFMHelper.GetSessionKey(xml));
                string name = await Task.Run(() => LastFMHelper.GetUserName(xml));
                await AccountsHelper.AddTextToFile(key, "userid.txt");
                await AccountsHelper.AddTextToFile(signature, "signature.txt");
                await AccountsHelper.AddTextToFile(name, "name.txt");
                app.sessionkey = key;
                app.signature = signature;
                MainPage.Current.AccountMenuText = name;
            }
        }
    }
}
