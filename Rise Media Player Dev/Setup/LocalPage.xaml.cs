using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace RMP.App.Setup
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LocalPage : Page
    {
        private List<string> Deletion { get; set; }

        public LocalPage()
        {
            this.InitializeComponent();

            Deletion = new List<string>
            {
                ResourceLoaders.MediaLibraryLoader.GetString("OnlyApp"),
                ResourceLoaders.MediaLibraryLoader.GetString("Device")
            };
        }
    }
}
