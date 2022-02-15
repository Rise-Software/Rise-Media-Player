using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rise.App.ViewModels
{
    public class PlaybackMetaViewModel : ViewModel
    {
        private string _title;

        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }

        private string _thumbnail;

        public string Thumbnail
        {
            get => _thumbnail;
            set => Set(ref _thumbnail, value);
        }

        public bool IsVideo;

        public async Task NotifyChangesAsync(bool isVideo)
        {
            IsVideo = isVideo;
            if (isVideo)
            {
                Title = App.PViewModel.CurrentVideo.Title;
                Thumbnail = App.PViewModel.CurrentVideo.Thumbnail;
            } else
            {
                Title = App.PViewModel.CurrentSong.Title;
                Thumbnail = App.PViewModel.CurrentSong.Thumbnail;
            }
        }

        public void NotifyChanges(bool isVideo)
        {
            IsVideo = isVideo;
            if (isVideo)
            {
                Title = App.PViewModel.CurrentVideo.Title;
                Thumbnail = App.PViewModel.CurrentVideo.Thumbnail;
            }
            else
            {
                Title = App.PViewModel.CurrentSong.Title;
                Thumbnail = App.PViewModel.CurrentSong.Thumbnail;
            }
        }
    }
}
