using Rise.App.ViewModels;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Dialogs
{
    public sealed partial class WidgetsDialogContent : Page
    {
        private MainViewModel MViewModel => App.MViewModel;

        public WidgetsDialogContent()
        {
            InitializeComponent();
        }
    }
}
