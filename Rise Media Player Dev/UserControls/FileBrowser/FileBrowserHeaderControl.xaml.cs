using Microsoft.Toolkit.Mvvm.Input;
using Rise.App.ViewModels.FileBrowser;
using Rise.Storage;
using System.Collections.Generic;
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

        public IList<BreadcrumbItemViewModel> Items
        {
            get => (IList<BreadcrumbItemViewModel>)GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register(nameof(Items), typeof(IList<BreadcrumbItemViewModel>), typeof(FileBrowserHeaderControl), new PropertyMetadata(null));


        public IFolder CurrentFolder
        {
            get => (IFolder)GetValue(CurrentFolderProperty);
            set => SetValue(CurrentFolderProperty, value);
        }
        public static readonly DependencyProperty CurrentFolderProperty =
            DependencyProperty.Register(nameof(CurrentFolder), typeof(IFolder), typeof(FileBrowserHeaderControl), new PropertyMetadata(null));


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
