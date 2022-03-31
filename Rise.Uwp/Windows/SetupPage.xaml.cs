using Rise.App.Dialogs;
using Rise.Common.Enums;
using Rise.Common.Extensions;
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
        }

        private async void SetupButton_Click(object sender, RoutedEventArgs e)
            => _ = await Dialog.ShowAsync(ExistingDialogOptions.CloseExisting);
    }
}
