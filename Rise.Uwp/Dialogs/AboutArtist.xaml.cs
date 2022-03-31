using Rise.App.ViewModels;
using System.Xml;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Rise.App.Dialogs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AboutArtist : Page
    {
        XmlDocument xmlDoc = new();
        public MainViewModel MViewModel => App.MViewModel;

        private static readonly DependencyProperty SelectedArtistProperty =
            DependencyProperty.Register("SelectedArtist", typeof(ArtistViewModel), typeof(Views.ArtistSongsPage), null);

        private ArtistViewModel SelectedArtist
        {
            get => (ArtistViewModel)GetValue(SelectedArtistProperty);
            set => SetValue(SelectedArtistProperty, value);
        }

        public AboutArtist(string aboutArtistBig)
        {
            this.InitializeComponent();
            AboutArtistText.Text = aboutArtistBig;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {


        }
    }

}
