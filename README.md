# Online Consultation and Mentoring Application

## Overview

This is an ASP.NET Core web application suite designed for managing online consultations and mentoring sessions. It consists of two main modules:

1. **RESTful Service Module (Web API)** – Exposes endpoints for managing mentors, consultations, and other entities. Includes logging, authentication, and static HTML pages.
2. **MVC Web Application Module** – Admin and user-facing website with CRUD, user registration, login, profile management, and consultation scheduling features.

---

## Features

### RESTful Web API

- Full **CRUD endpoints** for primary and related entities:
  - `Mentor` (primary)
  - `TypeOfWork` (1-to-N)
  - `Area` (M-to-N)
  - `Consultation` (User-M-to-N)
  - `User` (ApplicationUser)
- **Search and paging** supported on primary entity endpoints
- **Logging** for all actions (Create, Read, Update, Delete)
- Log endpoints:
  - `GET /api/logs/get/{n}` – last N logs
  - `GET /api/logs/count` – total number of logs
- **JWT Authentication**:
  - `POST /api/auth/register`
  - `POST /api/auth/login`
  - `POST /api/auth/changepassword`
- **Static HTML Pages**:
  - Login page with JWT token handling
  - Logs page using localStorage token, dropdown to view last 10/25/50 logs
- **Swagger UI** enabled for interactive API documentation and testing

### MVC Web Application

#### Admin Features (Minimum)

- **Secure login** with role-based access
- CRUD pages for:
  - Mentors
  - TypeOfWork
  - Areas
- List pages with:
  - Search textbox
  - Dropdown filtering
  - Pagination (previous/next)
- Navigation bar with links to all major sections
- **AJAX Profile Page** to update admin details

#### User Features (Desired)

- **Landing Page** with CTA to login or register
- **Self-registration** with validation
- **Login redirection** based on role
- **Consultation listing page**:
  - Search/filter consultations
  - Paginated navigation
- **Consultation details page**:
  - View full info
  - Reserve (apply) for a session
- **User Profile Page** with AJAX editing
- **Administrator dashboard** showing user-consultation associations

---

## API Endpoints

Examples:

- `GET /api/mentor`
- `POST /api/mentor`
- `GET /api/mentor/search?page=1&count=10&name=John`
- `GET /api/logs/get/25`
- `POST /api/auth/login`

---

## Installation & Setup

1. **Clone the Repository**

   ```bash
   git clone https://github.com/k41-s/DWA-OnlineConsultationApp.git
   cd OnlineConsultationApp

2. **Open the solution**

   - Open `OnlineConsultationApp.sln` in Visual Studio.
   - Restore NuGet packages if prompted.

3. **Configure the database**

   - Ensure SQL Server is installed and running.
   - Open the file `Database/Database.sql`.
   - Execute the SQL script in your database management tool (e.g., SSMS) to create tables and seed initial data.

4. **Run the application**

   - Set both `WebAPI` and `WebApp` projects as startup projects in Visual Studio.
   - Build the solution.
   - Run the application.

5. **Configure connection strings**

   - Open `appsettings.json` in both projects.
   - Update the connection string to point to your SQL Server instance.
   - Do **not** hardcode connection strings in code; always use configuration files.

---

## Error Handling

- Returns appropriate HTTP status codes such as:
  - `400 Bad Request`
  - `404 Not Found`
  - `500 Internal Server Error`
- Logs all CRUD operations and exceptions with meaningful messages.
- Gracefully handles deletion and update conflicts related to foreign keys.
- Uses data annotations for input validation and prevents invalid data entry.

---

## Technologies Used

- .NET 8.0 (or the version specified in your workshops)
- ASP.NET Core Web API
- ASP.NET Core MVC
- Entity Framework Core (Database-First approach)
- SQL Server
- AutoMapper
- Swagger / Swashbuckle for API documentation
- JavaScript, HTML, CSS
- Bootstrap (optional, for styling)
- AJAX (for profile page updates and paging)

