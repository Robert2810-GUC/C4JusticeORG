using Microsoft.EntityFrameworkCore;
using C4Justice.Web.Models;

namespace C4Justice.Web.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<AdminUser> AdminUsers => Set<AdminUser>();
        public DbSet<SliderImage> SliderImages => Set<SliderImage>();
        public DbSet<Article> Articles => Set<Article>();
        public DbSet<SiteEvent> Events => Set<SiteEvent>();
        public DbSet<SiteDocument> Documents => Set<SiteDocument>();
        public DbSet<SiteSetting> SiteSettings => Set<SiteSetting>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Article>()
                .HasIndex(a => a.Slug)
                .IsUnique();

            modelBuilder.Entity<AdminUser>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<SiteSetting>()
                .HasIndex(s => s.Key)
                .IsUnique();
        }
    }
}
