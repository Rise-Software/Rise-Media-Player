using Rise.App.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Dialogs
{
    public sealed partial class MessagesDialog : Page
    {
        private MainViewModel MViewModel => App.MViewModel;

        public static readonly DependencyProperty SelectedNotificationProperty =
            DependencyProperty.Register(nameof(SelectedNotification), typeof(NotificationViewModel),
                typeof(MessagesDialog), new PropertyMetadata(null));

        public NotificationViewModel SelectedNotification
        {
            get => (NotificationViewModel)GetValue(SelectedNotificationProperty);
            set => SetValue(SelectedNotificationProperty, value);
        }

        public MessagesDialog()
        {
            InitializeComponent();
        }
    }

    // Event handlers
    public sealed partial class MessagesDialog : Page
    {
        private void MenuFlyout_Opening(object sender, object e)
        {
            var fl = sender as MenuFlyout;
            var cont = NotificationsList.ItemFromContainer(fl.Target);

            if (cont == null)
                fl.Hide();
            else
                SelectedNotification = (NotificationViewModel)cont;
        }

        private async void DeleteMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedNotification != null)
                await SelectedNotification.DeleteAsync();
        }
    }
}
