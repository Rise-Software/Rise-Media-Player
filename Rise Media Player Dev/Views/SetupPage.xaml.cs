using RMP.App.Dialogs;
using RMP.App.Windows;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RMP.App.Views
{
    /// <summary>
    /// Setup page.
    /// </summary>
    public sealed partial class SetupPage : Page
    {
        public static SetupPage Current { get; set; }

        private readonly SetupDialog Dialog = new SetupDialog();

        public SetupPage()
        {
            InitializeComponent();
            Current = this;

            _ = new SetupTitleBar();
        }

        private async void SetupButton_Click(object sender, RoutedEventArgs e)
        {
            _ = await Dialog.ShowAsync();
        }
    }
}
