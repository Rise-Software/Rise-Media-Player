namespace RMP.App.SongHub
{
    internal class OfflineSong : BaseSong
    {
        public string FilePath { get; set; }

        public static OfflineSong Create(string songName, string filePath)
        {
            return new OfflineSong() { SongName = songName, FilePath = filePath };
        }
    }
}