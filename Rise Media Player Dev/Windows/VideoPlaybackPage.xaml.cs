using Rise.Common.Helpers;
using Rise.Common.Interfaces;
using Rise.Data.ViewModels;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Views
{
    public sealed partial class VideoPlaybackPage : Page
    {
        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        private readonly NavigationHelper _navigationHelper;

        private MediaPlaybackViewModel ViewModel => App.MPViewModel;

        public VideoPlaybackPage()
        {
            InitializeComponent();
            _navigationHelper = new NavigationHelper(this);

            Player.SetMediaPlayer(ViewModel.Player);
        }
    }
}
