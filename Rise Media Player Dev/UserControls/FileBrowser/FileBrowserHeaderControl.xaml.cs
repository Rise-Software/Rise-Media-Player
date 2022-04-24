using Microsoft.Toolkit.Mvvm.Input;
using Rise.App.ViewModels;
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


        public string CurrentLocation
        {
            get => (string)GetValue(CurrentLocationProperty);
            set => SetValue(CurrentLocationProperty, value);
        }
        public static readonly DependencyProperty CurrentLocationProperty =
            DependencyProperty.Register(nameof(CurrentLocation), typeof(string), typeof(FileBrowserHeaderControl), new PropertyMetadata(null));


        public IRelayCommand GoBackCommand
        {
            get => (IRelayCommand)GetValue(GoBackCommandProperty);
            set => SetValue(GoBackCommandProperty, value);
        }
        public static readonly DependencyProperty GoBackCommandProperty =
            DependencyProperty.Register(nameof(GoBackCommand), typeof(IRelayCommand), typeof(FileBrowserHeaderControl), new PropertyMetadata(null));


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
