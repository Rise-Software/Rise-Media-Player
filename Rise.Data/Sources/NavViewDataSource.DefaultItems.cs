using Rise.Data.Navigation;

namespace Rise.Data.Sources
{
    // Default collection of items for this data source
    public sealed partial class NavViewDataSource
    {
        private readonly NavigationItemBase[] _defaultItems = new NavigationItemBase[]
        {
            new NavigationItemDestination()
            {
                Id = "HomePage",
                Group = "General",
                DefaultIcon = "\uECA5",
                Label = "Home",
                AccessKey = "H",
                FlyoutId = "DefaultItemFlyout"
            },
            new NavigationItemDestination()
            {
                Id = "PlaylistsPage",
                Group = "General",
                DefaultIcon = "\uE8FD",
                Label = "Playlists",
                AccessKey = "P",
                FlyoutId = "DefaultItemFlyout"
            },

            new NavigationItemHeader()
            {
                Id = "MHeader",
                Group = "Music",
                Label = "Music"
            },
            new NavigationItemDestination()
            {
                Id = "SongsPage",
                Group = "Music",
                DefaultIcon = "\uEC4F",
                Label = "Songs",
                AccessKey = "N",
                FlyoutId = "DefaultItemFlyout"
            },
            new NavigationItemDestination()
            {
                Id = "ArtistsPage",
                Group = "Music",
                DefaultIcon = "\uE125",
                Label = "Artists",
                AccessKey = "T",
                FlyoutId = "DefaultItemFlyout"
            },
            new NavigationItemDestination()
            {
                Id = "AlbumsPage",
                Group = "Music",
                DefaultIcon = "\uE93C",
                Label = "Albums",
                AccessKey = "A",
                FlyoutId = "DefaultItemFlyout"
            },

            new NavigationItemHeader()
            {
                Id = "VHeader",
                Group = "Videos",
                Label = "Videos"
            },
            new NavigationItemDestination()
            {
                Id = "LocalVideosPage",
                Group = "Videos",
                DefaultIcon = "\uE8B2",
                Label = "LocalVideos",
                AccessKey = "V",
                FlyoutId = "RemoveItemFlyout"
            },

            new NavigationItemDestination()
            {
                Id = "DiscyPage",
                Group = "General",
                IsFooter = true,
                DefaultIcon = "\uE9CE",
                Label = "Discy",
                AccessKey = "C",
                FlyoutId = "RemoveItemFlyout"
            },
            new NavigationItemDestination()
            {
                Id = "SettingsPage",
                Group = "Other",
                DefaultIcon = "\uE115",
                Label = "Settings",
                AccessKey = "S",
                IsFooter = true
            }
        };
    }
}
