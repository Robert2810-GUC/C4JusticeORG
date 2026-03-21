using Microsoft.EntityFrameworkCore;
using C4Justice.Web.Data;
using C4Justice.Web.Models;
using C4Justice.Web.Helpers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(4);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Ensure DB and seed admin
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        db.Database.EnsureCreated();

        // Ensure SiteSettings table exists (handles DB created before this table was added)
        db.Database.ExecuteSqlRaw(@"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='SiteSettings' AND xtype='U')
            CREATE TABLE SiteSettings (
                Id INT IDENTITY(1,1) PRIMARY KEY,
                [Key] NVARCHAR(100) NOT NULL,
                [Value] NVARCHAR(MAX) NULL,
                Label NVARCHAR(200) NULL,
                [Group] NVARCHAR(50) NULL,
                CONSTRAINT UQ_SiteSettings_Key UNIQUE ([Key])
            )");

        if (!db.AdminUsers.Any())
        {
            db.AdminUsers.Add(new AdminUser
            {
                Username = "admin",
                PasswordHash = AuthHelper.HashPassword("Admin@123"),
                Email = "admin@cu4justice.com",
                CreatedAt = DateTime.UtcNow
            });
            db.SaveChanges();
        }

        // Seed default site settings
        var defaultSettings = new[]
        {
            // Hero
            new SiteSetting { Key="hero_tagline",  Value="\"United We Stand. Divided We Fall.\"", Label="Hero Tagline", Group="Hero" },
            new SiteSetting { Key="hero_title_1",  Value="Communities",       Label="Hero Title Line 1", Group="Hero" },
            new SiteSetting { Key="hero_title_2",  Value="United for",        Label="Hero Title Line 2", Group="Hero" },
            new SiteSetting { Key="hero_title_3",  Value="Justice",           Label="Hero Title Line 3", Group="Hero" },
            new SiteSetting { Key="hero_desc",     Value="Fighting for Political, Racial, Economic and Environmental Justice. We believe that all individuals deserve to be treated with fairness and equity.", Label="Hero Description", Group="Hero" },
            // Stats
            new SiteSetting { Key="stat_members",  Value="5000", Label="Community Members Count", Group="Stats" },
            new SiteSetting { Key="stat_events",   Value="48",   Label="Events Hosted Count",     Group="Stats" },
            new SiteSetting { Key="stat_cycles",   Value="3",    Label="Election Cycles Count",   Group="Stats" },
            new SiteSetting { Key="stat_partners", Value="20",   Label="Partner Organizations",   Group="Stats" },
            // About Teaser
            new SiteSetting { Key="about_heading", Value="United We Stand.<br>Divided We Fall.", Label="About Section Heading", Group="About" },
            new SiteSetting { Key="about_lead",    Value="For too long, grassroots organizations have advocated in silos on behalf of marginalized communities when all our social justice issues have a common thread: that all individuals be treated with fairness and equity.", Label="About Lead Text", Group="About" },
            // Mission Quote
            new SiteSetting { Key="quote_text",    Value="A collaboration of grassroots organizations came together over the last three election cycles to improve the efficiency and effectiveness of their collective voter education and mobilization strategies.", Label="Mission Quote", Group="Quote" },
            new SiteSetting { Key="quote_author",  Value="Richard Rose", Label="Quote Author", Group="Quote" },
            new SiteSetting { Key="quote_role",    Value="Founder & Architect, Communities United for Justice", Label="Quote Author Role", Group="Quote" },
        };
        foreach (var s in defaultSettings)
        {
            if (!db.SiteSettings.Any(x => x.Key == s.Key))
                db.SiteSettings.Add(s);
        }
        db.SaveChanges();

        // Ensure upload directories exist
        var uploadsPath = Path.Combine(app.Environment.WebRootPath, "uploads");
        Directory.CreateDirectory(Path.Combine(uploadsPath, "slider"));
        Directory.CreateDirectory(Path.Combine(uploadsPath, "documents"));
    }
    catch { /* DB not yet available - will fail gracefully */ }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "opinion_detail",
    pattern: "Opinion/{slug}",
    defaults: new { controller = "Opinion", action = "Detail" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
