# EventPlanner

EventPlanner is an ASP.NET Core MVC web application for creating, managing, and exploring events.
The project was developed as part of the **SoftUni ASP.NET Fundamentals** course.

---

## 📌 Project Description

EventPlanner allows registered users to create events with categories and locations,
manage their own events, browse public upcoming events, and interact with events through comments,
ratings, and event participation.
The application includes authentication, authorization, account management, administration features,
and custom error pages.

---

## ✨ Current Features

- User registration and login
- Separate username and email
- Account management:
  - Change username
  - Change email
  - Change password
- Create, edit, and delete events
- Public and private events
- View personal events (**My Events**)
- Join and leave events
- Event comments
- Event rating system
- Event categories management
- Event locations management
- Authorization:
  - Only creators can edit/delete their events
- Administrator panel:
  - View users
  - Suspend / unsuspend users
  - Manage events, categories, and locations
- Custom error pages:
  - 400 Bad Request
  - 404 Not Found
  - 500 Internal Server Error
- Unit tests for key application logic
- Responsive UI with Bootstrap

---

## 🛠 Technologies Used

- ASP.NET Core MVC (.NET 8)
- Entity Framework Core
- ASP.NET Core Identity
- SQL Server
- Bootstrap
- HTML / CSS / Razor
- xUnit

---

## 🗄 Database

The application uses **SQL Server** with Entity Framework Core.
Authentication and authorization are handled using **ASP.NET Core Identity**.

---

## ▶ How to Run the Project

### 1️⃣ Configure the database connection
Update the connection string in `appsettings.json`:

Example:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost,1433;Database=EventPlannerDb;User Id=sa;Password=YourPassword;TrustServerCertificate=True;"
}
```

### 2️⃣ Apply migrations
```bash
dotnet ef database update
```

### 3️⃣ Run the application
```bash
dotnet run
```

### 4️⃣ Run the unit tests
```bash
dotnet test
```

---

## 👤 Access and Roles

The application supports regular users and administrators.
Regular users can manage their own profiles and events, while administrators can manage users and core content.

---

## 🎓 Academic Purpose

This project was created for educational purposes and demonstrates practical work with:
- MVC architecture
- Entity Framework Core
- Identity and authentication
- Validation and error handling
- Unit testing
- Database-driven web development
