using Rise.App.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using static Rise.App.Common.Enums;

namespace Rise.App.UserControls
{
    [ContentProperty(Name = "Controls")]
    public sealed partial class NavigationExpander : UserControl
    {
        private readonly ExpanderViewModel ViewModel = new ExpanderViewModel();

        public NavigationExpander()
        {
            InitializeComponent();
        }

        /// <summary>
        /// <inheritdoc cref="ExpanderViewModel.Title"/>
        /// </summary>
        public string Title
        {
            get => ViewModel.Title;
            set => ViewModel.Title = value;
        }

        /// <summary>
        /// <inheritdoc cref="ExpanderViewModel.Description"/>
        /// </summary>
        public string Description
        {
            get => ViewModel.Description;
            set => ViewModel.Description = value;
        }

        /// <summary>
        /// <inheritdoc cref="ExpanderViewModel.ExpanderStyle"/>
        /// </summary>
        public ExpanderStyles ExpanderStyle
        {
            get => ViewModel.ExpanderStyle;
            set => ViewModel.ExpanderStyle = value;
        }

        /// <summary>
        /// <inheritdoc cref="ExpanderViewModel.Icon"/>
        /// </summary>
        public string Icon
        {
            get => ViewModel.Icon;
            set => ViewModel.Icon = value;
        }

        /// <summary>
        /// <inheritdoc cref="ExpanderViewModel.Controls"/>
        /// </summary>
        public object Controls
        {
            get => ViewModel.Controls;
            set => ViewModel.Controls = value;
        }

        /// <summary>
        /// <inheritdoc cref="ExpanderViewModel.HeaderControls"/>
        /// </summary>
        public object HeaderControls
        {
            get => ViewModel.Controls;
            set => ViewModel.Controls = value;
        }

        public event RoutedEventHandler Click;
        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            Click?.Invoke(this, e);
        }
    }

    public class ExpanderTemplateSelector : DataTemplateSelector
    {
        public ExpanderStyles Style { get; set; }

        public DataTemplate Default { get; set; }
        public DataTemplate Static { get; set; }
        public DataTemplate Button { get; set; }
        public DataTemplate Transparent { get; set; }
        public DataTemplate Disabled { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            switch (Style)
            {
                case ExpanderStyles.Static:
                    return Static;

                case ExpanderStyles.Button:
                    return Button;

                case ExpanderStyles.Transparent:
                    return Transparent;

                case ExpanderStyles.Disabled:
                    return Disabled;

                default:
                    return Default;
            }
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            switch (Style)
            {
                case ExpanderStyles.Static:
                    return Static;

                case ExpanderStyles.Button:
                    return Button;

                case ExpanderStyles.Transparent:
                    return Transparent;

                case ExpanderStyles.Disabled:
                    return Disabled;

                default:
                    return Default;
            }
        }
    }
}
