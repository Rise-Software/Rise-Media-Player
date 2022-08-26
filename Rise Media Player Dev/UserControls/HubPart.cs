using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media.Animation;

namespace Rise.App.UserControls
{
    /// <summary>
    /// An implementation of <see cref="HubSection"/> with a
    /// property to set content without a template.
    /// </summary>
    [ContentProperty(Name = "Content")]
    public class HubPart : HubSection
    {
        /// <summary>
        /// Identifies the <see cref="Content"/> dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="Content"/>
        /// dependency property.</returns>
        public static DependencyProperty ContentProperty { get; }
            = DependencyProperty.Register(nameof(Content), typeof(object),
                typeof(HubPart), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the content of a <see cref="HubPart"/>.
        /// </summary>
        /// <returns>An object that contains the control's content.
        /// The default is null.</returns>
        public object Content
        {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ContentTemplateSelector"/> dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="ContentTemplateSelector"/>
        /// dependency property.</returns>
        public static DependencyProperty ContentTemplateSelectorProperty { get; }
            = DependencyProperty.Register(nameof(ContentTemplateSelector), typeof(DataTemplateSelector),
                typeof(HubPart), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets a selection object that changes the <see cref="DataTemplate"/>
        /// to apply for content, based on processing information about the content item
        /// or its container at run time.
        /// </summary>
        /// <returns>A selection object that changes the <see cref="DataTemplate"/>
        /// to apply for content.</returns>
        public DataTemplateSelector ContentTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(ContentTemplateSelectorProperty);
            set => SetValue(ContentTemplateSelectorProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ContentTransitions"/> dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="ContentTransitions"/>
        /// dependency property.</returns>
        public static DependencyProperty ContentTransitionsProperty { get; }
            = DependencyProperty.Register(nameof(ContentTransitions), typeof(TransitionCollection),
                typeof(HubPart), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the collection of <see cref="Transition"/> style elements
        /// that apply to the content of a <see cref="HubPart"/>.
        /// </summary>
        /// <returns>The strongly typed collection of <see cref="Transition"/> style
        /// elements.</returns>
        public TransitionCollection ContentTransitions
        {
            get => (TransitionCollection)GetValue(ContentTransitionsProperty);
            set => SetValue(ContentTransitionsProperty, value);
        }

        /// <summary>
        /// Initializes a new instance of the HubPart class.
        /// </summary>
        public HubPart()
        {
            DefaultStyleKey = typeof(HubPart);
        }
    }
}
