# Issue Tracker Web Application (ASP.NET Web Forms)

A modernized **ASP.NET Web Forms** application built with an N-Tier architecture, utilizing **Entity Framework**, **Repository & Unit of Work Patterns**, **DbContext Factory**, **Unity.WebForms Dependency Injection**, and a responsive **Bootstrap 5 UI**.

---

## 🏗️ Project Architecture

The solution is divided into three main projects:

1. **`IssueTracker.Core`**:
   - Contains domain entities (`Issue`, `ArchiveIssue`, etc.).
   - Contains repository and Unit of Work interfaces (`IRepository<T>`, `IUnitOfWork`).

2. **`IssueTracker.Data`**:
   - Manages dual database contexts (`PrimaryIssueEntities` and `ArchiveIssueEntities`) via `DbContextFactory`.
   - Implements EDMX-backed data persistence using the Generic Repository and Unit of Work patterns[cite: 5].
   - **Centralized Validation & Transaction Control**: `Repository<T>` handles in-memory entity staging[cite: 5], while `UnitOfWork.Complete()` executes atomic cross-context commits and logs `DbEntityValidationException` details[cite: 5].

3. **`IssueTracker.Web`**:
   - Presentation layer containing primary tracker (`Default.aspx`) and archive tracker (`Archive.aspx`).
   - Styled with Bootstrap 5.
   - Configured with `Unity.WebForms` for constructor/property dependency injection of `IUnitOfWork` into ASP.NET pages[cite: 5].

---

## ✨ Key Features & Architectural Enhancements

- **Dynamic DbContext Factory Integration**: Leverages `DbContextFactory` inside `UnitOfWork` to dynamically resolve and route entities to `PrimaryIssueEntities` or `ArchiveIssueEntities` on demand[cite: 5].
- **Atomic Cross-Database Transactions**: Deleting an issue from the active tracker (`Default.aspx`) copies the record to `ArchiveIssues.mdf` and soft-deletes (`IsDeleted = 1`) the primary record inside a single `UnitOfWork.Complete()` call[cite: 5].
- **Archive Management (`Archive.aspx`)**: A dedicated page to search, filter, and view archived records mapped to `ArchiveIssue` entity properties (`OriginalIssueId`, `ArchivedDate`, `Title`, `Priority`, `Status`).
- **Dependency Injection**: `Unity.WebForms` injects `IUnitOfWork` across page lifecycles to maintain clean decoupling[cite: 5].
- **Centralized Exception Logging**: Entity Framework validation errors (`DbEntityValidationException`) are captured and formatted down to the failing entity property name inside `UnitOfWork.Complete()`[cite: 5].
- **Modern UI**: Mobile-friendly, responsive interface built with Bootstrap 5 and ASP.NET `UpdatePanel` controls for flicker-free grid updates.

---

## 🛠️ Tech Stack & Dependencies

- **Framework**: .NET Framework 4.8 / ASP.NET Web Forms
- **ORM**: Entity Framework 6.x (EDMX & Code First)
- **Database**: SQL Server LocalDB (`App_Data/*.mdf`)
- **IoC Container**: Unity (`Unity.WebForms`)
- **UI Framework**: Bootstrap 5

---

## 💻 Local Setup & Execution Guide (Step-by-Step for Code Reviewers)

Follow these steps to set up and run the application on any development machine:

### Prerequisites
- **Visual Studio 2019 or Visual Studio 2022** with the **ASP.NET and web development** workload installed.
- **SQL Server Express LocalDB** installed (included by default with Visual Studio setup as `(LocalDB)\MSSQLLocalDB`).

### Setup Steps
1. **Clone the repository**:
   ```bash
   git clone [https://github.com/kamdev1976/aspnetwebform_assignment.git](https://github.com/kamdev1976/aspnetwebform_assignment.git)
