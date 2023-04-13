using Rise.App.Settings;
using Rise.App.ViewModels;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.UserControls
{
    public sealed partial class IndexingTipControl : UserControl
    {
        private MainViewModel MViewModel => App.MViewModel;

        public static readonly DependencyProperty IconContentProperty
            = DependencyProperty.Register(nameof(IconContent), typeof(object),
                typeof(IndexingTipControl), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the icon content of the tip.
        /// </summary>
        public object IconContent
        {
            get => GetValue(IconContentProperty);
            set => SetValue(IconContentProperty, value);
        }

        public static readonly DependencyProperty TextContentProperty
            = DependencyProperty.Register(nameof(TextContent), typeof(object),
                typeof(IndexingTipControl), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the content of the tip.
        /// </summary>
        public object TextContent
        {
            get => GetValue(TextContentProperty);
            set => SetValue(TextContentProperty, value);
        }

        public static readonly DependencyProperty ProgressBarVisibilityProperty
            = DependencyProperty.Register(nameof(ProgressBarVisibility), typeof(Visibility),
                typeof(IndexingTipControl), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Gets or sets the tip's progress bar visibility.
        /// </summary>
        public Visibility ProgressBarVisibility
        {
            get => (Visibility)GetValue(ProgressBarVisibilityProperty);
            set => SetValue(ProgressBarVisibilityProperty, value);
        }

        public static readonly DependencyProperty ProgressBarValueProperty
            = DependencyProperty.Register(nameof(ProgressBarValue), typeof(uint),
                typeof(IndexingTipControl), new PropertyMetadata(0u));

        /// <summary>
        /// Gets or sets the value of the tip's progress bar.
        /// </summary>
        public uint ProgressBarValue
        {
            get => (uint)GetValue(ProgressBarValueProperty);
            set => SetValue(ProgressBarValueProperty, value);
        }

        public static readonly DependencyProperty ProgressBarMaxValueProperty
            = DependencyProperty.Register(nameof(ProgressBarMaxValue), typeof(uint),
                typeof(IndexingTipControl), new PropertyMetadata(100u));

        /// <summary>
        /// Gets or sets the maximum value of the tip's progress bar.
        /// </summary>
        public uint ProgressBarMaxValue
        {
            get => (uint)GetValue(ProgressBarMaxValueProperty);
            set => SetValue(ProgressBarMaxValueProperty, value);
        }

        public static readonly DependencyProperty ProgressBarMinValueProperty
            = DependencyProperty.Register(nameof(ProgressBarMinValue), typeof(uint),
                typeof(IndexingTipControl), new PropertyMetadata(0u));

        /// <summary>
        /// Gets or sets the minimum value of the tip's progress bar.
        /// </summary>
        public uint ProgressBarMinValue
        {
            get => (uint)GetValue(ProgressBarMinValueProperty);
            set => SetValue(ProgressBarMinValueProperty, value);
        }

        public event RoutedEventHandler DismissButtonClick; 

        public IndexingTipControl()
        {
            InitializeComponent();
        }

        private void GoToScanningSettings_Click(object sender, RoutedEventArgs e)
            => _ = ((Frame)Window.Current.Content).Navigate(typeof(AllSettingsPage));

        private void DismissButton_Click(object sender, RoutedEventArgs e)
            => DismissButtonClick?.Invoke(sender, e);
    }
}
