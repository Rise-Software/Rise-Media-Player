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
                Name = "Video Support",
                Description = "We’ve finally added full fledged video to Rise Media Player. Watch videos from your device by adding them to your media sources from settings. We already look in the default videos folder. This support is basic for now and more features will be coming soon.",
                ImageUri = "ms-appx:///Assets/NavigationView/LocalVideosPage/Colorful.png"
            });

            _features.Add(new Feature
            {
                Name = "Extended Genres",
                Description = "The genre page now works, and you can browse your music by genre. This is very basic currently and we’ll be adding more features to it soon.",
                ImageUri = "ms-appx:///Assets/NavigationView/GenresPage/Colorful.png"
            });

            _features.Add(new Feature
            {
                Name = "More to personalise",
                Description = "You can now add more colour to our app with a new feature we’re calling Window Glaze! Choose to match your system accent colour or select your own colour from our list. We’re bringing a colour picker in soon you can have exact control. This colour will ‘glaze’ or tint the app window, while still retaining the mica material. Note: This will work on Windows 10, but of course, no Mica. Also, you can make this match the album art of the currently playing song, meaning it’s always changing!",
                ImageUri = "ms-appx:///Assets/Settings/Appearance.png"
            });

            _features.Add(new Feature
            {
                Name = "Improved Navigation",
                Description = "In the previous release, we added the option to remove entire sections from the sidebar. This is now possible from the right click context menu on the sidebar itself. Additionally, you can now move around items in your sidebar, moving them up and down, meaning stuff that is more important to you stays close.",
                ImageUri = "ms-appx:///Assets/Settings/Sidebar.png"
            });

            _features.Add(new Feature
            {
                Name = "New Help Centre",
                Description = "Help & Tips is back in the form of the help centre! Access basic written help and support and there may even be a certain round friend popping their head in from time to time!",
                ImageUri = "ms-appx:///Assets/NavigationView/DiscyPage/Colorful.png"
            });

            _features.Add(new Feature
            {
                Name = "Better Now Playing",
                Description = "Introduction of hover actions. When your mouse leaves the now playing window, all controls and text will recede, leaving your album art to have it’s time to shine. Hovering will reveal your controls again.",
                ImageUri = "ms-appx:///Assets/NavigationView/NowPlayingPage/Colorful.png"
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
    }

    public class Feature
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUri { get; set; }
    }
}
