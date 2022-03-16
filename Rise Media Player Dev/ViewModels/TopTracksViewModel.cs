namespace Rise.App.ViewModels
{
    public class TopTracks
    {
        public string SongName { get; set; }
        public string ArtistName { get; set; }
        public string Image { get; set; }
        public TopTracks(string songName, string artist, string image)
        {
            SongName = songName;
            ArtistName = artist;
            Image = image;
        }
    }
}
