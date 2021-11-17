using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Playback;

namespace Rise.App.ViewModels
{
    public class VideoPlaybackViewModel : ViewModel
    {
        /// <summary>
        /// Creates a new <see cref="VideoPlaybackViewModel"/>.
        /// </summary>
        public VideoPlaybackViewModel()
        {
            Player.Source = PlaybackList;
        }

        #region Variables
        public MediaPlayer Player { get; }
            = new MediaPlayer();

        public MediaPlaybackList PlaybackList { get; set; }
            = new MediaPlaybackList();
        #endregion

        public async Task PlayVideoAsync(VideoViewModel video)
        {
            Player.Source = await video.AsPlaybackItemAsync();
            Player.Play();
        }
    }
}
