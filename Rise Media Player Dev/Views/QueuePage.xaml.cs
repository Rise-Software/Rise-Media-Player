using Rise.App.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Rise.App.Views
{
    public sealed partial class QueuePage : Page
    {
        /// <summary>
        /// Gets the app-wide NPViewModel instance.
        /// </summary>
        private PlaybackViewModel ViewModel => App.PViewModel;

        public QueuePage()
        {
            InitializeComponent();
        }

        private void MainList_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            int itemIndex = MainList.SelectedIndex;

            if (itemIndex < 0)
            {
                return;
            }

            ViewModel.PlaybackList.MoveTo((uint)itemIndex);
        }
    }
}
