using Windows.UI.Xaml.Controls;
using Rise.App.ViewModels.FileBrowser;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Rise.App.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FileBrowserPage : Page
    {
        public FileBrowserPageViewModel ViewModel
        {
            get => (FileBrowserPageViewModel)DataContext;
            set => DataContext = value;
        }

        public FileBrowserPage()
        {
            this.InitializeComponent();

            ViewModel = new();
        }
    }
}
