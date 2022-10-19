using System;
using Rise.Common.Interfaces;
using Rise.Data.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Rise.App.UserControls
{
    /// <summary>
    /// A flyout that shows queued media. Supports displaying queues with
    /// <see cref="IMediaItem"/>.
    /// </summary>
    public sealed partial class DefaultQueueFlyout : UserControl
    {
        private MediaPlaybackViewModel MPViewModel => App.MPViewModel;

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(nameof(SelectedItem), typeof(IMediaItem),
                typeof(DefaultQueueFlyout), new PropertyMetadata(null));

        public IMediaItem SelectedItem
        {
            get => (IMediaItem)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public DefaultQueueFlyout()
        {
            InitializeComponent();
        }
    }

    // Event handlers
    public sealed partial class DefaultQueueFlyout
    {
        private void MenuFlyout_Opening(object sender, object e)
        {
            var fl = sender as MenuFlyout;
            var cont = MainList.ItemFromContainer(fl.Target);

            if (cont == null)
                fl.Hide();
            else
                SelectedItem = (IMediaItem)cont;
        }

        private void PlayItem_Click(object sender, RoutedEventArgs e)
        {
            if (MPViewModel.PlayingItem == SelectedItem)
            {
                MPViewModel.Player.PlaybackSession.Position = new TimeSpan(0);
            }
            else
            {
                int index = MainList.SelectedIndex;
                MPViewModel.PlaybackList.MoveTo((uint)index);
            }
        }

        private void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            int index = MPViewModel.QueuedItems.IndexOf(SelectedItem);

            if (MPViewModel.PlayingItem == SelectedItem)
                MPViewModel.PlaybackList.MoveNext();

            if (index >= 0)
            {
                MPViewModel.PlaybackList.Items.RemoveAt(index);
                MPViewModel.QueuedItems.RemoveAt(index);
            }
        }

        private async void MoveItemUp_Click(object sender, RoutedEventArgs e)
        {
            if ((MPViewModel.QueuedItems.IndexOf(SelectedItem) - 1) >= 0)
            {
                var index = MPViewModel.QueuedItems.IndexOf(SelectedItem);

                MPViewModel.QueuedItems.Remove(SelectedItem);
                MPViewModel.QueuedItems.Insert(index - 1, SelectedItem);

                MPViewModel.PlaybackList.Items.RemoveAt(index);
                MPViewModel.PlaybackList.Items.Insert(index - 1, await SelectedItem.AsPlaybackItemAsync());
            }
        }

        private async void MoveItemDown_Click(object sender, RoutedEventArgs e)
        {
            if ((MPViewModel.QueuedItems.IndexOf(SelectedItem) + 1) < MPViewModel.QueuedItems.Count)
            {
                var index = MPViewModel.QueuedItems.IndexOf(SelectedItem);

                MPViewModel.QueuedItems.Remove(SelectedItem);
                MPViewModel.QueuedItems.Insert(index + 1, SelectedItem);

                MPViewModel.PlaybackList.Items.RemoveAt(index);
                MPViewModel.PlaybackList.Items.Insert(index + 1, await SelectedItem.AsPlaybackItemAsync());
            }
        }

        private void MainList_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            var index = MPViewModel.QueuedItems.IndexOf((e.OriginalSource as FrameworkElement).DataContext as IMediaItem);

            if (index >= 0)
                MPViewModel.PlaybackList.MoveTo((uint)index);
        }
    }
}
