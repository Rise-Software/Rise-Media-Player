using System;
using System.Collections.ObjectModel;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace RMP.App.SongHub
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LocalSongsView : Page
    {
        private SongFactory songFactory;
        private ObservableCollection<string> songNames = new ObservableCollection<string>();

        public LocalSongsView()
        {
            InitializeComponent();
            itemsControl.ItemsSource = songNames;
            songFactory = SongFactory.Create();

            _ = songFactory.GetMusicFiles().ContinueWith(async t =>
            {
                if (t.IsCompletedSuccessfully && t.Result != null)
                {
                    foreach (OfflineSong eachSong in t.Result)
                    {
                        await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => songNames.Add(eachSong.SongName));
                    }
                }
            });
        }
    }
}