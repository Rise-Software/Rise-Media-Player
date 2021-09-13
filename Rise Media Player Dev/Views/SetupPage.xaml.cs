using RMP.App.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace RMP.App.Views
{
    /// <summary>
    /// Setup page.
    /// </summary>
    public sealed partial class SetupPage : Page
    {
        public SetupPage()
        {
            this.InitializeComponent();
        }

        private async void SetupButton_Click(object sender, RoutedEventArgs e)
        {
            SetupDialog setupDialog = new SetupDialog();
            ContentDialogResult setupResult = await setupDialog.ShowAsync();
        }
    }
}
