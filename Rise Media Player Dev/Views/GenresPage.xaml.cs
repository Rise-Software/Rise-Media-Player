using RMP.App.ViewModels;
using Windows.UI.Xaml.Controls;

namespace RMP.App.Views
{
    public sealed partial class GenresPage : Page
    {
        /// <summary>
        /// Gets the app-wide MViewModel instance.
        /// </summary>
        private MainViewModel ViewModel => App.MViewModel;

        public GenresPage()
        {
            InitializeComponent();
        }
    }
}
