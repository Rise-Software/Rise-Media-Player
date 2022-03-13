using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Rise.Common
{
    /// <summary>
    /// Listens to changes to a specified <see cref="DependencyProperty"/>.
    /// </summary>
    /// <typeparam name="T">Property type.</typeparam>
    public class DependencyPropertyWatcher<T> : DependencyObject, IDisposable
    {
        private static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value",
                typeof(T),
                typeof(DependencyPropertyWatcher<T>),
                new PropertyMetadata(null, OnPropertyChanged));

        public T Value
        {
            get => (T)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public DependencyObject Target { get; private set; }
        public event DependencyPropertyChangedEventHandler PropertyChanged;

        public DependencyPropertyWatcher(DependencyObject target, string propertyPath)
        {
            Target = target;
            Binding bind = new()
            {
                Source = target,
                Path = new PropertyPath(propertyPath),
                Mode = BindingMode.OneWay
            };

            BindingOperations.SetBinding(this, ValueProperty, bind);
        }

        private static void OnPropertyChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            DependencyPropertyWatcher<T> source = (DependencyPropertyWatcher<T>)sender;
            source.PropertyChanged?.Invoke(source.Target, args);
        }

        public void Dispose()
        {
            ClearValue(ValueProperty);
        }
    }
}
