# ?? User Management & Authentication System

A **production-ready ASP.NET Core Web API** for managing users, roles, authentication, and permissions using **Clean Architecture** and **JWT-based security**.

---

## ?? Project Overview

This project is a **multi-tenant User Management System** designed with industry best practices:

* ?? Secure authentication using JWT
* ?? Role-based and permission-based authorization
* ?? Clean Architecture (Domain, Application, Infrastructure, API)
* ??? Entity Framework Core with SQL Server
* ?? Soft Delete with Audit Logging
* ? Rate Limiting + Global Exception Handling
* ?? Swagger API documentation

---

## ? Features

### ?? Authentication & Security

* JWT Access Token & Refresh Token
* Secure password hashing
* Token validation with custom claims

### ?? User Management

* Create, update, delete users
* Multi-tenant support
* Pagination support

### ??? Authorization

* Role-based access (Admin, User)
* Permission-based policies
* Custom authorization handlers

### ?? Soft Delete & Audit

* Users are not deleted permanently
* `IsDeleted`, `DeletedAt`, `DeletedBy`
* Global query filters (auto exclude deleted data)

### ?? System Features

* Rate limiting (API protection)
* Global exception middleware
* Logging with Serilog
* Health check endpoint

---

## ??? Architecture

```bash
User Management System
??? UserManagement.Api              # Controllers, Middleware
??? UserManagement.Application      # Business Logic (CQRS, MediatR)
??? UserManagement.Domain           # Entities & Interfaces
??? UserManagement.Infrastructure   # EF Core, Security, Persistence
```

---

## ??? Tech Stack

* **Backend:** ASP.NET Core Web API (.NET 8)
* **Database:** SQL Server
* **ORM:** Entity Framework Core
* **Authentication:** JWT
* **Architecture:** Clean Architecture + CQRS

### Libraries:

* MediatR
* FluentValidation
* Serilog
* ASP.NET Rate Limiting

---

## ?? How to Run

### 1. Clone Repository

```bash
git clone https://github.com/your-username/user-management-api.git
```

### 2. Update Database Connection

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "your-sql-server-connection"
  }
}
```

### 3. Run Migration

```bash
dotnet ef database update
```

### 4. Run Project

```bash
dotnet run
```

### 5. Open Swagger

```
http://localhost:xxxx/swagger
```

---

## ?? Default Admin Credentials

* Email: [admin@gmail.com](mailto:admin@gmail.com)
* Password: Admin@123

---

## ?? API Endpoints

### ?? Auth

* `POST /api/auth/register`
* `POST /api/auth/login`
* `POST /api/auth/refresh`
* `POST /api/auth/logout`

### ?? Users

* `GET /api/users`
* `GET /api/users/{id}`
* `POST /api/users`
* `PUT /api/users/{id}`
* `DELETE /api/users/{id}` *(Soft Delete)*

---

## ?? Key Highlights (For Recruiters)

? Implemented Clean Architecture + CQRS
? Designed multi-tenant system
? Built custom authorization policies
? Implemented soft delete with global filters
? Secured API with JWT + Refresh Tokens
? Added rate limiting & logging (Serilog)
? Production-ready structure

---

## ?? Future Improvements

* Add Audit Log Table (history tracking)
* Add Restore (Undo delete)
* Docker support
* CI/CD pipeline
* Deployment on Azure / AWS

---

## ????? Author

**Rushikesh Jagdale**

---

## ? If you like this project

Give it a ? on GitHub!
