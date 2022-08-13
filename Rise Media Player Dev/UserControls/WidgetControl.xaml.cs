using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.UserControls
{
    public sealed partial class WidgetControl : UserControl
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

        public WidgetControl()
        {
            InitializeComponent();
        }
    }
}
