using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Rise.App.Messages.FileBrowser;
using Rise.App.ViewModels;
using Rise.App.ViewModels.FileBrowser.Listing;
using Rise.App.ViewModels.FileBrowser.Pages;
using Rise.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Rise.App.Views.FileBrowser
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FileBrowserDirectoryPage : Page, IRecipient<FileBrowserDirectoryEnumerationRequestedMessage>
    {
        public FileBrowserDirectoryPageViewModel ViewModel
        {
            get => (FileBrowserDirectoryPageViewModel)DataContext;
            set => DataContext = value;
        }

        public FileBrowserDirectoryPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is FileBrowserDirectoryPageViewModel viewModel)
            {
                ViewModel = viewModel;

                if (!ViewModel.Messenger.IsRegistered<FileBrowserDirectoryEnumerationRequestedMessage>(this))
                {
                    ViewModel.Messenger.Register<FileBrowserDirectoryEnumerationRequestedMessage>(this);
                }
            }
        }

        public async void Receive(FileBrowserDirectoryEnumerationRequestedMessage message)
        {
            await ViewModel.EnumerateDirectoryAsync(message.Value);
        }

        private async void FileBrowserDirectoryPage_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= FileBrowserDirectoryPage_Loaded;
            await ViewModel.EnumerateDirectoryAsync(CancellationToken.None);
        }

        private async void FileBrowserListingItem_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is not FileBrowserListingItemViewModel item)
                return;

            await item.OpenAsync();
        }

        [RelayCommand(AllowConcurrentExecutions = true)]
        private async Task PlayItemAsync(SongViewModel item)
        {
            App.MPViewModel.AddSingleItemToQueue(await item.AsPlaybackItemAsync());
            App.MPViewModel.Player.Play();
        }
    }
}
