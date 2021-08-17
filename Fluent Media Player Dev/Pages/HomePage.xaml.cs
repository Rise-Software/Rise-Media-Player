using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Fluent_Media_Player_Dev
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        public HomePage()
        {
            this.InitializeComponent();
            ConnectServiceNotification.IsOpen = true;
        }

        private void DropDownButton_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}