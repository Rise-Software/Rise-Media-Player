using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace RMP.App.Settings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NavigationPage : Page
    {
        private List<string> Show { get; set; }

        public NavigationPage()
        {
            InitializeComponent();
            Show = new List<string>
            {
                ResourceLoaders.NavigationLoader.GetString("NoIcons"),
                ResourceLoaders.NavigationLoader.GetString("OnlyIcons"),
                ResourceLoaders.NavigationLoader.GetString("Everything")
            };
        }
    }
}
