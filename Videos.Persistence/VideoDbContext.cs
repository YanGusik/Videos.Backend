using Microsoft.EntityFrameworkCore;
using Videos.Application.Interfaces;
using Videos.Domain;
using Videos.Persistence.EntityTypeConfigurations;

namespace Videos.Persistence
{
    public class VideoDbContext : DbContext, IVideoDbContext
    {
        public DbSet<Video> Videos { get; set; }

        public VideoDbContext(DbContextOptions<VideoDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new VideoConfiguration());
            base.OnModelCreating(builder);
        }
    }
}
