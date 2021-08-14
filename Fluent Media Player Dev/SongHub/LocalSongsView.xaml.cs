using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Fluent_Media_Player_Dev.SongHub
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LocalSongsView : Page
    {
        private SongFactory songFactory;
        private ObservableCollection<string> filePaths = new ObservableCollection<string>();

        public LocalSongsView()
        {
            this.InitializeComponent();
            itemsControl.ItemsSource = filePaths;
            songFactory = SongFactory.Create();

            _ = songFactory.GetMusicFiles().ContinueWith(async t =>
            {
                if (t.IsCompletedSuccessfully && t.Result != null)
                {
                    foreach (string path in t.Result)
                    {
                        await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            filePaths.Add(path);
                        });
                    }
                }
            });
        }
    }
}