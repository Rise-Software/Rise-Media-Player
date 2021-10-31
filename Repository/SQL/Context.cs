using Microsoft.EntityFrameworkCore;
using Rise.Models;

namespace Rise.Repository.SQL
{
    public class Context : DbContext
    {
        /// <summary>
        /// Creates a new DbContext.
        /// </summary>
        public Context(DbContextOptions<Context> options) : base(options)
        { }

        /// <summary>
        /// Gets the songs DbSet.
        /// </summary>
        public DbSet<Song> Songs { get; set; }

        /// <summary>
        /// Gets the albums DbSet.
        /// </summary>
        public DbSet<Album> Albums { get; set; }

        /// <summary>
        /// Gets the artists DbSet.
        /// </summary>
        public DbSet<Artist> Artists { get; set; }

        /// <summary>
        /// Gets the genres DbSet.
        /// </summary>
        public DbSet<Genre> Genres { get; set; }

        /// <summary>
        /// Gets the videos DbSet.
        /// </summary>
        // public DbSet<Video> Videos { get; set; }
    }
}
