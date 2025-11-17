# School Management API

A .NET 9 Minimal API for managing Students and School Classes. Demonstrates clean separation of concerns (Endpoints, Services, Repositories), API Versioning, Serilog logging, and automated tests.

## Features
- Student CRUD (with PATCH for partial updates + class assignment)
- School Class CRUD (max 20 students constraint)
- In-memory EF Core database (easy to swap to persistent store)
- API Versioning (URL segment, header, media type readers)
- Swagger / OpenAPI (Development only)
- Structured logging with Serilog
- Global exception handling & ProblemDetails responses

## Tech Stack
- .NET 9 Minimal API
- EF Core InMemory
- Serilog
- Asp.Versioning
- Swagger / Swashbuckle

## Project Structure
```
SchoolManagement/            API project
  Configuration/            Modular service & middleware configuration
  Data/                      DbContext + entity configurations
  Endpoints/                Minimal API endpoint groups
  Middleware/               Global exception handler
  Models/                   Entities & DTOs
  Services/                 Service layer + result wrappers
  Repositories/             Repository abstractions & implementations
SchoolManagement.Tests/     Unit & integration tests
```

## Requirements
- .NET 9 SDK
- (Optional) Docker / Docker Compose

## Quick Start (Local)
```bash
cd SchoolManagement
dotnet run
# Swagger (Development): https://localhost:<port>/swagger
```

## Quick Start (Docker Compose)
```bash
docker-compose up -d
# API Base URL: http://localhost:5000/api/v1
# Swagger UI:   http://localhost:5000/swagger (if Development config applied)
```

## Configuration
Logging levels are defined in `appsettings.json` with environment overrides (e.g. `appsettings.Development.json`). Serilog is configured early and enriched with request diagnostics.

Switch database: replace `UseInMemoryDatabase` in `DatabaseConfiguration` with a provider (e.g. `UseSqlServer` or `UseSqlite`).

## Logging
Serilog request logging template:
```
HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms
```
Additional context: RequestHost, RequestScheme.

## API Versioning
Default version: v1.0. Version can be supplied via:
- URL segment: `/api/v1/...`
- Header: `X-Api-Version: 1.0`
- Media type parameter: `Accept: application/json; version=1.0`

## Endpoints (v1)
### Students
- GET    `/api/v1/students`
- GET    `/api/v1/students/{id}`
- POST   `/api/v1/students`
- PUT    `/api/v1/students/{id}`
- PATCH  `/api/v1/students/{id}` (partial or class assignment)
- DELETE `/api/v1/students/{id}`

### Classes
- GET    `/api/v1/classes`
- GET    `/api/v1/classes/{id}`
- POST   `/api/v1/classes`
- PUT    `/api/v1/classes/{id}`
- PATCH  `/api/v1/classes/{id}`
- DELETE `/api/v1/classes/{id}`

### Assign / Unassign Student to Class
Assign:
```http
PATCH /api/v1/students/S001
Content-Type: application/json

{
  "schoolClassId": 1
}
```
Unassign:
```http
PATCH /api/v1/students/S001
Content-Type: application/json

{
  "schoolClassId": null
}
```

## Testing
Run all tests:
```bash
dotnet test
```
