# 📚 MVC Web App Project - Library Management System

## 🔵 Project Overview

This project is a **Library Management System** developed using **ASP.NET Core MVC**.
It allows libraries to manage books, handle borrowing and returns, track fines,
manage member accounts and moderate reviews through a clean web interface.

The project demonstrates practical implementation of **MVC architecture**,
**role-based authentication**, **database management** and **responsive UI design**
in a real-world scenario.

---

## 🚀 Features

- User registration and login (ASP.NET Core Identity)
- Role-based access — Admin and Member
- Add, edit, delete and view books with cover images
- Search books by title, author or ISBN
- Filter books by category
- Borrow and return books online
- Reserve books when unavailable
- Renew borrowed books
- Automatic overdue fine calculation
- Member borrow history tracking
- Star ratings and review system with admin approval
- Admin dashboard with live statistics
- Manage borrowing rules and penalties
- Library profile management
- Reports and analytics
- Responsive design — works on mobile and desktop
- Book cover images via Open Library API

---

## 🛠️ Technologies Used

| Technology | Purpose |
|-----------|---------|
| ASP.NET Core MVC (.NET 9) | Web framework |
| Entity Framework Core 9 | Database ORM — Code First |
| SQL Server LocalDB | Database |
| ASP.NET Core Identity | Authentication and role management |
| Bootstrap 5 | Responsive UI layout |
| Bootstrap Icons | Icons throughout the site |
| jQuery Validation | Client-side form validation |
| Open Library API | Free book cover images by ISBN |

---

## 👥 Development Team

| Name | Student ID | Role |
|------|-----------|------|
| Rakib Mahmud | 20029869  Lead Developer 
| Md Mehedi Hassan Talukder 20032100 
| Md Salaman Prodhan  20030404 

**Unit:** ICT272 — Web Design and Development
**Trimester:** T1 2026

---

## 🔐 Default Login Credentials

| Role | Email | Password |
|------|-------|----------|
| Admin | admin@library.com | Admin@123 |
| Member | member@library.com | Member@123 |

---

## ⚙️ How to Run

**Step 1** — Clone the repository:
````bash
git clone https://github.com/your-username/LibraryManagementSystem2.0-ICT272.git
````

**Step 2** — Open the solution in Visual Studio 2022

**Step 3** — Check `appsettings.json` has the correct connection string:

````json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=LibraryManagementDB_V2;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  }
}
````

**Step 4** — Open Package Manager Console and run:

````powershell
Update-Database
````

**Step 5** — Press **F5** to run

The database seeds automatically on first run with:
- Admin and Member accounts
- 18 book categories
- Default borrowing rules
- Library profile
- 70+ sample books with real cover images

---

## 🗄️ Database Tables

| Table | Purpose |
|-------|---------|
| Books | Book catalogue |
| Categories | Book genres |
| BookCategories | Many-to-many join table |
| AspNetUsers | Member and admin accounts |
| BorrowRecords | All borrow transactions |
| Reservations | Book reservations |
| Fines | Overdue fines |
| Feedbacks | Book reviews and ratings |
| BorrowingRules | Loan duration and penalty config |
| LibraryProfiles | Library information |

---

## 📁 Project Structure

````
LibraryManagementSystem2.0-ICT272/
│
├── Controllers/
│   ├── AccountController.cs
│   ├── AdminController.cs
│   ├── BooksController.cs
│   ├── BorrowController.cs
│   ├── BorrowingRulesController.cs
│   ├── CategoriesController.cs
│   ├── FeedbackController.cs
│   ├── FinesController.cs
│   ├── HomeController.cs
│   ├── LibraryProfileController.cs
│   └── ReportsController.cs
│
├── Data/
│   ├── ApplicationDbContext.cs
│   └── SeedData.cs
│
├── Models/
│   ├── ApplicationUser.cs
│   ├── Book.cs
│   ├── BookCategory.cs
│   ├── BorrowingRule.cs
│   ├── BorrowRecord.cs
│   ├── Category.cs
│   ├── ErrorViewModel.cs
│   ├── Feedback.cs
│   ├── Fine.cs
│   ├── LibraryProfile.cs
│   └── Reservation.cs
│
├── ViewModels/
│   ├── BookViewModel.cs
│   ├── LoginViewModel.cs
│   └── RegisterViewModel.cs
│
├── Views/
│   ├── Account/
│   ├── Admin/
│   ├── Books/
│   ├── Borrow/
│   ├── BorrowingRules/
│   ├── Categories/
│   ├── Feedback/
│   ├── Fines/
│   ├── Home/
│   ├── LibraryProfile/
│   ├── Reports/
│   └── Shared/
│       ├── _Layout.cshtml
│       └── Error.cshtml
│
├── wwwroot/
│   ├── css/
│   │   ├── library-theme.css
│   │   └── site.css
│   └── Uploads/
│       └── Covers/
│
├── appsettings.json
└── Program.cs
````

---

## ✅ Requirements

- Visual Studio 2022
- .NET 9 SDK
- SQL Server LocalDB (installed with Visual Studio)
- NuGet packages — restored automatically:
  - Microsoft.AspNetCore.Identity.EntityFrameworkCore
  - Microsoft.EntityFrameworkCore.SqlServer
  - Microsoft.EntityFrameworkCore.Tools

---

## 📌 Architecture

This project follows the **MVC (Model-View-Controller)** pattern:

- **Models** — Define data structure and database schema using EF Core Code First
- **Views** — Razor templates that render HTML using data from controllers
- **Controllers** — Handle HTTP requests, apply business logic and return views

Authentication uses **ASP.NET Core Identity** with two roles:
- `Admin` — Full access to all management features
- `Member` — Access to browse, borrow and review books
