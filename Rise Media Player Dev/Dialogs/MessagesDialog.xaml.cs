using Rise.App.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Rise.App.Dialogs
{
    public sealed partial class MessagesDialog : Page
    {
        private NotificationViewModel SelectedNotification
        {
            get => (NotificationViewModel)GetValue(SelectedNotificationProperty);
            set => SetValue(SelectedNotificationProperty, value);
        }

        public MessagesDialog()
        {
            InitializeComponent();
        }

        private void ListView_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            (NotificationsList.Resources["ListMenu"] as MenuFlyout).ShowAt(NotificationsList, e.GetPosition(NotificationsList));
            SelectedNotification = (e.OriginalSource as FrameworkElement).DataContext as NotificationViewModel;
        }

        private async void DeleteMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedNotification != null)
                await SelectedNotification.DeleteAsync();
        }

        private void NotificationsList_Tapped(object sender, TappedRoutedEventArgs e)
        {
            SelectedNotification = (e.OriginalSource as FrameworkElement).DataContext as NotificationViewModel;
        }
    }

    // Dependency properties
    public sealed partial class MessagesDialog : Page
    {
        private static readonly DependencyProperty SelectedNotificationProperty =
            DependencyProperty.Register(nameof(SelectedNotification), typeof(NotificationViewModel),
                typeof(MessagesDialog), new PropertyMetadata(null));
    }
}
