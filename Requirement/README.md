# Voter Registration Reporting Tool
### voters.cu4justice.org
**Client:** Richard Rose — Communities United for Justice (cu4justice.org)  
**Developer:** Gopal Kumar  
**Project Type:** New ASP.NET module on existing cu4justice.org infrastructure  
**Estimated Hours:** 30 hrs | **Rate:** $15/hr | **Total:** $450 USD  

---

## 📌 Project Overview

Richard downloads a monthly Excel file from the **Georgia Secretary of State website** containing voter registration data (~47,000 rows). He needs a web-based reporting tool where he can filter this data and generate reports instantly — without using Excel.

The tool will be hosted at **voters.cu4justice.org** as a new subdomain on the existing cu4justice.org server.

---

## 🏗️ Infrastructure

| Item | Details |
|---|---|
| **Server** | Same server as cu4justice.org — no new hosting needed |
| **Database** | Same existing cu4justice database — add new tables only |
| **Codebase** | Brand new ASP.NET project deployed to subdomain |
| **Login/Auth** | Reuse existing cu4justice.org admin login system |
| **Subdomain** | Create voters.cu4justice.org in cPanel |

---

## 🗄️ Database

### New Table: `voters_data`

| Column | Type | Notes |
|---|---|---|
| `Id` | INT (PK, Auto Increment) | Primary key |
| `AsOfDate` | DATE | Month of the data (from Excel "As Of Date" column) |
| `County` | VARCHAR(100) | 159 Georgia counties |
| `VoterStatus` | VARCHAR(20) | Active / Inactive |
| `AgeGroup` | VARCHAR(20) | 17 ranges (18-24 up to 100+) |
| `Gender` | VARCHAR(20) | Female / Male / Other |
| `Race` | VARCHAR(100) | 6 categories (see below) |
| `VoterCount` | INT | Number of voters |

### Race Categories (exactly as in state data)
1. Black
2. White
3. Hispanic/Latino
4. Asian/Pacific Islander
5. American Indian or Alaskan Native
6. Other/Unknown

### Age Group Ranges (17 total)
18-24, 25-29, 30-34, 35-40, 40-45, 45-50, 50-55, 55-60, 60-65, 65-70, 70-75, 75-80, 80-85, 85-90, 90-95, 95-100, 100+

> ⚠️ **Important:** Do NOT overwrite data on monthly upload. Always INSERT new records tagged with the new AsOfDate. This allows historical trend comparison.

---

## 🔧 What To Build

### 1. 🌐 Subdomain Setup
- Create `voters.cu4justice.org` subdomain in cPanel
- Point to new ASP.NET project folder on same server
- Connect to existing cu4justice database

---

### 2. 📤 Admin Upload Page (inside existing cu4justice admin panel)

**URL:** cu4justice.org/admin/voter-upload (or similar)  
**Access:** Existing admin login — no new login needed  

**Functionality:**
- Simple page with a file upload button
- Richard uploads the monthly Excel file (.xlsx)
- System reads the Excel file server-side
- Parses all rows from Sheet1 (header is on row 3)
- Inserts records into `voters_data` table tagged with the AsOfDate from the file
- Shows success message with count of records imported
- If same AsOfDate already exists in DB — show warning and ask to confirm before overwriting that month

**Excel File Structure (Sheet1, header row 3):**
| Column | DB Field |
|---|---|
| As Of Date | AsOfDate |
| County | County |
| Voter Status | VoterStatus |
| Age Group | AgeGroup |
| Gender | Gender |
| Race | Race |
| Voter Count | VoterCount |

---

### 3. 📊 Reporting Dashboard (voters.cu4justice.org)

**URL:** voters.cu4justice.org  
**Access:** Can be public OR password protected — TBD pending client confirmation  

**Filters (all optional, 1 to 5 can be applied simultaneously):**

| Filter | Type | Options |
|---|---|---|
| Report Month | Dropdown | All uploaded months (from AsOfDate) |
| County | Multi-select dropdown | All 159 counties + "All" option |
| Voter Status | Dropdown | Active / Inactive / All |
| Gender | Dropdown | Female / Male / Other / All |
| Age Group | Multi-select dropdown | All 17 ranges + "All" option |
| Race | Multi-select dropdown | All 6 categories + "All" option |

**Results Display:**
- Show aggregated voter counts based on selected filters
- Display as a clean summary table on screen
- Show total count at the bottom

**Export Button:**
- "Export to CSV" button below the results table
- Downloads the summarized results shown on screen as a CSV file
- ⚠️ Awaiting Richard's confirmation on export style (summary vs raw rows) — build summary for now

---

### 4. 📅 Historical Data & Trend Tracking
- Each monthly upload is saved separately (tagged by AsOfDate)
- "Report Month" filter lets Richard select and compare any month
- This enables trend analysis (e.g. voter growth in a county between months)
- This is a key feature — do NOT implement overwrite logic

---

## 🔄 How The Full Flow Works

```
Richard downloads Excel from Georgia SOS website
        ↓
Logs into cu4justice.org admin panel
        ↓
Goes to Voter Upload page
        ↓
Uploads Excel file → System parses & saves to DB (tagged with AsOfDate)
        ↓
Richard goes to voters.cu4justice.org
        ↓
Selects filters (County, Status, Gender, Age, Race, Month)
        ↓
Clicks "Generate Report" → System queries DB instantly
        ↓
Results shown on screen as summary table
        ↓
Clicks "Export CSV" → Downloads results
```

---

## ✅ Task Checklist for Developer

### Phase 1 — Setup
- [ ] Create `voters.cu4justice.org` subdomain in cPanel
- [ ] Create new ASP.NET project and deploy to subdomain
- [ ] Create `voters_data` table in existing cu4justice database
- [ ] Connect new project to existing database

### Phase 2 — Admin Upload Page
- [ ] Add upload page to existing cu4justice admin panel
- [ ] Build Excel parser (read Sheet1, header row 3, columns A-G)
- [ ] Insert parsed records into `voters_data` with AsOfDate tag
- [ ] Handle duplicate month warning
- [ ] Show import success/failure message with record count

### Phase 3 — Reporting Dashboard
- [ ] Build reporting UI at voters.cu4justice.org
- [ ] Implement all 5 filter dropdowns (County, Status, Gender, Age Group, Race)
- [ ] Add Report Month filter (populated from distinct AsOfDates in DB)
- [ ] Build query logic for all filter combinations
- [ ] Display aggregated results as summary table
- [ ] Add "Export to CSV" button (exports summary shown on screen)

### Phase 4 — Testing & Handover
- [ ] Test upload with May 2026 Excel file (already available)
- [ ] Test all filter combinations
- [ ] Test CSV export
- [ ] Test on mobile browser
- [ ] Share staging link for client review

---

## ⚠️ Important Notes

1. **Gender has 3 values** — Male, Female, Other. Include all 3. Do not ask client.
2. **Historical data** — Never overwrite. Always retain previous months.
3. **Excel header** — Sheet1, header is on Row 3 (not Row 1).
4. **phpMyAdmin** — Client mentioned this but we are NOT using it. Admin upload page replaces this entirely.
5. **Existing admin login** — Reuse cu4justice.org login for the upload page. No new auth system needed.
6. **CSV export style** — Awaiting Richard's confirmation. Build summarized export for now.

---

## 📁 Sample Data

- **File:** Voters Demographically (May 2026).xlsx
- **Rows:** ~47,777
- **Sheet:** Sheet1
- **Header Row:** Row 3
- **As Of Date in file:** 2026-05-15

---

## 📬 Client Contact

**Richard Rose**  
Communities United for Justice  
rrose@cu4justice.org  
Office: 404-768-3888  
Mobile: 404-931-2602  
