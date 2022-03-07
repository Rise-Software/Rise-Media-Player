using Rise.App.Common;
using Rise.Common.Constants;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Views
{
    public sealed partial class CrashDetailsPage : Page
    {
        private string Text = "";

        public CrashDetailsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Text = e.Parameter as string;
            base.OnNavigatedTo(e);
        }

        private void SubmitIssueButton_Click(object sender, RoutedEventArgs e) => _ = URLs.Feedback.LaunchAsync();
    }
}
