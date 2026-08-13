# Assignment & Submission Management System

A full-stack role-based web application for managing assignments and submissions in a school or college. Built as part of the OnnoRokom Projukti Assistant Software Engineer recruitment assignment.

## Tech Stack

| Layer | Technology |
|---|---|
| Frontend | Next.js 14, React, TypeScript, Tailwind CSS |
| Backend | ASP.NET Core Web API, C# |
| Database | PostgreSQL + Entity Framework Core |
| Auth | JWT-based authentication, role-based authorization |
| Tests | xUnit, EF Core InMemory |
| Docs | Swagger / OpenAPI |

## User Roles

- **Admin** — manage users, classes, subjects, enroll students
- **Teacher** — create/manage assignments, review and grade submissions
- **Student** — view assignments, submit answers, view marks and feedback

## Project Structure

```
assignment-management-system/
├── backend/
│   ├── AssignmentSystem.API/            # Controllers, middleware, entry point
│   ├── AssignmentSystem.Core/           # Entities, DTOs
│   ├── AssignmentSystem.Infrastructure/ # EF Core, DbContext, services
│   └── AssignmentSystem.Tests/          # xUnit unit tests
├── frontend/                            # Next.js 14 app (App Router)
├── .env.example                         # Environment variable reference
└── README.md
```

## Prerequisites

- .NET 10 SDK
- Node.js 18+
- PostgreSQL

## Database Setup

```bash
sudo -u postgres psql -c "CREATE DATABASE assignment_system;"
sudo -u postgres psql -c "ALTER USER postgres WITH PASSWORD 'postgres';"
```

## Backend Setup

```bash
cd backend
```

Update the connection string in `AssignmentSystem.API/appsettings.json`:

```json
"DefaultConnection": "Host=localhost;Port=5432;Database=assignment_system;Username=postgres;Password=postgres"
```

Then run:

```bash
# Apply migrations — creates all tables automatically
dotnet ef database update --project AssignmentSystem.Infrastructure --startup-project AssignmentSystem.API

# Start the API
dotnet run --project AssignmentSystem.API
```

- API runs at: `http://localhost:5038`
- Swagger UI: `http://localhost:5038/swagger`

## Frontend Setup

```bash
cd frontend

# Install dependencies
npm install

# Create environment file
echo "NEXT_PUBLIC_API_URL=http://localhost:5038" > .env.local

# Start dev server
npm run dev
```

- Frontend runs at: `http://localhost:3000`

## Running Tests

```bash
cd backend
dotnet test AssignmentSystem.Tests/AssignmentSystem.Tests.csproj
```

Expected: 9 tests, 0 failed.

## Demo Credentials

| Role    | Email              | Password    |
|---------|--------------------|-------------|
| Admin   | admin@school.com   | Admin@123   |
| Teacher | teacher@school.com | Teacher@123 |
| Student | student@school.com | Student@123 |

These accounts are seeded automatically on first run. No manual setup needed.

## API Endpoints

| Method | Endpoint | Role | Description |
|--------|----------|------|-------------|
| POST | /api/auth/login | Public | Login, returns JWT token |
| GET | /api/auth/me | Any | Get current user profile |
| GET | /api/admin/users | Admin | List all users |
| POST | /api/admin/users | Admin | Create a user |
| PATCH | /api/admin/users/{id}/toggle-active | Admin | Activate or deactivate user |
| GET | /api/admin/classes | Admin | List all classes and subjects |
| POST | /api/admin/classes | Admin | Create a class |
| POST | /api/admin/subjects | Admin | Create a subject and assign teacher |
| POST | /api/admin/enroll | Admin | Enroll a student into a class |
| GET | /api/assignment/my | Teacher/Admin | Get teacher's assignments |
| GET | /api/assignment/class | Student | Get assignments for student's class |
| GET | /api/assignment/{id} | Any | Get assignment details |
| POST | /api/assignment | Teacher | Create an assignment |
| PUT | /api/assignment/{id} | Teacher | Update an assignment |
| DELETE | /api/assignment/{id} | Teacher | Delete an assignment |
| GET | /api/assignment/teacher/subjects | Teacher | Get teacher's subjects |
| POST | /api/submission | Student | Submit an assignment |
| PUT | /api/submission/{id} | Student | Update submission before deadline |
| GET | /api/submission/my/{assignmentId} | Student | Get own submission |
| GET | /api/submission/assignment/{id} | Teacher/Admin | Get all submissions for assignment |
| PATCH | /api/submission/{id}/grade | Teacher/Admin | Grade a submission |

## Assumptions

- A student must be enrolled in a class by Admin before they can see assignments
- Submissions can only be updated before the deadline
- Draft assignments are not visible to students
- A teacher can only create assignments for subjects assigned to them by Admin
- Late submissions are automatically flagged when submitted after the deadline
- One student can only submit once per assignment

## Known Limitations

- File attachments not supported - text answers only
- No email notifications
- No pagination (suitable for demo scale)
- Npgsql does not yet have a stable release for EF Core 10 — version 9.x is used, which causes a non-breaking version mismatch warning at build time