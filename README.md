# 🎉 EventPlanner

> A modern ASP.NET Core MVC web application for creating, managing, and exploring events.

---

## 📖 Overview

EventPlanner is a full-featured event management platform developed as part of the **SoftUni ASP.NET Fundamentals** course.

The application allows users to create and manage events, interact with other users, and explore upcoming public events through a clean and intuitive interface.

---

## ✨ Features

### 🔐 Authentication & Account Management
- User registration and login
- Separate username and email
- Change username, email, and password

### 📅 Event Management
- Create, edit, and delete events
- Public and private events
- Event categories and locations
- "My Events" dashboard

### 🤝 User Interaction
- Join / Leave events
- Event ratings system
- Comments section for events

### 🎟 Tickets System
- Track event participation
- Dynamic join/leave updates

### 🛡 Authorization
- Only event creators can edit/delete their events
- Role-based access (Admin panel)

### ⚙ Admin Panel
- Manage categories
- Manage locations
- Full control over system data

### ❗ Error Handling
- Custom **404 Not Found** page
- Custom **500 Server Error** page
- Full validation (client + server)

### 🧪 Testing
- Unit tests for controllers and business logic
- Coverage targeting 65%+ of logic

---

## 🛠 Technologies

- ASP.NET Core MVC (.NET 8)
- Entity Framework Core
- ASP.NET Core Identity
- SQL Server (Docker supported)
- Bootstrap 5
- HTML / CSS

---

## 🗄 Database

The application uses **SQL Server** with Entity Framework Core.

Authentication and authorization are handled via **ASP.NET Core Identity**.

---

## ⚙️ Setup & Run

### 1. Clone the repository

```bash
git clone https://github.com/KerenDoz/Event-Planner.git
cd Event-Planner/EventPlanner