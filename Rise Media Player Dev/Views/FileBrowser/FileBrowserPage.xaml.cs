using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Rise.App.Services;
using Rise.App.ViewModels.FileBrowser;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Rise.App.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FileBrowserPage : Page
    {
        private IStorageService StorageService { get; } = Ioc.Default.GetRequiredService<IStorageService>();

        public FileBrowserPageViewModel ViewModel
        {
            get => App.FileBrowserPageViewModel;
            set
            {
                App.FileBrowserPageViewModel = value;
                DataContext = value;
            }
        }

        public FileBrowserPage()
        {
            this.InitializeComponent();

            ViewModel ??= new();
        }

        private async void FileBrowserPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (await StorageService.EnsureFileSystemIsAccessible())
            {
                await ViewModel.TryInitialize();
            }
        }
    }
}
