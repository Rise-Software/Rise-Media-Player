using CommunityToolkit.Mvvm.Messaging;
using Rise.App.Messages.FileBrowser;
using Rise.App.ViewModels.FileBrowser.Listing;
using Rise.App.ViewModels.FileBrowser.Pages;
using System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
            await ViewModel.EnumerateDirectoryAsync(CancellationToken.None);
        }

        private async void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as FileBrowserListingItemViewModel;
            await item?.OpenAsync();
        }
    }
}
