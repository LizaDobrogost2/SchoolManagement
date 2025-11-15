# School Management API - Complete Refactoring

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
- **API Base URL**: `https://localhost:5001`

See **[QUICKSTART.md](QUICKSTART.md)** for detailed testing examples.

## API Endpoints

All endpoints:

### Students
- `GET /api/students` - List all
- `GET /api/students/{id}` - Get by ID
- `POST /api/students` - Create
- `PUT /api/students/{id}` - Update
- `DELETE /api/students/{id}` - Delete

### Classes
- `GET /api/classes` - List all
- `GET /api/classes/{id}` - Get by ID
- `POST /api/classes` - Create
- `PUT /api/classes/{id}` - Update
- `DELETE /api/classes/{id}` - Delete
- `POST /api/classes/{classId}/students` - Add student
- `DELETE /api/classes/{classId}/students/{studentId}` - Remove student

## Running the Application

```bash
cd SchoolManagement
dotnet run
```

Access Swagger UI: `https://localhost:<port>/swagger`

## ?? Next Steps (Optional Enhancements)

While the current implementation meets all requirements, you could consider:

1. **Validation**: Add FluentValidation for complex rules
2. **Logging**: Add Serilog for structured logging
3. **Caching**: Add response caching for GET endpoints
4. **Health Checks**: Add health check endpoints
5. **API Versioning**: Add versioning support
6. **Rate Limiting**: Add rate limiting middleware
7. **Global Exception Handling**: Add global exception handler
8. **Unit Tests**: Add comprehensive unit test suite
9. **Integration Tests**: Add integration tests
10. **Docker**: Add Dockerfile for containerization

## ?? Contributing

The code is now organized to make contributions easy:
1. Identify which layer needs changes
2. Update relevant interface/class
3. Add tests
4. Submit changes

---

**Built with ?? using .NET 9 and best practices**
