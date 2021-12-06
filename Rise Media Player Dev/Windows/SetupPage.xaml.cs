using Rise.App.Dialogs;
using Rise.App.Helpers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using static Rise.App.Common.Enums;

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
            => _ = await Dialog.ShowAsync(ExistingDialogOptions.CloseExisting);
    }
}
