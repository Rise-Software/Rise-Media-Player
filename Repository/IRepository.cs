namespace Rise.Repository
{
    public interface IRepository
    {
        /// <summary>
        /// Returns the songs repository.
        /// </summary>
        ISongRepository Songs { get; }
        
        /// <summary>
        /// Returns the albums repository.
        /// </summary>
        IAlbumRepository Albums { get; }

        /// <summary>
        /// Returns the videos repository.
        /// </summary>
        // IVideoRepository Videos { get; }
    }
}
