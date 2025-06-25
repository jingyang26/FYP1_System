# Copilot Instructions for FYP1 Management System

<!-- Use this file to provide workspace-specific custom instructions to Copilot. For more details, visit https://code.visualstudio.com/docs/copilot/copilot-customization#_use-a-githubcopilotinstructionsmd-file -->

## Project Overview
This is an ASP.NET Core 9 web application for managing Final Year Project (FYP1) processes with role-based access control.

## Technologies Used
- ASP.NET Core 9 with Razor Pages
- Entity Framework Core with SQL Server
- ASP.NET Identity for authentication and authorization
- Bootstrap for responsive UI
- SQL Server/SQLite for database

## User Roles
- **Admin**: Manage programs, lecturers, and committee members
- **Committee**: Assign domains to lecturers, manage students, manage proposals, assign evaluators
- **Student**: Register, select supervisor, submit/view proposals, see feedback, resubmit
- **Supervisor**: View students and their proposals, comment, see evaluation results
- **Evaluator**: Download assigned proposals, evaluate, provide status and comments

## Key Features to Implement
1. Role-based authentication and authorization
2. Proposal submission and evaluation workflow
3. File upload/download functionality for proposals
4. Comment system for feedback
5. Dashboard for each user role
6. Email notifications (optional)

## Database Design
Main entities: Users, Programs, Lecturers, Students, Proposals, Evaluators, Comments, etc.

## Code Style Guidelines
- Use async/await for database operations
- Implement proper error handling and validation
- Follow ASP.NET Core naming conventions
- Use dependency injection for services
- Implement proper security measures for file uploads
- Use ViewModels for complex views
