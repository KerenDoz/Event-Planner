# EventPlanner

EventPlanner is an ASP.NET Core MVC web application for creating, managing, and exploring events.
The project was developed as part of the **SoftUni ASP.NET Fundamentals** course.

---

## 📌 Project Description

EventPlanner allows registered users to create events with specific categories and locations,
manage their own events, and browse public upcoming events created by other users.
The application includes authentication, authorization, and user account management.

---

## ✨ Features

- User registration and login
- Separate username and email
- Account management:
  - Change username
  - Change email
  - Change password
- Create, edit, delete events
- Public and private events
- View personal events ("My Events")
- Event categories management
- Event locations management
- Authorization:
  - Only creators can edit/delete their events
- Clean dashboard-style home page
- Responsive UI with Bootstrap

---

## 🛠 Technologies Used

- ASP.NET Core MVC (.NET 8)
- Entity Framework Core
- ASP.NET Core Identity
- SQL Server
- Bootstrap
- HTML / CSS

---

## 🗄 Database

The application uses **SQL Server** with Entity Framework Core.
Authentication and authorization are handled using **ASP.NET Core Identity**.

---

## ▶ How to Run the Project

### 1️⃣ Configure the database connection
Update the connection string in appsettings.json:

Example:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost,1433;Database=EventPlannerDb;User Id=sa;Password=YourPassword;TrustServerCertificate=True;"
}

## 2️⃣ Apply migrations:
dotnet ef database update

## 3️⃣ Run the application:
dotnet run
