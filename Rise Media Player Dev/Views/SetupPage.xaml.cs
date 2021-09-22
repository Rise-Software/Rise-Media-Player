using RMP.App.Dialogs;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace RMP.App.Views
{
    /// <summary>
    /// Setup page.
    /// </summary>
    public sealed partial class SetupPage : Page
    {
        public static SetupPage Current { get; set; }
        private SetupTitleBar SetupTitleBarHandle { get; set; }
        public SetupPage()
        {
            this.InitializeComponent();
            Current = this;

            SetupTitleBarHandle = new SetupTitleBar();
            SetupTitleBarHandle.InitTitleBar();
        }

        private async void SetupButton_Click(object sender, RoutedEventArgs e)
        {
            SetupDialog setupDialog = new SetupDialog();
            _ = await setupDialog.ShowAsync();
        }
    }
}
