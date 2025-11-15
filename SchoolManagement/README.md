# School Management API

A .NET 9 Minimal API application for managing students and school classes.

## Features

### Student Management
- Create, Read, Update, and Delete students
- Required fields: Student ID, Name, Surname, Date of Birth
- Optional fields: City, Street, Postal Code
- Unique student ID validation
- Assign students to classes

### School Class Management
- Create, Read, Update, and Delete school classes
- Required fields: Name, Leading Teacher
- Maximum 20 students per class
- Add/Remove students to/from classes

## Technology Stack

- .NET 9
- ASP.NET Core Minimal API
- Entity Framework Core 9
- In-Memory Database

## Getting Started

### Prerequisites
- .NET 9 SDK

### Running the Application

1. Navigate to the project directory:
```bash
cd SchoolManagement
```

2. Run the application:
```bash
dotnet run
```

The API will be available at:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`

## API Endpoints

### Students

#### Get All Students
```
GET /api/students
```

#### Get Student by ID
```
GET /api/students/{id}
```

#### Create Student
```
POST /api/students
Content-Type: application/json

{
  "studentId": "S001",
  "name": "John",
  "surname": "Doe",
  "dateOfBirth": "2005-05-15",
  "city": "New York",
  "street": "123 Main St",
  "postalCode": "10001"
}
```

#### Update Student
```
PUT /api/students/{id}
Content-Type: application/json

{
  "name": "John",
  "surname": "Doe",
  "dateOfBirth": "2005-05-15",
  "city": "New York",
  "street": "123 Main St",
  "postalCode": "10001"
}
```

#### Delete Student
```
DELETE /api/students/{id}
```

### School Classes

#### Get All Classes
```
GET /api/classes
```

#### Get Class by ID
```
GET /api/classes/{id}
```

#### Create Class
```
POST /api/classes
Content-Type: application/json

{
  "name": "Class 5A",
  "leadingTeacher": "Mrs. Smith"
}
```

#### Update Class
```
PUT /api/classes/{id}
Content-Type: application/json

{
  "name": "Class 5A",
  "leadingTeacher": "Mrs. Smith"
}
```

#### Delete Class
```
DELETE /api/classes/{id}
```

#### Add Student to Class
```
POST /api/classes/{classId}/students
Content-Type: application/json

{
  "studentId": "S001"
}
```

#### Remove Student from Class
```
DELETE /api/classes/{classId}/students/{studentId}
```

## Validation Rules

1. **Student ID Uniqueness**: The system prevents creating multiple students with the same ID
2. **Maximum Students per Class**: Each class can have a maximum of 20 students
3. **Required Fields**: All required fields must be provided when creating or updating entities

## Testing

You can test the API using:
- The included `SchoolManagement.http` file (with Visual Studio or VS Code REST Client extension)
- Swagger/OpenAPI UI (available in development mode)
- Postman or any HTTP client
- curl commands

## Project Structure

```
SchoolManagement/
├── Models/
│   ├── Student.cs          # Student entity
│   ├── SchoolClass.cs      # School class entity
│   └── DTOs.cs             # Data Transfer Objects
├── Data/
│   └── SchoolDbContext.cs  # EF Core DbContext
├── Program.cs              # API endpoints and configuration
└── appsettings.json        # Configuration
```

## Sample Workflow

1. Create a school class:
```bash
curl -X POST https://localhost:5001/api/classes \
  -H "Content-Type: application/json" \
  -d '{"name":"Class 5A","leadingTeacher":"Mrs. Smith"}'
```

2. Create students:
```bash
curl -X POST https://localhost:5001/api/students \
  -H "Content-Type: application/json" \
  -d '{"studentId":"S001","name":"John","surname":"Doe","dateOfBirth":"2005-05-15"}'
```

3. Add student to class:
```bash
curl -X POST https://localhost:5001/api/classes/1/students \
  -H "Content-Type: application/json" \
  -d '{"studentId":"S001"}'
```

4. View all students in the class:
```bash
curl https://localhost:5001/api/classes/1
```

## Notes

- The application uses an in-memory database, so all data is lost when the application stops
- No authentication or authorization is implemented as per requirements
- All endpoints return JSON responses
- Proper HTTP status codes are used (200, 201, 400, 404, 409)

