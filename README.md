# Mudraa - Integrated Sales & Inventory Management System

![.NET](https://img.shields.io/badge/.NET%208.0-blueviolet?logo=dotnet)
![Entity Framework Core](https://img.shields.io/badge/Entity%20Framework-Core%208.0-green)
![SQL Server](https://img.shields.io/badge/Database-SQL%20Server-red)
![MVC](https://img.shields.io/badge/Frontend-ASP.NET%20MVC-blue)

A full-stack **Sales & Inventory Management System** built with **.NET Core Web API**, **Entity Framework Core**, **SQL Server**, and **ASP.NET MVC**.  
Mudraa simplifies product tracking, order processing, and analytics through a clean dashboard with charts and reports.

---

## Features

- **JWT Authentication** with role-based access  
- **Secure password hashing** using BCrypt  
- **CRUD operations** for Products, Categories, Orders, and Inventory  
- **Sales tracking & analytics** using Chart.js  
- **Dashboard insights** for products, earnings, and tasks  
- **Image upload support** for products  
- **Global Exception Handler Middleware**  
- **Toast message notifications** for user feedback  
- **Session storage** and cookie-based JWT tokens  
- **Structured logging & error handling**

---

## Tech Stack

**Backend:** ASP.NET Core 8 Web API  
**Frontend:** ASP.NET Core MVC  
**Database:** SQL Server  
**ORM:** Entity Framework Core  
**Authentication:** JWT + Cookies + Roles  
**UI Framework:** Bootstrap 5 + Chart.js  

---

## Project Setup

```bash
# Install dependencies
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package BCrypt.Net-Next
````

---

## Database Configuration

1. Update your **connection string** in `appsettings.json`:

   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=YOUR_SERVER;Database=MudraaDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;"
   }
   ```
2. Run EF Core migrations:

   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

---

## Project Structure

```
Mudraa/
├── Mudraa.API/          # .NET Core Web API project
│   ├── Controllers/
│   ├── Services/
│   ├── Repositories/
│   ├── DTOs/
│   └── Middleware/
├── Mudraa.MVC/          # MVC Frontend (consumes the API)
│   ├── Controllers/
│   ├── Views/
│   ├── wwwroot/
│   └── Services/
└── README.md
```

---

## Dashboard Preview

<img width="1366" height="679" alt="image" src="https://github.com/user-attachments/assets/8f3e6085-e542-4fe5-b706-705c3dc592c2" />

---

## Deployment Ready

* Works on **Azure App Service** and **GCP Cloud Run**
* Dockerfile supported
* Environment variable-based secrets

---

## Author

**Prashant Devkate**
.NET Core | Web API | EF Core | SQL Server | Azure | GCP
[[meetprashant1234@gmail.com](mailto:meetprashant1234@gmail.com)]
