using Rise.App.ViewModels.FileBrowser;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Rise.App.Views.FileBrowser
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FileBrowserHomePage : Page
    {
        public FileBrowserHomePageViewModel ViewModel
        {
            get => (FileBrowserHomePageViewModel)DataContext;
            set => DataContext = value;
        }

        public FileBrowserHomePage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is FileBrowserHomePageViewModel viewModel)
            {
                ViewModel = viewModel;
            }
        }
    }
}
