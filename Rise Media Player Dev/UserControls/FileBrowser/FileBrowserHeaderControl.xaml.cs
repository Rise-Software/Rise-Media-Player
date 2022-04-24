using Microsoft.Toolkit.Mvvm.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Rise.App.UserControls.FileBrowser
{
    public sealed partial class FileBrowserHeaderControl : UserControl
    {
        public FileBrowserHeaderControl()
        {
            this.InitializeComponent();
        }

        public string CurrentPath
        {
            get => (string)GetValue(CurrentPathProperty);
            set => SetValue(CurrentPathProperty, value);
        }
        public static readonly DependencyProperty CurrentPathProperty =
            DependencyProperty.Register(nameof(CurrentPath), typeof(string), typeof(FileBrowserHeaderControl), new PropertyMetadata(null));


        public IRelayCommand SectionClickedCommand
        {
            get => (IRelayCommand)GetValue(SectionClickedCommandProperty);
            set => SetValue(SectionClickedCommandProperty, value);
        }
        public static readonly DependencyProperty SectionClickedCommandProperty =
            DependencyProperty.Register(nameof(SectionClickedCommand), typeof(IRelayCommand), typeof(FileBrowserHeaderControl), new PropertyMetadata(null));


        public IRelayCommand PinToSidebarCommand
        {
            get => (IRelayCommand)GetValue(PinToSidebarCommandProperty);
            set => SetValue(PinToSidebarCommandProperty, value);
        }
        public static readonly DependencyProperty PinToSidebarCommandProperty =
            DependencyProperty.Register(nameof(PinToSidebarCommand), typeof(IRelayCommand), typeof(FileBrowserHeaderControl), new PropertyMetadata(null));


        public IRelayCommand OpenInFileExplorerCommand
        {
            get => (IRelayCommand)GetValue(OpenInFileExplorerCommandProperty);
            set => SetValue(OpenInFileExplorerCommandProperty, value);
        }
        public static readonly DependencyProperty OpenInFileExplorerCommandProperty =
            DependencyProperty.Register(nameof(OpenInFileExplorerCommand), typeof(IRelayCommand), typeof(FileBrowserHeaderControl), new PropertyMetadata(null));
    }
}
