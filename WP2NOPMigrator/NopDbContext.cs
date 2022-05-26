using Microsoft.EntityFrameworkCore;

namespace WP2NOPMigrator
{
    public class NopDbContext : DbContext
    {
        public DbSet<BlogPost> BlogPosts { get; set; }

        public DbSet<UrlRecord> UrlRecords { get; set; }
        
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        
        public DbSet<Picture> Pictures { get; set; }
        
        public DbSet<PictureBinary> PictureBinaries { get; set; }

        public NopDbContext(DbContextOptions<NopDbContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BlogPost>().ToTable("BlogPost");
            modelBuilder.Entity<UrlRecord>().ToTable("UrlRecord");
            modelBuilder.Entity<ActivityLog>().ToTable("ActivityLog");
            modelBuilder.Entity<Picture>().ToTable("Picture");
            modelBuilder.Entity<PictureBinary>().ToTable("PictureBinary");

            modelBuilder.Entity<Picture>().Ignore(p => p.Url).Ignore(p=> p.Extension);
        }
    }
}