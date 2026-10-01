# Issue Tracker Web Application (ASP.NET Web Forms)

A modernized **ASP.NET Web Forms** application built with an N-Tier architecture, utilizing **Entity Framework Code First**, **Repository & Unit of Work Patterns**, **Unity.WebForms Dependency Injection**, and a responsive **Bootstrap 5 UI**.

---

## 🏗️ Project Architecture

The solution is divided into three main projects:

1. **`IssueTracker.Core`**:
   - Contains domain entities (`Issue`, etc.).
   - Contains repository and Unit of Work interfaces (`IRepository<T>`, `IUnitOfWork`).

2. **`IssueTracker.Data`**:
   - Manages database contexts (`PrimaryIssueEntities` and `ArchiveIssuesEntities`).
   - Implements data persistence using the Generic Repository and Unit of Work patterns.

3. **`IssueTracker.Web`**:
   - Presentation layer containing Web Forms (`Default.aspx`).
   - Styled with Bootstrap 5.
   - Configured with `Unity.WebForms` for property dependency injection into ASP.NET pages.

---

## ✨ Key Features

- **CRUD Operations**: Full Create, Read, Update, and Soft Delete functionality for tracking issues.
- **Dependency Injection**: Injected repositories via `Unity.WebForms` with per-HTTP-request lifetime management.
- **Multi-Database Support**: Configured for active data (`PrimaryIssues.mdf`) and dedicated archive storage (`ArchiveIssues.mdf`).
- **Search & Filtering**: Real-time filtering across Title, Priority, and Assigned Person fields.
- **Modern UI**: Clean, mobile-friendly interface built with Bootstrap 5.

---

## 🛠️ Tech Stack & Dependencies

- **Framework**: .NET Framework 4.8 / ASP.NET Web Forms
- **ORM**: Entity Framework 6.x
- **Database**: SQL Server LocalDB (`App_Data/*.mdf`)
- **IoC Container**: Unity (`Unity.WebForms`)
- **UI Framework**: Bootstrap 5

---

## 🚀 Getting Started

### Prerequisites
- Visual Studio 2019 or later with **ASP.NET and web development** workload.
- SQL Server Express / LocalDB instance installed.

### Setup & Execution
1. **Clone the repository**:
   ```bash
   git clone [https://github.com/kamdev1976/aspnetwebform_assignment.git](https://github.com/kamdev1976/aspnetwebform_assignment.git)
