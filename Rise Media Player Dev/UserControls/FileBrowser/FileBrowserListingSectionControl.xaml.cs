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

        public DataTemplateSelector ItemTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty);
            set => SetValue(ItemTemplateSelectorProperty, value);
        }
        public static readonly DependencyProperty ItemTemplateSelectorProperty =
            DependencyProperty.Register(nameof(ItemTemplateSelector), typeof(DataTemplateSelector), typeof(FileBrowserListingSectionControl), new PropertyMetadata(null));
    }
}
