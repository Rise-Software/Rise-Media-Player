using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Mvvm.Messaging;
using Rise.App.Messages.FileBrowser;
using Rise.App.ViewModels.FileBrowser.Pages;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Rise.App.Views.FileBrowser
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FileBrowserDirectoryPage : Page, IRecipient<FileBrowserDirectoryNavigationRequestedMessage>
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
            }
        }

        private async void FileBrowserDirectoryPage_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.EnumerateDirectoryAsync();
        }

        public async void Receive(FileBrowserDirectoryNavigationRequestedMessage message)
        {
            await ViewModel.EnumerateDirectoryAsync();
        }
    }
}
