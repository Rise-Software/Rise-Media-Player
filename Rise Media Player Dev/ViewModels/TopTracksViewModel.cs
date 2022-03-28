namespace Rise.App.ViewModels
{
    public class TopTracksViewModel
    {
        public string SongName { get; set; }
        public string ArtistName { get; set; }
        public string Rank { get; set; }
        public TopTracksViewModel(string songName/*, string artist*/, string rank)
        {
            SongName = songName;
            // ArtistName = artist;
            Rank = rank + ".";
        }
    }
}
