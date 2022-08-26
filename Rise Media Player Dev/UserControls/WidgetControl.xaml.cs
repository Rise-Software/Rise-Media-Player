using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Rise.App.UserControls
{
    public partial class WidgetControl : UserControl
    {
        public static readonly DependencyProperty IconProperty
            = DependencyProperty.Register(nameof(Icon), typeof(IconElement),
                typeof(WidgetControl), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the widget icon.
        /// </summary>
        public IconElement Icon
        {
            get => (IconElement)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public static readonly DependencyProperty WidgetContentProperty
            = DependencyProperty.Register(nameof(WidgetContent), typeof(object),
                typeof(WidgetControl), new PropertyMetadata(null));
        
        /// <summary>
        /// Gets or sets the widget content.
        /// </summary>
        public object WidgetContent
        {
            get => GetValue(WidgetContentProperty);
            set => SetValue(WidgetContentProperty, value);
        }

        public static readonly DependencyProperty MoreFlyoutProperty
            = DependencyProperty.Register(nameof(MoreFlyout), typeof(FlyoutBase),
                typeof(WidgetControl), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the widget more button's flyout.
        /// </summary>
        public FlyoutBase MoreFlyout
        {
            get => (FlyoutBase)GetValue(MoreFlyoutProperty);
            set => SetValue(MoreFlyoutProperty, value);
        }

        public static readonly DependencyProperty TitleProperty
            = DependencyProperty.Register(nameof(Title), typeof(string),
                typeof(WidgetControl), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets the widget title.
        /// </summary>
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty MoreButtonVisibilityProperty
            = DependencyProperty.Register(nameof(MoreButtonVisibility), typeof(Visibility),
                typeof(WidgetControl), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Gets or sets the widget more button's visibility.
        /// </summary>
        public Visibility MoreButtonVisibility
        {
            get => (Visibility)GetValue(MoreButtonVisibilityProperty);
            set => SetValue(MoreButtonVisibilityProperty, value);
        }

        public static readonly DependencyProperty ShowTitleProperty
            = DependencyProperty.Register(nameof(ShowTitle), typeof(bool),
                typeof(WidgetControl), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets the widget title's visibility.
        /// </summary>
        public bool ShowTitle
        {
            get => (bool)GetValue(ShowTitleProperty);
            set => SetValue(ShowTitleProperty, value);
        }

        public static readonly DependencyProperty ShowIconProperty
            = DependencyProperty.Register(nameof(ShowTitle), typeof(bool),
                typeof(WidgetControl), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets the widget icon's visibility.
        /// </summary>
        public bool ShowIcon
        {
            get => (bool)GetValue(ShowIconProperty);
            set => SetValue(ShowIconProperty, value);
        }

        public WidgetControl()
        {
            InitializeComponent();
        }
    }
}
