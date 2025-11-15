# School Management API - Refactored

## Overview
This is a .NET 9 Minimal API application for managing students and school classes, refactored to follow industry best practices and SOLID principles.

## Architecture

The application follows a **layered architecture** with clear separation of concerns:

```
???????????????????????????????????????
?          Endpoints Layer            ?  ? HTTP/API concerns
???????????????????????????????????????
?          Services Layer             ?  ? Business logic
???????????????????????????????????????
?        Repositories Layer           ?  ? Data access
???????????????????????????????????????
?         Database (EF Core)          ?  ? In-Memory DB
???????????????????????????????????????
```

## Project Structure

```
SchoolManagement/
??? Common/
?   ??? Constants.cs              # Business constants and validation messages
??? Data/
?   ??? SchoolDbContext.cs        # EF Core DbContext
??? Endpoints/
?   ??? StudentEndpoints.cs       # Student API endpoints
?   ??? SchoolClassEndpoints.cs   # Class API endpoints
?   ??? ResultExtensions.cs       # Result mapping extensions
??? Extensions/
?   ??? MappingExtensions.cs      # Entity ? DTO mapping
??? Models/
?   ??? Student.cs                # Student entity
?   ??? SchoolClass.cs            # SchoolClass entity
?   ??? DTOs.cs                   # Data Transfer Objects
??? Repositories/
?   ??? StudentRepository.cs      # Student data access
?   ??? SchoolClassRepository.cs  # Class data access
??? Services/
?   ??? StudentService.cs         # Student business logic
?   ??? SchoolClassService.cs     # Class business logic
??? Program.cs                    # Application entry point
```

## Key Improvements

### 1. **Repository Pattern**
- Abstracts data access logic from business logic
- Makes the code more testable by allowing repository mocking
- Provides a single source for data operations

### 2. **Service Layer**
- Encapsulates business logic and validation
- Uses the `ServiceResult<T>` pattern for consistent error handling
- Easier to unit test without dealing with HTTP concerns

### 3. **Separation of Concerns**
- **Endpoints**: Handle HTTP requests/responses only
- **Services**: Contain business logic and orchestration
- **Repositories**: Manage data persistence
- **Models**: Define domain entities
- **DTOs**: Control API contract

### 4. **Dependency Injection**
- All dependencies are registered in `Program.cs`
- Follows SOLID principles (Dependency Inversion)
- Easy to swap implementations for testing

### 5. **Extension Methods**
- `MappingExtensions`: Clean entity ? DTO conversions
- `ResultExtensions`: Consistent ServiceResult to IResult mapping
- `EndpointExtensions`: Organized endpoint registration

### 6. **Constants Management**
- Centralized business rules (e.g., `MaxStudentsPerClass = 20`)
- Consistent validation messages
- Easy to maintain and update

### 7. **Result Pattern**
- `ServiceResult<T>` encapsulates operation outcomes
- Provides consistent error handling across the application
- Includes status codes for proper HTTP response mapping

## API Endpoints

### Students

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/students` | Get all students |
| GET | `/api/students/{id}` | Get student by ID |
| POST | `/api/students` | Create a new student |
| PUT | `/api/students/{id}` | Update student |
| DELETE | `/api/students/{id}` | Delete student |

### School Classes

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/classes` | Get all classes |
| GET | `/api/classes/{id}` | Get class by ID |
| POST | `/api/classes` | Create a new class |
| PUT | `/api/classes/{id}` | Update class |
| DELETE | `/api/classes/{id}` | Delete class |
| POST | `/api/classes/{classId}/students` | Add student to class |
| DELETE | `/api/classes/{classId}/students/{studentId}` | Remove student from class |

## Business Rules

### Students
- **Required fields**: StudentId, Name, Surname, DateOfBirth
- **Optional fields**: City, Street, PostalCode
- **Constraints**: 
  - StudentId must be unique
  - Cannot create duplicate students

### School Classes
- **Required fields**: Name, LeadingTeacher
- **Constraints**: 
  - Maximum 20 students per class
  - Deleting a class sets all students' SchoolClassId to null

## Running the Application

```bash
dotnet run --project SchoolManagement
```

Access Swagger UI at: `https://localhost:<port>/swagger`

## Testing

The architecture makes it easy to write unit tests:

```csharp
// Example: Testing StudentService
var mockRepo = new Mock<IStudentRepository>();
var service = new StudentService(mockRepo.Object);
var result = await service.GetStudentByIdAsync("123");
```

## Benefits of This Architecture

1. **Testability**: Each layer can be tested independently
2. **Maintainability**: Changes are localized to specific layers
3. **Scalability**: Easy to add new features without breaking existing code
4. **Readability**: Clear responsibility for each component
5. **Reusability**: Services and repositories can be used by different endpoints
6. **Type Safety**: Strong typing with DTOs and entities
7. **Error Handling**: Consistent error responses across all endpoints

## Best Practices Implemented

- ? Dependency Injection
- ? Repository Pattern
- ? Service Layer Pattern
- ? Result Pattern for error handling
- ? DTOs for API contracts
- ? Extension methods for clean code
- ? Constants for magic values
- ? Async/await for all I/O operations
- ? Interface segregation
- ? Single Responsibility Principle
- ? Clear separation of concerns
- ? Proper HTTP status codes
- ? API documentation with Swagger
