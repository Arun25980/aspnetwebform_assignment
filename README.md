# Issue Tracker Web Application (ASP.NET Web Forms)

A modernized **ASP.NET Web Forms** application built with an N-Tier architecture, utilizing **Entity Framework**, **Repository & Unit of Work Patterns**, **Unity.WebForms Dependency Injection**, and a responsive **Bootstrap 5 UI**.

---

## 🏗️ Project Architecture

The solution is divided into three main projects:

1. **`IssueTracker.Core`**:
   - Contains domain entities (`Issue`, `ArchiveIssue`, etc.).
   - Contains repository and Unit of Work interfaces (`IRepository<T>`, `IUnitOfWork`).

2. **`IssueTracker.Data`**:
   - Manages dual database contexts (`PrimaryIssueEntities` and `ArchiveIssueEntities`).
   - Implements EDMX-backed data persistence using the Generic Repository and Unit of Work patterns.

3. **`IssueTracker.Web`**:
   - Presentation layer containing primary tracker (`Default.aspx`) and archive tracker (`Archive.aspx`).
   - Styled with Bootstrap 5.
   - Configured with `Unity.WebForms` for named repository dependency injection into ASP.NET pages.

---

## ✨ Key Features & Recent Updates

- **Cross-Database Delete-to-Archive Migration**: Deleting an issue from the active tracker (`Default.aspx`) automatically copies the issue to a dedicated archive database (`ArchiveIssues.mdf`) before soft-deleting it from the primary database (`PrimaryIssues.mdf`).
- **Archive Management (`Archive.aspx`)**: A dedicated page to search, filter, and view archived records mapped to `ArchiveIssue` entity properties (`OriginalIssueId`, `ArchivedDate`, `Title`, `Priority`, `Status`).
- **Dependency Injection**: Named Unity container registrations in `UnityWebFormsStart.cs` (`ArchiveContext` and `ArchiveRepository`) to cleanly manage dual `DbContext` lifetimes.
- **Exception Handling & Logging**: Integrated error logging across page command handlers and database operations for cross-database transaction reliability.
- **Modern UI**: Clean, mobile-friendly interface built with Bootstrap 5.

---

## 🛠️ Tech Stack & Dependencies

- **Framework**: .NET Framework 4.8 / ASP.NET Web Forms
- **ORM**: Entity Framework 6.x (Code First & EDMX)
- **Database**: SQL Server LocalDB (`App_Data/*.mdf`)
- **IoC Container**: Unity (`Unity.WebForms`)
- **UI Framework**: Bootstrap 5

---

## 📁 Repository & Git Rules

- A `.gitignore` file is configured at the root level to exclude build artifacts (`bin/`, `obj/`), SQL LocalDB binary files (`.mdf`, `.ldf`), and local user settings (`.vs/`). This prevents file-locking issues during commits when LocalDB or IIS Express processes are active.

---

## 🚀 Getting Started

### Prerequisites
- Visual Studio 2019 or later with **ASP.NET and web development** workload.
- SQL Server Express / LocalDB instance installed.

### Setup & Execution
1. **Clone the repository**:
   ```bash
   git clone [https://github.com/kamdev1976/aspnetwebform_assignment.git](https://github.com/kamdev1976/aspnetwebform_assignment.git)
