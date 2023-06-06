using Rise.App.ViewModels;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Dialogs
{
    /// <summary>
    /// A simple setup wizard for Musixmatch API tokens. For use when setting
    /// up online lyrics.
    /// </summary>
    public sealed partial class MusixmatchTokenDialog : ContentDialog
    {
        private SettingsViewModel ViewModel => App.SViewModel;

        public MusixmatchTokenDialog()
        {
            InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            ViewModel.MusixmatchLyricsToken = TokenBox.Text;
        }
    }
}
