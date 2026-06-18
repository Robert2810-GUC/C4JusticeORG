-- ============================================================
--  Voter Registration Reporting Tool — Database Setup
--  Run this once against the existing cu4justice database
-- ============================================================

-- 1. Main voters data table
CREATE TABLE IF NOT EXISTS `voters_data` (
    `Id`          INT           NOT NULL AUTO_INCREMENT,
    `AsOfDate`    DATE          NOT NULL,
    `County`      VARCHAR(100)  NOT NULL,
    `VoterStatus` VARCHAR(20)   NOT NULL,
    `AgeGroup`    VARCHAR(20)   NOT NULL,
    `Gender`      VARCHAR(20)   NOT NULL,
    `Race`        VARCHAR(100)  NOT NULL,
    `VoterCount`  INT           NOT NULL DEFAULT 0,
    PRIMARY KEY (`Id`),
    INDEX `idx_asofdate`    (`AsOfDate`),
    INDEX `idx_county`      (`County`),
    INDEX `idx_status`      (`VoterStatus`),
    INDEX `idx_date_county` (`AsOfDate`, `County`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 2. Site setting: allow admin to toggle dashboard protection
INSERT IGNORE INTO `sitesettings` (`Key`, `Value`, `Label`, `Group`)
VALUES ('voters_dashboard_protected', 'false', 'Voter Dashboard — Require Admin Login to Access (true / false)', 'Voters');
