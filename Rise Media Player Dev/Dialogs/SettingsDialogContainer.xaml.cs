using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RMP.App.Dialogs
{
    public sealed partial class SettingsDialogContainer : ContentDialog
    {
        public SettingsDialogContainer Current;
        public ObservableCollection<string> Breadcrumbs =
            new ObservableCollection<string>();

        public SettingsDialogContainer()
        {
            InitializeComponent();
            Current = this;

            SizeChanged += SettingsDialogContainer_SizeChanged;
        }

        private void SettingsDialogContainer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double width = Window.Current.Bounds.Width;
            double height = Window.Current.Bounds.Height;

            Page content = Content as Page;
            content.Width = width < 800 ? width : 800;
            content.Height = height < 620 ? height : 620;
        }
    }
}
