# School Management API - Project Overview

## 📋 Business Requirements

This application fulfills the following business requirements:

### 1. Student Management
- **Create/Edit/List/Delete** students in the school system
- **Required fields**: Student ID, Name, Surname, Date of Birth
- **Optional fields**: City, Street, Postal Code
- **Validation**: System prevents creation of multiple students with the same ID

### 2. School Class Management
- **Create/Edit/List/Delete** school classes
- **Add/Remove** students to/from classes
- **Required fields**: Name, Leading Teacher
- **Constraint**: Maximum 20 students per class

### 3. Technical Implementation
- ✅ .NET 9 Minimal API
- ✅ Entity Framework Core with In-Memory Database
- ✅ No authentication/authorization required
- ✅ RESTful API design
- ✅ OpenAPI/Swagger documentation

## 🏗️ Architecture

The application follows a **Clean Architecture** pattern with clear separation of concerns:

```
┌──────────────────────────────────┐
│    Endpoints (HTTP Layer)        │  ← API endpoints, request/response handling
├──────────────────────────────────┤
│    Services (Business Layer)     │  ← Business logic, validation, orchestration
├──────────────────────────────────┤
│    Repositories (Data Layer)     │  ← Data access, EF Core operations
├──────────────────────────────────┤
│    DbContext (Persistence)       │  ← In-Memory Database
└──────────────────────────────────┘
```

## 📁 Project Structure

```
SchoolManagement/
├── Common/
│   └── Constants.cs                    # Business constants & validation messages
│
├── Data/
│   └── SchoolDbContext.cs              # EF Core DbContext configuration
│
├── Endpoints/
│   ├── StudentEndpoints.cs             # Student API endpoints
│   ├── SchoolClassEndpoints.cs         # Class API endpoints
│   └── ResultExtensions.cs             # ServiceResult to IResult mapping
│
├── Extensions/
│   └── MappingExtensions.cs            # Entity ↔ DTO mapping extensions
│
├── Models/
│   ├── Student.cs                      # Student entity model
│   ├── SchoolClass.cs                  # SchoolClass entity model
│   └── DTOs.cs                         # Data Transfer Objects
│
├── Repositories/
│   ├── StudentRepository.cs            # Student data access layer
│   └── SchoolClassRepository.cs        # Class data access layer
│
├── Services/
│   ├── StudentService.cs               # Student business logic
│   └── SchoolClassService.cs           # Class business logic
│
├── Program.cs                          # Application entry point & DI setup
├── appsettings.json                    # Application configuration
└── appsettings.Development.json        # Development configuration
```

## 🚀 API Endpoints

### Students API

| Method | Endpoint | Description | Status Codes |
|--------|----------|-------------|--------------|
| GET | `/api/students` | Get all students | 200 OK |
| GET | `/api/students/{id}` | Get student by ID | 200 OK, 404 Not Found |
| POST | `/api/students` | Create new student | 201 Created, 400 Bad Request, 409 Conflict |
| PUT | `/api/students/{id}` | Update student | 200 OK, 400 Bad Request, 404 Not Found |
| DELETE | `/api/students/{id}` | Delete student | 200 OK, 404 Not Found |

### Classes API

| Method | Endpoint | Description | Status Codes |
|--------|----------|-------------|--------------|
| GET | `/api/classes` | Get all classes | 200 OK |
| GET | `/api/classes/{id}` | Get class by ID | 200 OK, 404 Not Found |
| POST | `/api/classes` | Create new class | 201 Created, 400 Bad Request |
| PUT | `/api/classes/{id}` | Update class | 200 OK, 400 Bad Request, 404 Not Found |
| DELETE | `/api/classes/{id}` | Delete class | 200 OK, 404 Not Found |
| POST | `/api/classes/{classId}/students` | Add student to class | 200 OK, 400 Bad Request, 404 Not Found |
| DELETE | `/api/classes/{classId}/students/{studentId}` | Remove student from class | 200 OK, 400 Bad Request, 404 Not Found |

## ✅ Implementation Status

All requirements have been successfully implemented:

### 1. Student Management ✅
- ✅ Create students with required fields (Student ID, Name, Surname, Date of Birth)
- ✅ Optional fields support (City, Street, Postal Code)
- ✅ Edit student information
- ✅ List all students with class information
- ✅ Delete students
- ✅ **Unique Student ID validation** - prevents duplicate student IDs (409 Conflict)

### 2. School Class Management ✅
- ✅ Create classes with required fields (Name, Leading Teacher)
- ✅ Edit class information
- ✅ List all classes with student count
- ✅ Delete classes (automatically unassigns students)
- ✅ Add students to classes
- ✅ Remove students from classes
- ✅ **Maximum 20 students per class** - enforced validation (400 Bad Request)

### 3. Technical Requirements ✅
- ✅ .NET 9 Minimal API
- ✅ EF Core with In-Memory Database
- ✅ No authentication/authorization
- ✅ RESTful API endpoints
- ✅ Proper HTTP status codes (200, 201, 400, 404, 409)
- ✅ JSON request/response format
- ✅ OpenAPI/Swagger documentation

## 🔒 Business Rules Implemented

### Student Rules
1. **Unique Student ID Constraint**
   - System validates student ID uniqueness before creation
   - Returns `409 Conflict` if duplicate ID is detected
   - Error message: "Student with ID '{id}' already exists."

2. **Required Field Validation**
   - StudentId, Name, Surname, DateOfBirth are mandatory
   - Returns `400 Bad Request` if any required field is missing

3. **Data Integrity**
   - Students can exist without being assigned to a class
   - Deleting a student removes them from their class automatically

### Class Rules
1. **Maximum Class Size (20 Students)**
   - System enforces maximum of 20 students per class
   - Returns `400 Bad Request` when attempting to exceed limit
   - Error message: "Class '{name}' already has the maximum of 20 students."

2. **Required Field Validation**
   - Name and LeadingTeacher are mandatory
   - Returns `400 Bad Request` if any required field is missing

3. **Cascade Behavior**
   - When a class is deleted, all students' `SchoolClassId` is set to `null`
   - Students remain in the system, just unassigned from the class

## 🎯 Design Patterns Used

### 1. Repository Pattern
- Abstracts data access logic
- Makes code testable by allowing mock repositories
- Single source of truth for data operations

### 2. Service Layer Pattern
- Encapsulates business logic
- Validates input and enforces business rules
- Returns structured results using `ServiceResult<T>`

### 3. Result Pattern
- Consistent error handling across the application
- Eliminates exception-based flow control
- Clear success/failure states with messages

### 4. Dependency Injection
- All dependencies injected via constructor
- Follows SOLID principles
- Easy to swap implementations for testing

### 5. Extension Methods
- Clean entity-to-DTO mapping
- Organized code structure
- Reusable helper methods

## 🧪 Testing the Application

### Quick Start
```bash
cd SchoolManagement
dotnet run
```

The application will start on `https://localhost:5001` (HTTPS) and `http://localhost:5000` (HTTP).

### Testing Options

1. **Swagger UI** (Recommended for beginners)
   - Navigate to `https://localhost:5001/swagger`
   - Interactive API documentation
   - Try endpoints directly in the browser

2. **curl Commands** (See QUICKSTART.md)
   - Command-line testing
   - Scriptable and automatable

3. **HTTP Files** (VS Code REST Client)
   - Create `.http` files with requests
   - Execute directly from the editor

4. **API Clients** (Postman, Insomnia, etc.)
   - Full-featured API testing
   - Save collections for reuse

## 📊 Data Models

### Student Entity
```csharp
- StudentId: string (Primary Key, Required, Unique)
- Name: string (Required, Max: 100)
- Surname: string (Required, Max: 100)
- DateOfBirth: DateTime (Required)
- City: string (Optional, Max: 100)
- Street: string (Optional, Max: 200)
- PostalCode: string (Optional, Max: 20)
- SchoolClassId: int? (Foreign Key, Optional)
- SchoolClass: SchoolClass (Navigation Property)
```

### SchoolClass Entity
```csharp
- Id: int (Primary Key, Auto-increment)
- Name: string (Required, Max: 100)
- LeadingTeacher: string (Required, Max: 100)
- Students: List<Student> (Navigation Property)
```

## 🔄 Data Flow Example

**Creating a Student and Adding to Class:**

```
1. POST /api/students
   ↓
2. StudentEndpoints.CreateStudent
   ↓
3. StudentService.CreateStudentAsync
   ↓
4. Validates required fields
   ↓
5. Checks for duplicate ID
   ↓
6. StudentRepository.AddAsync
   ↓
7. DbContext.SaveChangesAsync
   ↓
8. Returns ServiceResult<StudentDto>
   ↓
9. ResultExtensions.ToHttpResult
   ↓
10. Returns 201 Created with student data
```

## 🛠️ Technologies Used

- **.NET 9** - Latest .NET framework
- **ASP.NET Core Minimal API** - Lightweight API framework
- **Entity Framework Core** - ORM for data access
- **In-Memory Database** - Fast, temporary storage for development
- **Swagger/OpenAPI** - API documentation and testing
- **C# 13** - Latest C# language features
- **Records** - Immutable DTOs
- **Pattern Matching** - Modern C# syntax

## 📚 Documentation Files

- **README.md** - Complete project overview and benefits
- **ARCHITECTURE.md** - Detailed architecture explanation
- **REFACTORING_SUMMARY.md** - Before/after comparison
- **QUICKSTART.md** - Quick start guide with curl examples
- **PROJECT_OVERVIEW.md** - This file (business requirements & implementation)

## 🎓 Learning Outcomes

This project demonstrates:
- Clean Architecture principles
- SOLID principles in practice
- Repository and Service patterns
- RESTful API design
- Entity Framework Core usage
- Dependency Injection
- Result-based error handling
- DTO pattern for API contracts
- Extension methods for clean code
- Minimal API organization

## 🚀 Next Steps (Optional Enhancements)

While all requirements are met, consider these enhancements:

1. **FluentValidation** - Advanced validation rules
2. **Serilog** - Structured logging
3. **AutoMapper** - Automatic DTO mapping
4. **MediatR** - CQRS pattern implementation
5. **Unit Tests** - xUnit test suite
6. **Integration Tests** - API endpoint testing
7. **Health Checks** - Application health monitoring
8. **Docker** - Containerization
9. **Response Caching** - Performance optimization
10. **Rate Limiting** - API protection

---

**Status**: ✅ All Requirements Implemented | Build: ✅ Successful | Tests: Ready for Implementation

