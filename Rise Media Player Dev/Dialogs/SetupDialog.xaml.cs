using Rise.App.Common;
using Rise.App.Setup;
using Rise.App.ViewModels;
using Rise.App.Views;
using System;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Rise.App.Dialogs
{
    public sealed partial class SetupDialog : ContentDialog
    {
        private SettingsViewModel ViewModel => App.SViewModel;

        #region Setup Icons
        private readonly BitmapSource TermsImage =
            new BitmapImage(new Uri("ms-appx:///Assets/Setup/Terms.png"));

        private readonly BitmapSource ConnectImage =
            new BitmapImage(new Uri("ms-appx:///Assets/Setup/Connect.png"));

        private readonly BitmapSource LocalImage =
            new BitmapImage(new Uri("ms-appx:///Assets/Setup/Local.png"));

        private readonly BitmapSource PrivacyImage =
            new BitmapImage(new Uri("ms-appx:///Assets/Setup/Privacy.png"));

        private readonly BitmapSource AppearanceImage =
            new BitmapImage(new Uri("ms-appx:///Assets/Setup/Appearance.png"));

        private readonly BitmapSource DoneImage =
            new BitmapImage(new Uri("ms-appx:///Assets/Setup/Done.png"));
        #endregion

        public SetupDialog()
        {
            InitializeComponent();
            ContentDialog_SizeChanged(null, null);
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
            PrimaryAction();
            ViewModel.SetupProgress++;
            Navigate();
        }

        private async void SecondaryButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            await SecondaryActionAsync();
            ViewModel.SetupProgress++;
            Navigate();
        }

        /// <summary>
        /// Action that takes place when primary dialog button is pressed.
        /// </summary>
        private void PrimaryAction()
        {
            switch (ViewModel.SetupProgress)
            {
                case 0:
                    break;

                case 1:
                    break;

                case 2:
                    break;

                case 3:
                    break;

                case 4:
                    break;

                case 5:
                    break;

                default:
                    break;
            }
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
                    Hide();
                    break;

                case 1:
                    ViewModel.SetupProgress++;
                    break;

                case 2:
                    break;

                case 3:
                    break;

                case 4:
                    break;

                case 5:
                    ViewModel.SetupCompleted = true;
                    ViewModel.SetupProgress = 0;
                    _ = await ApplicationView.GetForCurrentView().TryConsolidateAsync();
                    break;

                default:
                    break;
            }

            return;
        }

        /// <summary>
        /// Navigate between pages of the setup.
        /// </summary>
        private void Navigate()
        {
            BackButton.Visibility = ViewModel.SetupProgress > 1 ?
                Visibility.Visible : Visibility.Collapsed;

            ContentDialog_SizeChanged(null, null);

            switch (ViewModel.SetupProgress)
            {
                case 0:
                    Header.Text = ResourceLoaders.SetupLoader.GetString("LicenseH");
                    PrimaryButton.Content = ResourceLoaders.SetupLoader.GetString("Accept");
                    SecondaryButton.Content = ResourceLoaders.SetupLoader.GetString("Decline");
                    SetupInfo.Text = ResourceLoaders.SetupLoader.GetString("PreSetup");

                    SetupProgress.Value = 0;
                    SetupIcon.Source = TermsImage;
                    _ = SetupFrame.Navigate(typeof(TermsPage));
                    break;

                case 1:
                    Header.Text = ResourceLoaders.SetupLoader.GetString("ConnectH");
                    PrimaryButton.Content = ResourceLoaders.SetupLoader.GetString("Continue");
                    SecondaryButton.Content = ResourceLoaders.SetupLoader.GetString("OnlyLocal");
                    SetupInfo.Text = ResourceLoaders.SetupLoader.GetString("Step1");

                    SetupProgress.Value = 20;
                    SetupIcon.Source = ConnectImage;
                    _ = SetupFrame.Navigate(typeof(ConnectPage));
                    break;

                case 2:
                    Header.Text = ResourceLoaders.SetupLoader.GetString("LocalH");
                    PrimaryButton.Content = ResourceLoaders.SetupLoader.GetString("Continue");
                    SecondaryButton.Content = ResourceLoaders.SetupLoader.GetString("OnlyStreaming");
                    SetupInfo.Text = ResourceLoaders.SetupLoader.GetString("Step2");

                    SetupProgress.Value = 40;
                    SetupIcon.Source = LocalImage;
                    _ = SetupFrame.Navigate(typeof(LocalPage));
                    break;

                case 3:
                    Header.Text = ResourceLoaders.SetupLoader.GetString("PrivacyH");
                    PrimaryButton.Content = ResourceLoaders.SetupLoader.GetString("Continue");
                    SecondaryButton.Content = ResourceLoaders.SetupLoader.GetString("Decide");
                    SetupInfo.Text = ResourceLoaders.SetupLoader.GetString("Step3");

                    SetupProgress.Value = 60;
                    SetupIcon.Source = PrivacyImage;
                    _ = SetupFrame.Navigate(typeof(PrivacyPage));
                    break;

                case 4:
                    Header.Text = ResourceLoaders.SetupLoader.GetString("AppearanceH");
                    PrimaryButton.Content = ResourceLoaders.SetupLoader.GetString("Continue");
                    SecondaryButton.Content = ResourceLoaders.SetupLoader.GetString("Decide");
                    SetupInfo.Text = ResourceLoaders.SetupLoader.GetString("Step4");

                    SetupProgress.Value = 80;
                    SetupIcon.Source = AppearanceImage;
                    _ = SetupFrame.Navigate(typeof(AppearancePage));
                    break;

                case 5:
                    Header.Text = ResourceLoaders.SetupLoader.GetString("DoneH");
                    PrimaryButton.Content = ResourceLoaders.SetupLoader.GetString("Continue");
                    SecondaryButton.Content = ResourceLoaders.SetupLoader.GetString("NotNow");
                    SetupInfo.Text = ResourceLoaders.SetupLoader.GetString("Step5");

                    SetupProgress.Value = 100;
                    SetupIcon.Source = DoneImage;
                    _ = SetupFrame.Navigate(typeof(FinishPage));
                    break;

                case 6:
                    ViewModel.SetupCompleted = true;
                    ViewModel.SetupProgress = 0;
                    Hide();

                    Frame rootFrame = Window.Current.Content as Frame;
                    _ = rootFrame.Navigate(typeof(MainPage));
                    break;

                default:
                    Header.Text = ResourceLoaders.SetupLoader.GetString("LicenseH");
                    PrimaryButton.Content = ResourceLoaders.SetupLoader.GetString("Accept");
                    SecondaryButton.Content = ResourceLoaders.SetupLoader.GetString("Decline");
                    SetupInfo.Text = ResourceLoaders.SetupLoader.GetString("PreSetup");

                    SetupProgress.Value = 0;
                    SetupIcon.Source = TermsImage;
                    _ = SetupFrame.Navigate(typeof(TermsPage));
                    break;
            }
        }

        private void ContentDialog_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double windowWidth = Window.Current.Bounds.Width;
            double windowHeight = Window.Current.Bounds.Height;

            if (windowWidth < 770)
            {
                SetupFrame.Width = windowWidth - 68;
                IconColumn.Width = new GridLength(0);
                ProgressColumn.Width = new GridLength(0);
                InfoGrid.ColumnSpacing = 0;
                ControlGrid.Margin = new Thickness(-32, -24, -24, -24);

                Header.Margin = BackButton.Visibility == Visibility.Visible ?
                    new Thickness(42, -5, 0, 0) : new Thickness(0, -5, 0, 0);
            }
            else
            {
                SetupFrame.Width = 770 - 284;
                IconColumn.Width = new GridLength(188);
                ProgressColumn.Width = new GridLength(210);
                InfoGrid.ColumnSpacing = 28;
                ControlGrid.Margin = new Thickness(-24);
                Header.Margin = new Thickness(0, -4, 0, 0);
            }

            // The 59 is because for some reason the dialog has a 1px transparent
            // line at the bottom. Don't shoot me, I'm just the messenger.
            RootGrid.Height = windowHeight < 498 ? windowHeight - 59 : 498 - 59;
        }
    }
}
