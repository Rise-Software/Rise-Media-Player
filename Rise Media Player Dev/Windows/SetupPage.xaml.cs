using Rise.App.Dialogs;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Views
{
    /// <summary>
    /// Setup page.
    /// </summary>
    public sealed partial class SetupPage : Page
    {
        private readonly SetupDialog Dialog = new SetupDialog();

        public SetupPage()
        {
            InitializeComponent();
            _ = new ApplicationTitleBar(AppTitleBar);
        }

        private async void SetupButton_Click(object sender, RoutedEventArgs e)
            => _ = await Dialog.ShowAsync();
    }
}
