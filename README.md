# School Management API 

## Business Requirements 

 **Student Management**
- Create/Edit/List/Delete students
- Required: StudentId, Name, Surname, DateOfBirth
- Optional: City, Street, PostalCode
- Unique StudentId validation

 **School Class Management**
- Create/Edit/List/Delete classes
- Add/Remove students to/from classes
- Required: Name, LeadingTeacher
- Maximum 20 students per class

 **Technical Requirements**
- .NET 9 Minimal API
- In-memory database with EF Core
- No authentication/authorization required

## Quick Start

```bash
cd SchoolManagement
dotnet run
```

Then open your browser to:
- **Swagger UI**: `https://localhost:5001/swagger`
- **API Base URL**: `https://localhost:5001/api/v1`

See **[QUICKSTART.md](QUICKSTART.md)** for detailed testing examples.

## API Endpoints (v1)

All endpoints are versioned. Current version: **v1.0**

### Students
- `GET /api/v1/students` - List all
- `GET /api/v1/students/{id}` - Get by ID
- `POST /api/v1/students` - Create
- `PUT /api/v1/students/{id}` - Update (full)
- `PATCH /api/v1/students/{id}` - Update (partial) - **Use for class assignments**
- `DELETE /api/v1/students/{id}` - Delete

### Classes
- `GET /api/v1/classes` - List all
- `GET /api/v1/classes/{id}` - Get by ID
- `POST /api/v1/classes` - Create
- `PUT /api/v1/classes/{id}` - Update (full)
- `PATCH /api/v1/classes/{id}` - Update (partial)
- `DELETE /api/v1/classes/{id}` - Delete
- ~~`POST /api/v1/classes/{classId}/students`~~ - **Deprecated** (use PATCH student instead)
- ~~`DELETE /api/v1/classes/{classId}/students/{studentId}`~~ - **Deprecated** (use PATCH student instead)

### Assigning Students to Classes

**Recommended approach** (RESTful):
```http
PATCH /api/v1/students/S001
Content-Type: application/json

{
  "schoolClassId": 1  // Assign to class
}
```

```http
PATCH /api/v1/students/S001
Content-Type: application/json

{
  "schoolClassId": null  // Unassign from class
}
```

## Running the Application

```bash
cd SchoolManagement
dotnet run
```

Access Swagger UI: `https://localhost:<port>/swagger`

