using Microsoft.EntityFrameworkCore;

namespace WP2NOPMigrator
{
    public class NopDbContext : DbContext
    {
        public DbSet<BlogPost> BlogPosts { get; set; }

        public DbSet<UrlRecord> UrlRecords { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"Server=localhost;Database=nopcommerce_db;User Id=sa;Password=nopCommerce_db_password;Encrypt=false;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BlogPost>().ToTable("BlogPost");
            modelBuilder.Entity<UrlRecord>().ToTable("UrlRecord");
        }
    }
}