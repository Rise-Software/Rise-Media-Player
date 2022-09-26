using Rise.App.Setup;
using Rise.App.ViewModels;
using Rise.App.Views;
using Rise.Common;
using System;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Rise.App.Dialogs
{
    public sealed partial class SetupDialogContent : Page
    {
        private SettingsViewModel ViewModel => App.SViewModel;

        public SetupDialogContent()
        {
            InitializeComponent();
            Navigate();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SetupProgress > 0)
            {
                ViewModel.SetupProgress--;
            }

            Navigate();
        }

        private void PrimaryButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SetupProgress++;
            Navigate();
        }

        private async void SecondaryButton_Click(object sender, RoutedEventArgs e)
        {
            await SecondaryActionAsync();
            ViewModel.SetupProgress++;
            Navigate();
        }

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
        /// Navigate between pages of the setup.
        /// </summary>
        private void Navigate()
        {
            BackButton.Visibility = ViewModel.SetupProgress > 1 ?
                Visibility.Visible : Visibility.Collapsed;

            switch (ViewModel.SetupProgress)
            {
                case 1:
                    PrimaryButton.Content = ResourceLoaders.SetupLoader.GetString("Continue");
                    SecondaryButton.Content = ResourceLoaders.SetupLoader.GetString("OnlyLocal");
                    SetupInfo.Text = ResourceLoaders.SetupLoader.GetString("Step1");

                    _ = SetupFrame.Navigate(typeof(ConnectPage));
                    break;

                case 2:
                    PrimaryButton.Content = ResourceLoaders.SetupLoader.GetString("Continue");
                    SecondaryButton.Content = ResourceLoaders.SetupLoader.GetString("OnlyStreaming");
                    SetupInfo.Text = ResourceLoaders.SetupLoader.GetString("Step2");

                    _ = SetupFrame.Navigate(typeof(LocalPage));
                    break;

                case 3:
                    PrimaryButton.Content = ResourceLoaders.SetupLoader.GetString("Continue");
                    SecondaryButton.Content = ResourceLoaders.SetupLoader.GetString("Decide");
                    SetupInfo.Text = ResourceLoaders.SetupLoader.GetString("Step3");

                    _ = SetupFrame.Navigate(typeof(PrivacyPage));
                    break;

                case 4:
                    PrimaryButton.Content = ResourceLoaders.SetupLoader.GetString("Continue");
                    SecondaryButton.Content = ResourceLoaders.SetupLoader.GetString("Decide");
                    SetupInfo.Text = ResourceLoaders.SetupLoader.GetString("Step4");

                    _ = SetupFrame.Navigate(typeof(AppearancePage));
                    break;

                case 5:
                    PrimaryButton.Content = ResourceLoaders.SetupLoader.GetString("Continue");
                    SecondaryButton.Content = ResourceLoaders.SetupLoader.GetString("NotNow");
                    SetupInfo.Text = ResourceLoaders.SetupLoader.GetString("Step5");

                    _ = SetupFrame.Navigate(typeof(FinishPage));
                    break;

                case 6:
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
                    break;

                default:
                    PrimaryButton.Content = ResourceLoaders.SetupLoader.GetString("Accept");
                    SecondaryButton.Content = ResourceLoaders.SetupLoader.GetString("Decline");
                    SetupInfo.Text = ResourceLoaders.SetupLoader.GetString("PreSetup");

                    _ = SetupFrame.Navigate(typeof(TermsPage));
                    break;
            }
        }
    }
}
