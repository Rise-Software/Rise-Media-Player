using RMP.App.Settings;
using RMP.App.Setup;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace RMP.App.Dialogs
{
    public sealed partial class SetupDialog : ContentDialog
    {
        #region Setup Icons
        private readonly BitmapSource TermsImage =
            new BitmapImage(new Uri("ms-appx:///Assets/Setup/Terms.png"));

        private readonly BitmapSource ConnectImage =
            new BitmapImage(new Uri("ms-appx:///Assets/Setup/Connect.png"));
        #endregion

        public SetupDialog()
        {
            this.InitializeComponent();
            ContentDialog_SizeChanged(null, null);
            Navigate();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (SetupSettings.SetupProgress > 0)
            {
                SetupSettings.SetupProgress--;
            }
            Navigate();
        }

        private void PrimaryButton_Click(object sender, RoutedEventArgs e)
        {
            PrimaryAction();
            SetupSettings.SetupProgress++;
            Navigate();
        }

        private void SecondaryButton_Click(object sender, RoutedEventArgs e)
        {
            SecondaryAction();
            SetupSettings.SetupProgress++;
            Navigate();
        }

        /// <summary>
        /// Action that takes place when primary dialog button is pressed.
        /// </summary>
        private void PrimaryAction()
        {
            switch (SetupSettings.SetupProgress)
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
        private void SecondaryAction()
        {
            switch (SetupSettings.SetupProgress)
            {
                case 0:
                    SetupSettings.SetupProgress--;
                    Hide();
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
        /// Navigate between pages of the setup.
        /// </summary>
        private void Navigate()
        {
            BackButton.Visibility = SetupSettings.SetupProgress > 1 ?
                Visibility.Visible : Visibility.Collapsed;

            switch (SetupSettings.SetupProgress)
            {
                case 0:
                    Header.Text = ResourceLoaders.SetupLoader.GetString("LicenseH");
                    PrimaryButton.Content = ResourceLoaders.SetupLoader.GetString("Accept");
                    SecondaryButton.Content = ResourceLoaders.SetupLoader.GetString("Decline");
                    SetupInfo.Text = ResourceLoaders.SetupLoader.GetString("PreSetup");

                    SetupProgress.Value = 0;
                    SetupIcon.Source = TermsImage;
                    SetupFrame.Navigate(typeof(TermsPage));
                    break;

                case 1:
                    Header.Text = ResourceLoaders.SetupLoader.GetString("ConnectH");
                    PrimaryButton.Content = ResourceLoaders.SetupLoader.GetString("Accept");
                    SecondaryButton.Content = ResourceLoaders.SetupLoader.GetString("Decline");
                    SetupInfo.Text = ResourceLoaders.SetupLoader.GetString("Step1");

                    SetupProgress.Value = 20;
                    SetupIcon.Source = ConnectImage;
                    SetupFrame.Navigate(typeof(ConnectPage));
                    break;

                case 2:
                    SetupFrame.Navigate(typeof(LocalPage));
                    break;

                case 3:
                    SetupFrame.Navigate(typeof(PrivacyPage));
                    break;

                case 4:
                    SetupFrame.Navigate(typeof(Setup.AppearancePage));
                    break;

                case 5:
                    SetupFrame.Navigate(typeof(FinishPage));
                    break;

                default:
                    SetupFrame.Navigate(typeof(TermsPage));
                    break;
            }
        }

        private void ContentDialog_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double windowWidth = Window.Current.Bounds.Width;
            double windowHeight = Window.Current.Bounds.Height;

            if (windowWidth < 770)
            {
                SetupFrame.Width = windowWidth - 284;
            }
            else
            {
                SetupFrame.Width = 770 - 284;
            }

            // The 59 is because for some reason the dialog has a 1px transparent
            // line at the bottom. Don't shoot me, I'm just the messenger.
            if (windowHeight < 498)
            {
                RootGrid.Height = windowHeight - 59;
            }
            else
            {
                RootGrid.Height = 498 - 59;
            }
        }
    }
}
