namespace Fluent_Media_Player_Dev.SongHub
{
    internal class BaseSong
    {
        public string SongName { get; set; }
        public string ArtistName { get; set; }
        public string AlbumName { get; set; }
        public int UserRating { get; set; }
        public double AIScore { get; set; }
        public double Duration { get; set; }
    }
}