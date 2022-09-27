using Rise.App.Setup;
using Rise.App.ViewModels;
using Rise.App.Views;
using Rise.Common.Extensions.Markup;
using System;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Rise.App.Dialogs
{
    public sealed partial class SetupDialogContent : Page
    {
        private SettingsViewModel ViewModel => App.SViewModel;

        public SetupDialogContent()
        {
            InitializeComponent();
            Navigate(SlideNavigationTransitionEffect.FromRight);
        }
    }

    // Event handlers
    public sealed partial class SetupDialogContent
    {
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SetupProgress--;
            Navigate(SlideNavigationTransitionEffect.FromLeft);
        }

        private void PrimaryButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SetupProgress++;
            Navigate(SlideNavigationTransitionEffect.FromRight);
        }

        private async void SecondaryButton_Click(object sender, RoutedEventArgs e)
        {
            await SecondaryActionAsync();
            ViewModel.SetupProgress++;
            Navigate(SlideNavigationTransitionEffect.FromRight);
        }
    }

    // Navigation
    public sealed partial class SetupDialogContent
    {
        /// <summary>
        /// Action that takes place when secondary dialog button is pressed.
        /// </summary>
        private async Task SecondaryActionAsync()
        {
            switch (ViewModel.SetupProgress)
            {
                case 0:
                    ViewModel.SetupProgress--;
                    break;

                case 1:
                    ViewModel.FetchOnlineData = false;
                    break;

                case 5:
                    ViewModel.SetupCompleted = true;
                    ViewModel.SetupProgress = 0;

                    _ = await ApplicationView.GetForCurrentView().TryConsolidateAsync();
                    break;
            }
        }

        /// <summary>
        /// Navigate between pages of the setup using the
        /// specified effect for the page transition.
        /// </summary>
        private void Navigate(SlideNavigationTransitionEffect effect)
        {
            int progress = ViewModel.SetupProgress;
            if (progress == 0)
            {
                SetupInfo.Text = ResourceHelper.GetString("SetupPre");
                PrimaryButton.Content = ResourceHelper.GetString("Accept");
            }
            else if (progress < 6)
            {
                string format = ResourceHelper.GetString("StepOf");
                SetupInfo.Text = string.Format(format, progress, 5);

                PrimaryButton.Content = ResourceHelper.GetString("Continue");
                BackButton.Visibility = progress > 1 ?
                    Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                ViewModel.SetupCompleted = true;
                ViewModel.SetupProgress = 0;

                var popups = VisualTreeHelper.GetOpenPopups(Window.Current);
                foreach (var popup in popups)
                {
                    if (popup.Child is ContentDialog dialog)
                    {
                        dialog.Hide();
                        break;
                    }
                }

                Frame rootFrame = Window.Current.Content as Frame;
                _ = rootFrame.Navigate(typeof(MainPage));
                return;
            }

            string res = GetSecondaryButtonResource(progress);
            SecondaryButton.Content = ResourceHelper.GetString(res);

            var nextPage = GetCurrentPage(progress);
            var transition = new SlideNavigationTransitionInfo() { Effect = effect };

            _ = SetupFrame.Navigate(nextPage, null, transition);
        }

        private Type GetCurrentPage(int progress) => progress switch
        {
            1 => typeof(ConnectPage),
            2 => typeof(LocalPage),
            3 => typeof(PrivacyPage),
            4 => typeof(AppearancePage),
            5 => typeof(FinishPage),
            _ => typeof(TermsPage)
        };

        private string GetSecondaryButtonResource(int progress) => progress switch
        {
            1 => "OnlyLocal",
            2 => "OnlyStreaming",
            3 => "DecideForMe",
            4 => "DecideForMe",
            5 => "NotNow",
            _ => "Decline"
        };
    }
}
