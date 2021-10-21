using System;
using System.Collections.ObjectModel;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace RMP.App.SongHub
{
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
                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => songNames.Add(eachSong.SongName));
                    }
                }
            });
        }
    }
}