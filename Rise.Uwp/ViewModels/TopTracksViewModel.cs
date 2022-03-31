namespace Rise.App.ViewModels
{
    public class TopTracks
    {
        public string SongName { get; set; }
        public string ArtistName { get; set; }
        public string Rank { get; set; }
        public TopTracks(string songName/*, string artist*/, string rank)
        {
            SongName = songName;
            // ArtistName = artist;
            Rank = rank + ".";
        }
    }
}
