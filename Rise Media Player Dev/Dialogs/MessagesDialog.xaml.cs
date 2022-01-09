using Rise.App.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Rise.App.Dialogs
{
    public sealed partial class MessagesDialog : Page
    {
        public NotificationViewModel SelectedNotification { get; set; }

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
            {
                await SelectedNotification.DeleteAsync();
            }
        }

        private void NotificationsList_Tapped(object sender, TappedRoutedEventArgs e)
        {
            SelectedNotification = (e.OriginalSource as FrameworkElement).DataContext as NotificationViewModel;
            Title.Text = SelectedNotification.Title;
            Description.Text = SelectedNotification.Description;
        }
    }
}
