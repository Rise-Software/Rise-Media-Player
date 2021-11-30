using Rise.App.ViewModels;
using Rise.App.Views;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Documents;

namespace Rise.App.ResourceDictionaries
{
    public sealed partial class SongTemplates : ResourceDictionary
    {
        private SongViewModel _song;

        public SongTemplates()
        {
            InitializeComponent();
        }

        private void Grid_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (_song == null)
            {
                if ((e.OriginalSource as FrameworkElement).DataContext is SongViewModel song)
                {
                    _song = song;
                }
            }

            _song.IsFocused = true;
        }

        private void Grid_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (_song != null)
            {
                _song.IsFocused = false;
                _song = null;
            }
        }

        private void Album_Click(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            var run = sender.Inlines.FirstOrDefault() as Run;

            _ = MainPage.Current.ContentFrame.
                Navigate(typeof(AlbumSongsPage), run.Text);
        }

        private void Artist_Click(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            var run = sender.Inlines.FirstOrDefault() as Run;

            _ = MainPage.Current.ContentFrame.
                Navigate(typeof(ArtistSongsPage), run.Text);
        }
    }
}
