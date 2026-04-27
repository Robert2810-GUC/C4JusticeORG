using C4Justice.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace C4Justice.Web.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<AdminUser>              AdminUsers              => Set<AdminUser>();
        public DbSet<SliderImage>            SliderImages            => Set<SliderImage>();
        public DbSet<Article>                Articles                => Set<Article>();
        public DbSet<SiteEvent>              Events                  => Set<SiteEvent>();
        public DbSet<SiteDocument>           Documents               => Set<SiteDocument>();
        public DbSet<SiteSetting>            SiteSettings            => Set<SiteSetting>();
        public DbSet<ContactSubmission>      ContactSubmissions      => Set<ContactSubmission>();
        public DbSet<VolunteerSignup>        VolunteerSignups        => Set<VolunteerSignup>();
        public DbSet<NewsletterSubscription> NewsletterSubscriptions => Set<NewsletterSubscription>();
        public DbSet<SpeakerRequest>         SpeakerRequests         => Set<SpeakerRequest>();
        public DbSet<OpinionPost>            OpinionPosts            => Set<OpinionPost>();
        public DbSet<GalleryPhoto>           GalleryPhotos           => Set<GalleryPhoto>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Article>()
                .HasIndex(a => a.Slug)
                .IsUnique();

            modelBuilder.Entity<OpinionPost>()
                .HasIndex(o => o.Slug)
                .IsUnique();

            modelBuilder.Entity<AdminUser>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<SiteSetting>()
                .HasIndex(s => s.Key)
                .IsUnique();

            modelBuilder.Entity<SliderImage>().ToTable("sliderimages");
            modelBuilder.Entity<AdminUser>().ToTable("adminusers");
            modelBuilder.Entity<Article>().ToTable("articles");
            modelBuilder.Entity<SiteEvent>().ToTable("events");
            modelBuilder.Entity<SiteDocument>().ToTable("documents");
            modelBuilder.Entity<SiteSetting>().ToTable("sitesettings");
            modelBuilder.Entity<ContactSubmission>().ToTable("contactsubmissions");
            modelBuilder.Entity<VolunteerSignup>().ToTable("volunteersignups");
            modelBuilder.Entity<NewsletterSubscription>().ToTable("newslettersubscriptions");
            modelBuilder.Entity<SpeakerRequest>().ToTable("speakerrequests");
            modelBuilder.Entity<OpinionPost>().ToTable("opinionposts");
            modelBuilder.Entity<GalleryPhoto>().ToTable("galleryphotos");
        }
    }
}
