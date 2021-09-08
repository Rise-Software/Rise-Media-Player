using System;
using System.Collections.Generic;
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

namespace RMP.App.Settings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PlaybackPage : Page
    {
        private List<string> Crossfade { get; set; }
        private List<string> VideoScale { get; set; }

        private Windows.ApplicationModel.Resources.ResourceLoader resourceLoader =
            Windows.ApplicationModel.Resources.
                ResourceLoader.GetForCurrentView("Playback");

        public PlaybackPage()
        {
            this.InitializeComponent();

            Crossfade = new List<string>
            {
                resourceLoader.GetString("Duration0"),
                resourceLoader.GetString("Duration3s"),
                resourceLoader.GetString("Duration5s"),
                resourceLoader.GetString("Duration10s")
            };

            VideoScale = new List<string>
            {
                resourceLoader.GetString("WindowSize"),
                resourceLoader.GetString("MatchRes")
            };
        }
    }
}
