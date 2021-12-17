using Rise.App.Common;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using static Rise.App.Common.Enums;

namespace Rise.App.Dialogs
{
    public sealed partial class FeatureDialog : ContentDialog
    {
        private readonly ObservableCollection<Feature> _features =
            new ObservableCollection<Feature>();

        private int _index;
        private int Index
        {
            get => _index;
            set
            {
                _index = value;
                if (_index == 0)
                {
                    Left.IsEnabled = false;
                }
                else
                {
                    Left.IsEnabled = true;
                    if (_index == _features.Count - 1)
                    {
                        Right.IsEnabled = false;
                    }
                    else
                    {
                        Right.IsEnabled = true;
                    }
                }

                RefreshData();
            }
        }

        public FeatureDialog()
        {
            _features.Add(new Feature
            {
                Name = "Playlist Support",
                Description = "We’ve finally added support for playlists to Rise Media Player. Add a song to your playlist by hovering on it. You can also import and export playlists, sort them by date created, and more!",
                ImageUri = "ms-appx:///Assets/NavigationView/LocalVideosPage/Colorful.png"
            });

            _features.Add(new Feature
            {
                Name = "Artists & Albums",
                Description = "The genre page now works, and you can browse your music by genre. This is very basic currently and we’ll be adding more features to it soon.",
                ImageUri = "ms-appx:///Assets/NavigationView/GenresPage/Colorful.png"
            });

            _features.Add(new Feature
            {
                Name = "Now Playing Bar",
                Description = "The now playing bar is back with a new design! You can now view the song title, artist and thumbnail from the now playing bar and view a song's album in one click!",
                ImageUri = "ms-appx:///Assets/NavigationView/NowPlayingPage/Colorful.png"
            });

            _features.Add(new Feature
            {
                Name = "Contextual help",
                Description = "Now you can experience Discy in context menus!",
                ImageUri = "ms-appx:///Assets/Settings/Sidebar.png"
            });

            _features.Add(new Feature
            {
                Name = "Messages & reports",
                Description = "If the app is crashing or something unexpected happened, there's now more details on what happened, like stack trace, the exception name etc...",
                ImageUri = "ms-appx:///Assets/NavigationView/DiscyPage/Colorful.png"
            });
            InitializeComponent();
        }

        public async Task OpenFeatureAsync(int index)
        {
            Index = index;
            _ = await this.ShowAsync(ExistingDialogOptions.CloseExisting);
        }

        private void RefreshData()
        {
            FeatureName.Text = _features[Index].Name;
            Description.Text = _features[Index].Description;
            Screenshot.UriSource = new Uri(_features[Index].ImageUri);

            CurrentIndex.Text = (Index + 1).ToString();
        }

        private void Left_Click(object sender, RoutedEventArgs e)
            => Index--;

        private void Right_Click(object sender, RoutedEventArgs e)
            => Index++;

        private void Button_Click(object sender, RoutedEventArgs e)
            => Hide();

        private void ContentDialog_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double windowWidth = Window.Current.Bounds.Width;
            double windowHeight = Window.Current.Bounds.Height;

            RootGrid.Width = windowWidth < 498 ? windowWidth - 68 : 498 - 68;

            // The 59 is because for some reason the dialog has a 1px transparent
            // line at the bottom. Don't shoot me, I'm just the messenger.
            RootGrid.Height = windowHeight < 398 ? windowHeight - 59 : 398 - 59;
        }
    }

    public class Feature
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUri { get; set; }
    }
}
