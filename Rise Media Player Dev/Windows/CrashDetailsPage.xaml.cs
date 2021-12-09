using Rise.App.Common;
using Rise.App.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Views
{
    public sealed partial class CrashDetailsPage : Page
    {
        public static CrashDetailsPage Current;
        private string Text = "";

        public CrashDetailsPage()
        {
            InitializeComponent();
            Current = this;
            Loaded += CrashDetailsPage_Loaded;
        }

        private void CrashDetailsPage_Loaded(object sender, RoutedEventArgs e)
        {
            _ = new ApplicationTitleBar(AppTitleBar);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Text = e.Parameter as string;
            base.OnNavigatedTo(e);
        }

        private void SubmitIssueButton_Click(object sender, RoutedEventArgs e) => _ = URLs.Feedback.LaunchAsync();
    }
}
