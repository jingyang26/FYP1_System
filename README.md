# FYP1 System - Final Year Project Management System

## Introduction
This web application manages the Final Year Project (FYP1) process for academic institutions, supporting Admin, Committee, Student, Supervisor, and Evaluator workflows with role-based access control.

## Technologies Used
- **Backend**: ASP.NET Core 9 with Razor Pages
- **Database**: Entity Framework Core with SQL Server/SQLite
- **Authentication**: ASP.NET Identity with role-based authorization
- **Frontend**: Bootstrap 5 for responsive UI, Font Awesome for icons
- **Development**: Visual Studio Code, .NET 9 SDK

## Features
### Role-Based Dashboards
- **Admin**: Manage programs, lecturers, and system settings
- **Committee**: Assign domains to lecturers, manage students, proposals, and assign evaluators
- **Student**: Register, select supervisor, submit/view proposals, see feedback, resubmit
- **Supervisor**: View students and their proposals, comment, see evaluation results
- **Evaluator**: Download assigned proposals, evaluate, provide status and comments

### Core Functionality
1. Role-based authentication and authorization
2. Proposal submission and evaluation workflow
3. File upload/download functionality for proposals
4. Comment system for feedback
5. Comprehensive dashboards for each user role
6. Real-time status tracking

## Database Design
### Main Entities
- **Users** (ApplicationUser): Extended Identity user with role assignments
- **Programs**: Academic programs (Software Engineering, Data Engineering)
- **Lecturers**: Faculty members with domain expertise
- **Students**: Students enrolled in programs
- **Proposals**: FYP1 project proposals with status tracking
- **Evaluators**: Lecturer assignments for proposal evaluation
- **Comments**: Feedback system for proposals

## Installation & Setup

### Prerequisites
- .NET 9 SDK
- Visual Studio Code (recommended)
- SQL Server (or SQLite for development)

### Setup Steps

1. **Clone/Download the project**
   ```bash
   # Extract to your desired directory
   cd "your-project-directory"
   ```

2. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

3. **Update Database**
   ```bash
   dotnet ef database update
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

5. **Access the application**
   - Open browser and navigate to `https://localhost:5001` or `http://localhost:5000`

### Configuration

#### Database Connection
- **Development**: Uses SQLite by default (`app.db`)
- **Production**: Configure SQL Server connection in `appsettings.json`:
  ```json
  {
    "ConnectionStrings": {
      "SqlServerConnection": "Server=(localdb)\\mssqllocaldb;Database=FYP1SystemDb;Trusted_Connection=true"
    }
  }
  ```

#### File Upload Settings
- Maximum file size: 10MB
- Allowed extensions: `.pdf`, `.doc`, `.docx`
- Upload path: `wwwroot/uploads`

## Default User Credentials

### Test Accounts
| Role | Email | Password | Description |
|------|-------|----------|-------------|
| Admin | admin@fyp1.edu.my | Admin@123 | System Administrator |
| Committee | committee@fyp1.edu.my | Committee@123 | Committee Member (Dr. John Smith) |
| Student | student@fyp1.edu.my | Student@123 | Student (Ahmad Ali) |

### Role Permissions
- **Admin**: Full system access, user management, program management
- **Committee**: Student management, proposal review, evaluator assignment
- **Student**: Proposal submission, supervisor selection, view feedback
- **Supervisor**: View assigned students, provide guidance, view evaluations
- **Evaluator**: Evaluate assigned proposals, provide scores and feedback

## Development Guidelines

### Code Style
- Use async/await for database operations
- Implement proper error handling and validation
- Follow ASP.NET Core naming conventions
- Use dependency injection for services
- Implement proper security measures for file uploads
- Use ViewModels for complex views

### Project Structure
```
FYP1System/
├── Areas/Identity/          # Identity scaffolded pages
├── Data/                    # DbContext, migrations, seed data
├── Models/                  # Entity models
├── Pages/                   # Razor Pages
│   ├── Admin/              # Admin-specific pages
│   ├── Committee/          # Committee-specific pages
│   ├── Student/            # Student-specific pages
│   ├── Supervisor/         # Supervisor-specific pages
│   ├── Evaluator/          # Evaluator-specific pages
│   └── Shared/             # Shared layouts and partials
├── wwwroot/                # Static files
│   ├── css/               # Stylesheets
│   ├── js/                # JavaScript files
│   ├── lib/               # Client libraries
│   └── uploads/           # Uploaded files
└── Properties/             # Launch settings
```

## Additional Features to Implement

### Phase 2 Enhancements
1. **Email Notifications**
   - Proposal submission confirmations
   - Evaluation status updates
   - Assignment notifications

2. **Advanced Reporting**
   - Proposal analytics
   - Performance metrics
   - Export functionality

3. **File Management**
   - Version control for proposals
   - Bulk file operations
   - File preview capabilities

4. **Communication System**
   - Internal messaging
   - Discussion forums
   - Announcement system

## Troubleshooting

### Common Issues
1. **Database Connection Errors**
   - Verify connection string in `appsettings.json`
   - Ensure SQL Server is running (if using SQL Server)
   - Run `dotnet ef database update` to apply migrations

2. **Login Issues**
   - Use the default credentials provided above
   - Check if Identity is properly configured
   - Verify user roles are seeded correctly

3. **File Upload Issues**
   - Check file size limits in configuration
   - Verify upload directory exists and has write permissions
   - Ensure allowed file extensions are configured

### Support
For technical support or questions:
- Check the application logs in the console output
- Review Entity Framework migration files
- Verify user roles and permissions in the database

## License
This project is developed for educational purposes as part of the SECP3106 Individual Project coursework.

---

**Last Updated**: December 2024  
**Version**: 1.0.0  
**Framework**: ASP.NET Core 9
