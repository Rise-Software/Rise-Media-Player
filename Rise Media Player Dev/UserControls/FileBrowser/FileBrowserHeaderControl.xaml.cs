using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls;
using Rise.App.ViewModels.FileBrowser;
using CommunityToolkit.Mvvm.Input;

namespace Rise.App.UserControls.FileBrowser
{
    public sealed partial class FileBrowserHeaderControl : UserControl
    {
        public FileBrowserHeaderControl()
        {
            InitializeComponent();
        }

        public IList<FileBrowserBreadcrumbItemViewModel> Items
        {
            get => (IList<FileBrowserBreadcrumbItemViewModel>)GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register(nameof(Items), typeof(IList<FileBrowserBreadcrumbItemViewModel>), typeof(FileBrowserHeaderControl), new PropertyMetadata(null));

        public string? CurrentLocation
        {
            get => (string?)GetValue(CurrentLocationProperty);
            set => SetValue(CurrentLocationProperty, value);
        }
        public static readonly DependencyProperty CurrentLocationProperty =
            DependencyProperty.Register(nameof(CurrentLocation), typeof(string), typeof(FileBrowserHeaderControl), new PropertyMetadata(string.Empty));

        public bool IsPinToSidebarEnabled
        {
            get => (bool)GetValue(IsPinToSidebarEnabledProperty);
            set => SetValue(IsPinToSidebarEnabledProperty, value);
        }
        public static readonly DependencyProperty IsPinToSidebarEnabledProperty =
            DependencyProperty.Register(nameof(IsPinToSidebarEnabled), typeof(bool), typeof(FileBrowserHeaderControl), new PropertyMetadata(null));

        public bool IsOpenInFileExplorerEnabled
        {
            get => (bool)GetValue(IsOpenInFileExplorerEnabledProperty);
            set => SetValue(IsOpenInFileExplorerEnabledProperty, value);
        }
        public static readonly DependencyProperty IsOpenInFileExplorerEnabledProperty =
            DependencyProperty.Register(nameof(IsOpenInFileExplorerEnabled), typeof(bool), typeof(FileBrowserHeaderControl), new PropertyMetadata(null));

        public bool CanGoBack
        {
            get => (bool)GetValue(CanGoBackProperty);
            set => SetValue(CanGoBackProperty, value);
        }
        public static readonly DependencyProperty CanGoBackProperty =
            DependencyProperty.Register(nameof(CanGoBack), typeof(bool), typeof(FileBrowserHeaderControl), new PropertyMetadata(false));

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

        private void BreadcrumbBar_ItemClicked(BreadcrumbBar sender, BreadcrumbBarItemClickedEventArgs args)
        {
            if (args.Item is FileBrowserBreadcrumbItemViewModel itemViewModel)
            {
                itemViewModel.ItemClickedCommand?.Execute(itemViewModel);
            }
        }
    }
}
