# C4Justice.org — Project Progress Log

> Last updated: 2026-06-18
> Keep this file updated at the start/end of every chat session.

---

## Project Overview

**Org:** Communities United for Justice Fund (501c3 nonprofit)
**Founder:** Richard Rose
**Stack:** .NET 9.0 ASP.NET Core MVC · MySQL 8 · Bootstrap 5 CDN · Font Awesome 6 · AOS animations
**Project root:** `C:\Bharat\C4Justice\`
**Web project:** `C:\Bharat\C4Justice\C4Justice.Web\`
**Dev server:** `dotnet run --project C4Justice.Web --urls http://localhost:5050`

---

## Architecture

### Public Pages (frontend)
| Route | Controller | View | Status |
|-------|-----------|------|--------|
| `/` | HomeController | Views/Home/Index.cshtml | ✅ Built |
| `/About` | AboutController | Views/About/Index.cshtml | ✅ Built |
| `/Events` | EventsController | Views/Events/Index.cshtml | ✅ Built |
| `/GetInvolved` | GetInvolvedController | Views/GetInvolved/Index.cshtml | ✅ Built |
| `/Opinion` | OpinionController | Views/Opinion/Index.cshtml | ✅ Built |
| `/Opinion/{slug}` | OpinionController | Views/Opinion/Detail.cshtml | ✅ Built |
| `/Contact` | ContactController | Views/Contact/Index.cshtml | ✅ Built |
| `/Mission` | MissionController | Views/Mission/Index.cshtml | ✅ Built |
| `/Issues` | IssuesController | Views/Issues/Index.cshtml | ✅ Built |
| `/Documents` | DocumentsController | Views/Documents/Index.cshtml | ✅ Built |

### Admin Panel (`/Admin/...`)
| Section | Controller | Views | Status |
|---------|-----------|-------|--------|
| Login | AuthController | Auth/Login.cshtml | ✅ Done |
| Dashboard | DashboardController | Dashboard/Index.cshtml | ✅ Done |
| Articles | ArticlesController | Articles/ (Index, Create, Edit) | ✅ Done |
| Slider | SliderController | Slider/ (Index, Create, Edit) | ✅ Done |
| Events | EventsController | Events/ (Index, Create, Edit) | ✅ Done |
| Opinion | OpinionController | Opinion/ (Index, Create, Edit) | ✅ Done |
| Documents | DocumentsController | Documents/ (Index, Create, Edit) | ✅ Done |
| Gallery | GalleryController | Gallery/Index.cshtml | ✅ Done |
| Volunteers | VolunteersController | Volunteers/Index.cshtml | ✅ Done |
| Speakers | SpeakersController | Speakers/Index.cshtml | ✅ Done |
| Newsletter | NewsletterController | Newsletter/Index.cshtml | ✅ Done |
| Submissions | SubmissionsController | Submissions/Index.cshtml | ✅ Done |
| Users | UsersController | Users/ (Index, Create, ChangePassword) | ✅ Done |
| Settings | SettingsController | Settings/Index.cshtml | ✅ Done |
| Zeffy | ZeffyController | Zeffy/Index.cshtml | ✅ Done |

### Services & Infrastructure
- `RecaptchaService` — Google reCAPTCHA v3 on contact form
- `LocalStorageService` / `IStorageService` — file uploads saved to `wwwroot/uploads/`
- `AppDbContext` — EF Core with MySQL 8.0.36
- Session auth (4-hour idle timeout), no ASP.NET Identity
- Rate limiter: 5 req/10 sec on `"abc"` policy
- `SearchAPIController` — search endpoint (recently added, not yet committed)

---

## Design System (`wwwroot/css/c4justice.css`)
| Variable | Value |
|----------|-------|
| `--c4-dark` | `#0d1117` |
| `--c4-primary` | `#b91c1c` (red) |
| `--c4-gold` | `#d97706` |
| Pillar: Political | Blue |
| Pillar: Racial | Red |
| Pillar: Economic | Gold |
| Pillar: Environmental | Green |

Animations: preloader, AOS scroll reveal, counter animation, card tilt, particle hero, floating shapes, scroll fade.

---

## Voter Registration Module (added 2026-06-18)

| File | Purpose |
|------|---------|
| `Requirement/voters_data.sql` | Run once on MySQL — creates `voters_data` table + protection setting |
| `Models/VoterData.cs` | EF Core model for voter rows |
| `Models/VoterReportViewModel.cs` | ViewModel for public dashboard |
| `Data/AppDbContext.cs` | Added `VoterData` DbSet + table mapping |
| `Areas/Admin/Controllers/VoterUploadController.cs` | Upload, parse, confirm-replace, cancel |
| `Areas/Admin/Views/VoterUpload/Index.cshtml` | Admin upload page with drag-and-drop |
| `Areas/Admin/Views/VoterUpload/ConfirmReplace.cshtml` | Duplicate month warning + confirm |
| `Areas/Admin/Views/Shared/_AdminLayout.cshtml` | Added "Voter Data" sidebar section |
| `Controllers/VotersController.cs` | Public dashboard + CSV export |
| `Views/Voters/Index.cshtml` | Public dashboard (hero, filters, results table) |
| NuGet | ExcelDataReader 3.9.0 + ExcelDataReader.DataSet 3.9.0 |

**Before first use:** run `Requirement/voters_data.sql` on the live MySQL database.

**Dashboard protection:** Admin → Site Settings → set `Voter Dashboard — Require Admin Login to Access` to `true`.

---

## Uncommitted Changes (as of 2026-06-18)

These files have been modified but **not yet committed**:

### Admin Area
- `Areas/Admin/Controllers/AuthController.cs`
- `Areas/Admin/Controllers/GalleryController.cs`
- `Areas/Admin/Controllers/SettingsController.cs`
- `Areas/Admin/Views/Auth/Login.cshtml`
- `Areas/Admin/Views/Gallery/Index.cshtml`
- `Areas/Admin/Views/Newsletter/Index.cshtml`
- `Areas/Admin/Views/Opinion/Index.cshtml`
- `Areas/Admin/Views/Shared/_AdminLayout.cshtml`
- `Areas/Admin/Views/Speakers/Index.cshtml`
- `Areas/Admin/Views/Submissions/Index.cshtml`
- `Areas/Admin/Views/Volunteers/Index.cshtml`

### Public / Core
- `Controllers/ContactController.cs`
- `Controllers/HomeController.cs`
- `Models/ContactSubmission.cs`
- `Program.cs`
- `Services/RecaptchaService.cs`
- `Views/About/Index.cshtml`
- `Views/Home/Index.cshtml`
- `Views/Shared/_Layout.cshtml`
- `appsettings.json`
- `wwwroot/css/c4justice.css`

### Untracked (new files not yet added to git)
- `Admin_Panel_QA.rtf`
- `C4Justice.Web/.idea/` (JetBrains IDE folder)
- `Controllers/SearchAPIController.cs`
- `wwwroot/images/c4TitleLOGO.png`

---

## Recent Git History
```
044cbb2  changes sent to the client.
a39c155  fixing design
40374a5  FIXING Home page DESIGN AND inner pages
020852c  tried to fox some design
8d62ae6  Minor changes
818d963  Add slider edit feature, UI updates, and new homepage slide
ce1b0fa  menu fix
afc1bb1  design and pages update
fa4be19  image fix
6da7b2e  design update
```

---

## Session Log

### 2026-06-18
- Created this PROGRESS.md file to track inter-session state.
- Project is in active development; last client delivery commit was `044cbb2`.
- Large batch of uncommitted changes covering admin panel UI, contact/home controllers, reCAPTCHA service, CSS, and layout.
- `SearchAPIController.cs` added but never committed.
- DB init/seed code in `Program.cs` is commented out (MySQL is live, schema already exists).

---

## What's Next / Open Items
<!-- Update this section at the end of each session -->
- [ ] Commit the current batch of uncommitted changes with a descriptive message
- [ ] Verify `SearchAPIController` is wired up and working
- [ ] QA Admin Panel (see `Admin_Panel_QA.rtf` for checklist)
- [ ] Deploy / push to production server

---

## Environment & Credentials
- **DB:** MySQL 8.0.36 — connection string in `appsettings.json` (not committed to git)
- **reCAPTCHA keys:** in `appsettings.json`
- **Email:** info@cu4justice.com
- **Donate link:** https://link.clover.com/urlshortener/TscDTq
- **Facebook:** https://www.facebook.com/profile.php?id=61550088064673
