using Rise.Common.Enums;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.UserControls.FileBrowser
{
    public sealed partial class FileBrowserListingSectionControl : UserControl
    {
        public FileBrowserListingSectionControl()
        {
            InitializeComponent();
        }

        public string SectionName
        {
            get => (string)GetValue(SectionNameProperty);
            set => SetValue(SectionNameProperty, value);
        }
        public static readonly DependencyProperty SectionNameProperty =
            DependencyProperty.Register(nameof(SectionName), typeof(string), typeof(FileBrowserListingSectionControl), new PropertyMetadata(null));

        public Visibility SectionHeaderVisibility
        {
            get => (Visibility)GetValue(SectionHeaderVisibilityProperty);
            set => SetValue(SectionHeaderVisibilityProperty, value);
        }
        public static readonly DependencyProperty SectionHeaderVisibilityProperty =
            DependencyProperty.Register(nameof(SectionHeaderVisibility), typeof(Visibility), typeof(FileBrowserListingSectionControl), new PropertyMetadata(Visibility.Visible));

        public DisplayMode DisplayMode
        {
            get => (DisplayMode)GetValue(DisplayModeProperty);
            set => SetValue(DisplayModeProperty, value);
        }
        public static readonly DependencyProperty DisplayModeProperty =
            DependencyProperty.Register(nameof(DisplayMode), typeof(DisplayMode), typeof(FileBrowserListingSectionControl), new PropertyMetadata(DisplayMode.List));

        public IconElement Icon
        {
            get => (IconElement)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(nameof(Icon), typeof(IconElement), typeof(FileBrowserListingSectionControl), new PropertyMetadata(null));

        public object ItemsSource
        {
            get => (object)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(object), typeof(FileBrowserListingSectionControl), new PropertyMetadata(null));


        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(FileBrowserListingSectionControl), new PropertyMetadata(null));
    }
}
