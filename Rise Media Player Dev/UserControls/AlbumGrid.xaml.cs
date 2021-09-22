using RMP.App.ViewModels;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace RMP.App.UserControls
{
    public sealed partial class AlbumGrid : UserControl
    {
        /// <summary>
        /// Gets the app-wide ViewModel instance.
        /// </summary>
        public MainViewModel ViewModel => App.ViewModel;
        public AlbumGrid()
        {
            this.InitializeComponent();
        }
    }
}
