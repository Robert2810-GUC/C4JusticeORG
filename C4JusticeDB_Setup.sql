-- ============================================================
-- C4JusticeDB - SQL Server Setup Script
-- Server: SILVER\SQLEXPRESS  |  User: sa  |  Pass: sa123
-- Run this in SSMS against SILVER\SQLEXPRESS
-- ============================================================

USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = 'C4JusticeDB')
BEGIN
    ALTER DATABASE C4JusticeDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE C4JusticeDB;
END
GO

CREATE DATABASE C4JusticeDB;
GO

USE C4JusticeDB;
GO

-- ============================================================
-- ADMIN USERS
-- ============================================================
CREATE TABLE AdminUsers (
    Id           INT IDENTITY(1,1) PRIMARY KEY,
    Username     NVARCHAR(100)  NOT NULL UNIQUE,
    PasswordHash NVARCHAR(500)  NOT NULL,
    Email        NVARCHAR(200)  NOT NULL,
    CreatedAt    DATETIME2      NOT NULL DEFAULT GETUTCDATE()
);
-- NOTE: Admin user seeded automatically by app on first startup
-- Default credentials:  admin / Admin@123

-- ============================================================
-- SLIDER IMAGES (Homepage hero carousel)
-- ============================================================
CREATE TABLE SliderImages (
    Id        INT IDENTITY(1,1) PRIMARY KEY,
    ImageUrl  NVARCHAR(500) NOT NULL,
    Title     NVARCHAR(300) NULL,
    Subtitle  NVARCHAR(600) NULL,
    IsActive  BIT           NOT NULL DEFAULT 1,
    SortOrder INT           NOT NULL DEFAULT 0,
    CreatedAt DATETIME2     NOT NULL DEFAULT GETUTCDATE()
);

-- ============================================================
-- ARTICLES (Opinion / Essays)
-- ============================================================
CREATE TABLE Articles (
    Id               INT IDENTITY(1,1) PRIMARY KEY,
    Title            NVARCHAR(300)  NOT NULL,
    Slug             NVARCHAR(300)  NOT NULL UNIQUE,
    Content          NVARCHAR(MAX)  NOT NULL,
    Excerpt          NVARCHAR(1000) NULL,
    FeaturedImageUrl NVARCHAR(500)  NULL,
    Category         NVARCHAR(100)  NOT NULL DEFAULT 'General',
    IsPublished      BIT            NOT NULL DEFAULT 0,
    PublishedAt      DATETIME2      NULL,
    CreatedAt        DATETIME2      NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt        DATETIME2      NOT NULL DEFAULT GETUTCDATE()
);

-- ============================================================
-- EVENTS
-- ============================================================
CREATE TABLE Events (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    Title       NVARCHAR(300) NOT NULL,
    Description NVARCHAR(MAX) NOT NULL,
    EventDate   DATETIME2     NOT NULL,
    Location    NVARCHAR(300) NULL,
    ImageUrl    NVARCHAR(500) NULL,
    RegisterUrl NVARCHAR(500) NULL,
    Category    NVARCHAR(100) NULL,
    IsActive    BIT           NOT NULL DEFAULT 1,
    CreatedAt   DATETIME2     NOT NULL DEFAULT GETUTCDATE()
);

-- ============================================================
-- DOCUMENTS
-- ============================================================
CREATE TABLE Documents (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    Title       NVARCHAR(300)  NOT NULL,
    Description NVARCHAR(1000) NULL,
    Category    NVARCHAR(100)  NOT NULL DEFAULT 'General',
    FileUrl     NVARCHAR(500)  NOT NULL,
    FileName    NVARCHAR(300)  NOT NULL,
    FileType    NVARCHAR(50)   NOT NULL,
    FileSize    BIGINT         NULL,
    IsPublished BIT            NOT NULL DEFAULT 1,
    PublishedAt DATETIME2      NULL,
    CreatedAt   DATETIME2      NOT NULL DEFAULT GETUTCDATE()
);

-- ============================================================
-- SEED: SLIDER IMAGES
-- ============================================================
INSERT INTO SliderImages (ImageUrl, Title, Subtitle, IsActive, SortOrder) VALUES
('/images/community-1.png', 'United for Political Justice', 'Fighting gerrymandering, voter suppression, and authoritarianism together.', 1, 1),
('/images/community-2.png', 'Racial Justice for All',       'Dismantling systemic racism and building an equitable society.', 1, 2),
('/images/community-3.png', 'Economic Empowerment',          'Closing wealth gaps and creating opportunity for every community.', 1, 3);

-- ============================================================
-- SEED: EVENTS (future dates)
-- ============================================================
INSERT INTO Events (Title, Description, EventDate, Location, Category, IsActive) VALUES
('Community Town Hall Meeting',
 'Join us for our quarterly town hall to discuss local justice issues, upcoming initiatives, and community updates. All are welcome.',
 DATEADD(day, 30, GETUTCDATE()), 'Community Center, Main Hall', 'Political', 1),

('Voter Registration Drive',
 'Help us register voters in underserved communities. Volunteers and participants welcome.',
 DATEADD(day, 45, GETUTCDATE()), 'Public Library, Downtown', 'Political', 1),

('Healthcare Access Forum',
 'A panel discussion on healthcare disparities and what our community can do to advocate for better access and coverage for all.',
 DATEADD(day, 60, GETUTCDATE()), 'City Hall Auditorium', 'Healthcare', 1),

('Environmental Justice March',
 'March with us against environmental racism — clean air, clean water, and climate equity for all communities.',
 DATEADD(day, 75, GETUTCDATE()), 'City Park, Main Entrance', 'Environmental', 1);

-- ============================================================
-- SEED: ARTICLES
-- ============================================================
INSERT INTO Articles (Title, Slug, Content, Excerpt, Category, IsPublished, PublishedAt) VALUES
('Why Voter Rights Are Under Attack',
 'why-voter-rights-are-under-attack',
 '<p>Across the country, legislation designed to restrict voting access disproportionately impacts communities of color. We must fight back against these systemic efforts to silence our voices.</p><p>From gerrymandering to strict voter ID laws, the assault on democracy continues. Communities United for Justice stands firm in our commitment to protect every citizen''s right to vote.</p>',
 'Across the country, legislation designed to restrict voting access disproportionately impacts communities of color.',
 'Political', 1, DATEADD(day, -5, GETUTCDATE())),

('The Healthcare Crisis in Black Communities',
 'healthcare-crisis-black-communities',
 '<p>Black Americans face significantly higher rates of chronic illness, lower life expectancy, and reduced access to quality healthcare. These disparities are not accidental — they are the result of decades of systemic inequality.</p><p>Communities United for Justice is committed to fighting for universal healthcare access and eliminating racial health disparities at every level.</p>',
 'Black Americans face significantly higher rates of chronic illness and reduced access to quality healthcare.',
 'Healthcare', 1, DATEADD(day, -10, GETUTCDATE())),

('Economic Justice: Closing the Wealth Gap',
 'economic-justice-closing-wealth-gap',
 '<p>The racial wealth gap in America is staggering. The median white family holds nearly 8 times the wealth of the median Black family. Policies like redlining, discriminatory lending, and wage suppression built this gap over generations.</p><p>We fight for equitable access to capital, fair wages, and community investment that lifts all boats.</p>',
 'The racial wealth gap in America is staggering and demands urgent policy action.',
 'Economic', 1, DATEADD(day, -15, GETUTCDATE()));

-- ============================================================
-- DONE
-- ============================================================
PRINT '====================================================';
PRINT ' C4JusticeDB setup complete!';
PRINT ' App will seed admin user on first startup.';
PRINT ' Default login: admin / Admin@123';
PRINT '====================================================';
GO
