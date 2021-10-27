using RMP.App.ViewModels;
using RMP.App.Views;
using RMP.App.Windows;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace RMP.App.UserControls
{
    public sealed partial class AlbumGrid : UserControl
    {
        /// <summary>
        /// Gets the app-wide MViewModel instance.
        /// </summary>
        private MainViewModel ViewModel => App.MViewModel;

        public AlbumGrid()
        {
            InitializeComponent();
        }

        public static DependencyProperty ListProperty =
            DependencyProperty.Register(nameof(List), typeof(ObservableCollection<AlbumViewModel>),
                typeof(AlbumGrid), null);

        public ObservableCollection<AlbumViewModel> List
        {
            get => (ObservableCollection<AlbumViewModel>)GetValue(ListProperty);
            set => SetValue(ListProperty, value);
        }

        private void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is AlbumViewModel album)
            {
                _ = MainPage.Current.ContentFrame.Navigate(typeof(AlbumSongsPage), album);
            }
        }
    }
}
