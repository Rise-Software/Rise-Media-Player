using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace RMP.App.Setup
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppearancePage : Page
    {
        private List<string> Themes { get; set; }
        public AppearancePage()
        {
            this.InitializeComponent();

            Themes = new List<string>
            {
                ResourceLoaders.AppearanceLoader.GetString("Light"),
                ResourceLoaders.AppearanceLoader.GetString("Dark"),
                ResourceLoaders.AppearanceLoader.GetString("System")
            };
        }
    }
}
