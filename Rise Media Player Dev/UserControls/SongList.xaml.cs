using RMP.App.Converters;
using RMP.App.ViewModels;
using System.ServiceModel.Channels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace RMP.App.UserControls
{
    public sealed partial class SongList : UserControl
    {
        /// <summary>
        /// Gets the app-wide ViewModel instance.
        /// </summary>
        public MainViewModel ViewModel => App.ViewModel;
        private readonly BooleanToVisibility BoolToVis = new BooleanToVisibility();
        public SongList()
        {
            InitializeComponent();
        }

        private void Grid_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            Grid itemRoot = sender as Grid;
            if (List.ContainerFromItem(itemRoot.DataContext) is ListViewItem lvi)
            {
                Windows.UI.Xaml.Data.Binding binding = new Windows.UI.Xaml.Data.Binding
                {
                    Source = itemRoot.DataContext,
                    Path = new PropertyPath("WillRemove"),
                    Converter = BoolToVis,
                    ConverterParameter = "Reverse",
                };
                lvi.SetBinding(VisibilityProperty, binding);
            }
        }
    }
}
