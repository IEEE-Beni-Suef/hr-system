# HR System

A comprehensive **Human Resources Management System** built with ASP.NET Core, designed to streamline employee management, authentication, and organizational workflows with role-based access control.

---

## 📋 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Technology Stack](#technology-stack)
- [Project Structure](#project-structure)
- [Prerequisites](#prerequisites)
- [Installation & Setup](#installation--setup)
- [Configuration](#configuration)
- [Authentication & Authorization](#authentication--authorization)
- [API Endpoints](#api-endpoints)
- [Database](#database)
- [Contributing](#contributing)
- [License](#license)

---

## Overview

The **HR System** is a modern, scalable REST API built with **ASP.NET Core** that provides complete human resources management capabilities. The system includes user authentication via JWT, role-based authorization, email notifications, and a well-architected codebase following enterprise design patterns.

This project was developed by the **IEEE Beni Suef** organization and serves as a comprehensive solution for organizations looking to digitize their HR operations.

---

## ✨ Features

### Core HR Management
- **Employee Management**: Create, read, update, and manage employee records
- **Department & Organization Structure**: Organize employees by departments and hierarchy
- **Role-Based Access Control**: Multiple user roles with specific permissions
  - High Board (Admin)
  - Department Head
  - Team Member
  - HR Personnel
  - Vice Directors
- **User Status Management**: Track active and inactive users

### Security & Authentication
- **JWT Authentication**: Secure token-based authentication
- **Identity Management**: Built on ASP.NET Core Identity
- **Authorization Policies**: Fine-grained access control based on roles and claims
- **Secure Password Handling**: Industry-standard password hashing and validation

### Communication & Notifications
- **Email Service Integration**: Send emails for notifications and communications
- **Configurable Email Settings**: Easily configure SMTP and email templates

### API Features
- **RESTful API Design**: Clean, standard REST API architecture
- **Swagger/OpenAPI Documentation**: Auto-generated API documentation
- **JSON Converters**: Custom date/time and enum serialization
- **CORS Support**: Cross-origin requests for frontend integration
- **Exception Handling Middleware**: Centralized error handling

---

## 🛠️ Technology Stack

| Technology | Purpose |
|-----------|---------|
| **ASP.NET Core** | Web framework |
| **C#** | Programming language |
| **Entity Framework Core** | ORM for database operations |
| **SQL Server** | Relational database |
| **JWT (JSON Web Tokens)** | Authentication |
| **ASP.NET Core Identity** | User and role management |
| **Swagger/Swashbuckle** | API documentation |
| **SMTP** | Email service |

---

## 📁 Project Structure

```
IEEE-Beni-Suef/hr-system/
├── Controllers/              # API controllers handling HTTP requests
├── Services/                 # Business logic and services
│   ├── Auth/                # Authentication services
│   ├── Email/               # Email notification services
│   └── EmailSettings/       # Email configuration
├── Entities/                # Database entity models
├── DTO/                     # Data Transfer Objects
├── Data/                    # Database context and configurations
├── Middleware/              # Custom middleware (exception handling)
├── JsonConverters/          # Custom JSON serialization converters
├── Configurations/          # Application configurations
├── Utility/                 # Utility and helper functions
├── Migrations/              # Database migrations
├── Properties/              # Project properties
├── wwwroot/                 # Static files
├── Program.cs               # Application entry point
├── appsettings.json         # Configuration file
└── IEEE.csproj              # Project file
```

### Directory Descriptions

- **Controllers**: Contains API endpoint definitions for all HR operations
- **Services**: Business logic implementations for authentication, email, and core HR operations
- **Entities**: Entity Framework Core models representing database tables
- **DTO**: Data Transfer Objects for API request/response contracts
- **Data**: DbContext and database-related configurations
- **Middleware**: Custom middleware for exception handling and request/response processing
- **JsonConverters**: Custom JSON serializers for handling dates and enums
- **Migrations**: EF Core migrations for database schema versioning

---

## 📋 Prerequisites

- **.NET 6.0 or higher**
- **SQL Server** (local or remote)
- **Visual Studio 2022** or Visual Studio Code
- **.NET CLI** (for command-line operations)

---

## 🚀 Installation & Setup

### 1. Clone the Repository

```bash
git clone https://github.com/IEEE-Beni-Suef/hr-system.git
cd hr-system
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Update Database Connection

Edit `appsettings.json` and update the connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=HRSystemDB;Trusted_Connection=true;"
  }
}
```

### 4. Apply Database Migrations

```bash
dotnet ef database update
```

### 5. Run the Application

```bash
dotnet run
```

The API will be available at `https://localhost:5001` (HTTPS) or `http://localhost:5000` (HTTP).

### 6. Access API Documentation

Navigate to `https://localhost:5001/swagger` to view the interactive API documentation.

---

## ⚙️ Configuration

### appsettings.json

The application configuration includes:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=HRSystemDB;Trusted_Connection=true;"
  },
  "Jwt": {
    "SecritKey": "YOUR_SECRET_KEY_MIN_32_CHARS",
    "IssuerIP": "https://your-api-domain.com",
    "AudienceIP": "https://your-frontend-domain.com"
  },
  "EmailConfiguration": {
    "From": "your-email@example.com",
    "SmtpServer": "smtp.gmail.com",
    "Port": 587,
    "Username": "your-email@gmail.com",
    "Password": "your-app-password"
  }
}
```

### JWT Configuration

The system uses JWT for stateless authentication. Tokens include:
- User identity
- Role information
- Custom claims
- Token expiration

### Email Configuration

Configure your SMTP settings for email notifications:
- **SMTP Server**: Gmail, SendGrid, or your custom server
- **Port**: Typically 587 (TLS) or 465 (SSL)
- **Authentication**: Username and password or API key

---

## 🔐 Authentication & Authorization

### Authentication Flow

1. User submits credentials (username/password)
2. System validates against ASP.NET Core Identity
3. JWT token is issued with user claims and roles
4. Client stores token and includes it in Authorization header
5. API validates token and processes request

### Authorization Policies

The system defines role-based policies:

| Policy | Role ID | Description |
|--------|---------|-------------|
| HighBoardOnly | 1 | High Board / Admin access |
| HeadOnly | 2 | Department Head access |
| MemberOnly | 3 | Team Member access |
| HROnly | 4 | HR Personnel access |
| ViceOnly | 5 | Vice Director access |
| ActiveUserOnly | - | Active user status required |

### Using Authorization

```csharp
[Authorize(Policy = "HROnly")]
public IActionResult GetEmployees()
{
    // Only HR personnel can access this endpoint
}
```

---

## 📡 API Endpoints

### Authentication
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - User login
- `POST /api/auth/refresh` - Refresh JWT token
- `POST /api/auth/logout` - User logout

### Employees
- `GET /api/employees` - List all employees
- `GET /api/employees/{id}` - Get employee by ID
- `POST /api/employees` - Create new employee
- `PUT /api/employees/{id}` - Update employee
- `DELETE /api/employees/{id}` - Delete employee

### Departments
- `GET /api/departments` - List all departments
- `POST /api/departments` - Create department
- `PUT /api/departments/{id}` - Update department
- `DELETE /api/departments/{id}` - Delete department

### Email (HR Only)
- `POST /api/email/send` - Send notification email
- `POST /api/email/bulk` - Send bulk emails

> **Note**: For complete endpoint documentation, refer to the Swagger UI at `/swagger`

---

## 🗄️ Database

### Database Provider

The system uses **SQL Server** with Entity Framework Core for data access.

### Migrations

Create and apply migrations:

```bash
# Create a new migration
dotnet ef migrations add MigrationName

# Apply migrations
dotnet ef database update

# Remove last migration
dotnet ef migrations remove
```

### Database Initialization

On first run, ensure database is created:

```bash
dotnet ef database update
```

---

## 🤝 Contributing

Contributions are welcome! Please follow these guidelines:

1. **Fork** the repository
2. **Create** a feature branch (`git checkout -b feature/amazing-feature`)
3. **Commit** changes (`git commit -m 'Add amazing feature'`)
4. **Push** to branch (`git push origin feature/amazing-feature`)
5. **Open** a Pull Request

### Contribution Standards
- Follow C# coding conventions
- Add unit tests for new features
- Update documentation
- Ensure all tests pass

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 📞 Support & Contact

For questions, issues, or suggestions:

- **GitHub Issues**: [Open an issue](https://github.com/IEEE-Beni-Suef/hr-system/issues)
- **Organization**: IEEE Beni Suef
- **Repository**: [IEEE-Beni-Suef/hr-system](https://github.com/IEEE-Beni-Suef/hr-system)

---

## 🎯 Roadmap

- [ ] Advanced reporting and analytics
- [ ] Leave management system
- [ ] Performance reviews module
- [ ] Mobile application (React Native/Flutter)
- [ ] Multi-language support
- [ ] Audit logging
- [ ] Two-factor authentication (2FA)
- [ ] Integration with payment systems

---

**Made with ❤️ by IEEE Beni Suef**
