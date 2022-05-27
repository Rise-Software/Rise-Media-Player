using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Messaging;
using Rise.App.Messages.FileBrowser;
using Rise.App.Services;
using Rise.App.ViewModels.FileBrowser;
using Rise.App.Views.FileBrowser;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Rise.App.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FileBrowserPage : Page, IRecipient<FileBrowserNavigationRequestedMessage>
    {
        private IStorageService StorageService { get; } = Ioc.Default.GetRequiredService<IStorageService>();

        public FileBrowserPageViewModel ViewModel
        {
            get => App.FileBrowserPageViewModel.Value;
        }

        public FileBrowserPage()
        {
            this.InitializeComponent();
        }

        private async void FileBrowserPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!ViewModel.Messenger.IsRegistered<FileBrowserNavigationRequestedMessage>(this))
            {
                ViewModel.Messenger.Register<FileBrowserNavigationRequestedMessage>(this);
            }

            if (await StorageService.EnsureFileSystemIsAccessible())
            {
                await ViewModel.EnsureInitialized();
            }
        }

        public void Receive(FileBrowserNavigationRequestedMessage message)
        {
            switch (message.Value)
            {
                case FileBrowserHomePageViewModel:
                    ContentFrame.Navigate(typeof(FileBrowserHomePage), message.Value, new DrillInNavigationTransitionInfo());
                    break;

                case FileBrowserDirectoryPageViewModel:
                    ContentFrame.Navigate(typeof(FileBrowserDirectoryPage), message.Value, new DrillInNavigationTransitionInfo());
                    break;
            }
        }
    }
}
