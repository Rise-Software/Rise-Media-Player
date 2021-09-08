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
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace RMP.App.UserControls
{
    [ContentProperty(Name = "Controls")]
    public sealed partial class NavigationExpander : UserControl
    {
        public NavigationExpander()
        {
            this.InitializeComponent();
        }

        public string Title { get; set; }
        public string Description { get; set; }
        public int ControlKind { get; set; }

        public string ExpanderKind { get; set; }
        private readonly string Button = "Button";
        private readonly string Expander = "Expander";
        private readonly string Static = "Static";

        public string Icon { get; set; }

        public static DependencyProperty ControlsProperty =
            DependencyProperty.Register("Controls", typeof(object), typeof(NavigationExpander), null);

        public object Controls
        {
            get => GetValue(ControlsProperty);
            set => SetValue(ControlsProperty, value);
        }

        public event RoutedEventHandler Click;
        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            if (this.Click != null)
            {
                this.Click(this, e);
            }
        }
    }
}
