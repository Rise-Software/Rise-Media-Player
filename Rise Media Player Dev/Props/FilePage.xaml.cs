using RMP.App.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace RMP.App.Props
{
    public sealed partial class FilePage : Page
    {
        private SongPropertiesViewModel Props { get; set; }

        public FilePage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is SongPropertiesViewModel props)
            {
                Props = props;
            }

            base.OnNavigatedTo(e);
        }
    }
}
