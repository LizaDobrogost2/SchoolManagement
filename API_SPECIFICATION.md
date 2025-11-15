# API Specification - School Management API

## ?? Base Information

- **Base URL**: `https://localhost:5001` (Development)
- **Protocol**: HTTPS (Development), HTTP available on port 5000
- **Content-Type**: `application/json`
- **API Style**: RESTful
- **Framework**: .NET 9 Minimal API
- **OpenAPI**: Available at `/swagger` and `/openapi/v1.json`

## ?? Endpoints Overview

### Students API
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/students` | Get all students |
| GET | `/api/students/{id}` | Get student by ID |
| POST | `/api/students` | Create a new student |
| PUT | `/api/students/{id}` | Update existing student |
| DELETE | `/api/students/{id}` | Delete student |

### Classes API
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/classes` | Get all classes |
| GET | `/api/classes/{id}` | Get class by ID |
| POST | `/api/classes` | Create a new class |
| PUT | `/api/classes/{id}` | Update existing class |
| DELETE | `/api/classes/{id}` | Delete class |
| POST | `/api/classes/{classId}/students` | Add student to class |
| DELETE | `/api/classes/{classId}/students/{studentId}` | Remove student from class |

---

## ?? Detailed Endpoint Specifications

### 1. Get All Students

**GET** `/api/students`

Get a list of all students in the system.

**Request**
- No parameters required

**Response** `200 OK`
```json
[
  {
    "studentId": "S001",
    "name": "John",
    "surname": "Doe",
    "dateOfBirth": "2005-05-15T00:00:00",
    "city": "New York",
    "street": "123 Main St",
    "postalCode": "10001",
    "schoolClassId": 1,
    "schoolClassName": "Class 5A"
  }
]
```

---

### 2. Get Student by ID

**GET** `/api/students/{id}`

Get details of a specific student.

**Path Parameters**
- `id` (string, required) - Student ID

**Response** `200 OK`
```json
{
  "studentId": "S001",
  "name": "John",
  "surname": "Doe",
  "dateOfBirth": "2005-05-15T00:00:00",
  "city": "New York",
  "street": "123 Main St",
  "postalCode": "10001",
  "schoolClassId": 1,
  "schoolClassName": "Class 5A"
}
```

**Error Responses**
- `404 Not Found` - Student does not exist
```json
{
  "message": "Student with ID 'S001' not found."
}
```

---

### 3. Create Student

**POST** `/api/students`

Create a new student.

**Request Body**
```json
{
  "studentId": "S001",      // Required, must be unique
  "name": "John",            // Required
  "surname": "Doe",          // Required
  "dateOfBirth": "2005-05-15", // Required, format: YYYY-MM-DD
  "city": "New York",        // Optional
  "street": "123 Main St",   // Optional
  "postalCode": "10001"      // Optional
}
```

**Field Constraints**
| Field | Type | Required | Max Length | Constraints |
|-------|------|----------|-----------|-------------|
| studentId | string | Yes | - | Must be unique |
| name | string | Yes | 100 | Not empty |
| surname | string | Yes | 100 | Not empty |
| dateOfBirth | DateTime | Yes | - | Valid date |
| city | string | No | 100 | - |
| street | string | No | 200 | - |
| postalCode | string | No | 20 | - |

**Response** `201 Created`
```json
{
  "studentId": "S001",
  "name": "John",
  "surname": "Doe",
  "dateOfBirth": "2005-05-15T00:00:00",
  "city": "New York",
  "street": "123 Main St",
  "postalCode": "10001",
  "schoolClassId": null,
  "schoolClassName": null
}
```

**Error Responses**
- `400 Bad Request` - Missing required fields or validation failed
```json
{
  "message": "StudentId is required. Name is required. Surname is required."
}
```

- `409 Conflict` - Student ID already exists
```json
{
  "message": "Student with ID 'S001' already exists."
}
```

---

### 4. Update Student

**PUT** `/api/students/{id}`

Update an existing student's information.

**Path Parameters**
- `id` (string, required) - Student ID to update

**Request Body**
```json
{
  "name": "John Updated",    // Required
  "surname": "Doe",          // Required
  "dateOfBirth": "2005-05-15", // Required
  "city": "Los Angeles",     // Optional
  "street": "456 Oak Ave",   // Optional
  "postalCode": "90001"      // Optional
}
```

**Note**: StudentId cannot be changed

**Response** `200 OK`
```json
{
  "studentId": "S001",
  "name": "John Updated",
  "surname": "Doe",
  "dateOfBirth": "2005-05-15T00:00:00",
  "city": "Los Angeles",
  "street": "456 Oak Ave",
  "postalCode": "90001",
  "schoolClassId": 1,
  "schoolClassName": null
}
```

**Error Responses**
- `400 Bad Request` - Validation failed
- `404 Not Found` - Student does not exist

---

### 5. Delete Student

**DELETE** `/api/students/{id}`

Delete a student from the system.

**Path Parameters**
- `id` (string, required) - Student ID to delete

**Response** `200 OK`
```json
{
  "message": "Student with ID 'S001' has been deleted."
}
```

**Error Responses**
- `404 Not Found` - Student does not exist

**Note**: Deleting a student automatically removes them from their class.

---

### 6. Get All Classes

**GET** `/api/classes`

Get a list of all school classes.

**Response** `200 OK`
```json
[
  {
    "id": 1,
    "name": "Class 5A",
    "leadingTeacher": "Mrs. Smith",
    "studentCount": 2,
    "students": [
      {
        "studentId": "S001",
        "name": "John",
        "surname": "Doe",
        "dateOfBirth": "2005-05-15T00:00:00",
        "city": "New York",
        "street": "123 Main St",
        "postalCode": "10001",
        "schoolClassId": 1,
        "schoolClassName": "Class 5A"
      }
    ]
  }
]
```

---

### 7. Get Class by ID

**GET** `/api/classes/{id}`

Get details of a specific class including all students.

**Path Parameters**
- `id` (integer, required) - Class ID

**Response** `200 OK`
```json
{
  "id": 1,
  "name": "Class 5A",
  "leadingTeacher": "Mrs. Smith",
  "studentCount": 2,
  "students": [...]
}
```

**Error Responses**
- `404 Not Found` - Class does not exist

---

### 8. Create Class

**POST** `/api/classes`

Create a new school class.

**Request Body**
```json
{
  "name": "Class 5A",           // Required
  "leadingTeacher": "Mrs. Smith" // Required
}
```

**Field Constraints**
| Field | Type | Required | Max Length |
|-------|------|----------|-----------|
| name | string | Yes | 100 |
| leadingTeacher | string | Yes | 100 |

**Response** `201 Created`
```json
{
  "id": 1,
  "name": "Class 5A",
  "leadingTeacher": "Mrs. Smith",
  "studentCount": 0,
  "students": []
}
```

**Error Responses**
- `400 Bad Request` - Missing required fields

---

### 9. Update Class

**PUT** `/api/classes/{id}`

Update an existing class.

**Path Parameters**
- `id` (integer, required) - Class ID to update

**Request Body**
```json
{
  "name": "Class 5A Advanced",  // Required
  "leadingTeacher": "Dr. Smith"  // Required
}
```

**Response** `200 OK`
```json
{
  "id": 1,
  "name": "Class 5A Advanced",
  "leadingTeacher": "Dr. Smith",
  "studentCount": 2,
  "students": []
}
```

**Error Responses**
- `400 Bad Request` - Validation failed
- `404 Not Found` - Class does not exist

---

### 10. Delete Class

**DELETE** `/api/classes/{id}`

Delete a class from the system.

**Path Parameters**
- `id` (integer, required) - Class ID to delete

**Response** `200 OK`
```json
{
  "message": "School class with ID 1 has been deleted."
}
```

**Error Responses**
- `404 Not Found` - Class does not exist

**Note**: All students in the class will have their `schoolClassId` set to `null`.

---

### 11. Add Student to Class

**POST** `/api/classes/{classId}/students`

Add a student to a class.

**Path Parameters**
- `classId` (integer, required) - Class ID

**Request Body**
```json
{
  "studentId": "S001"  // Required
}
```

**Response** `200 OK`
```json
{
  "message": "Student 'John Doe' has been added to class 'Class 5A'."
}
```

**Error Responses**
- `400 Bad Request` - Class is full (20 students) or student already in class
```json
{
  "message": "Class 'Class 5A' already has the maximum of 20 students."
}
```
```json
{
  "message": "Student 'John Doe' is already in this class."
}
```

- `404 Not Found` - Class or student does not exist

**Note**: Adding a student to a new class automatically moves them from their current class.

---

### 12. Remove Student from Class

**DELETE** `/api/classes/{classId}/students/{studentId}`

Remove a student from a class.

**Path Parameters**
- `classId` (integer, required) - Class ID
- `studentId` (string, required) - Student ID

**Response** `200 OK`
```json
{
  "message": "Student 'John Doe' has been removed from class 'Class 5A'."
}
```

**Error Responses**
- `400 Bad Request` - Student is not in this class
```json
{
  "message": "Student 'John Doe' is not in this class."
}
```

- `404 Not Found` - Class or student does not exist

---

## ?? Data Models

### StudentDto
```typescript
{
  studentId: string;           // Primary identifier
  name: string;                // First name
  surname: string;             // Last name
  dateOfBirth: DateTime;       // Birth date
  city?: string | null;        // Optional city
  street?: string | null;      // Optional street address
  postalCode?: string | null;  // Optional postal code
  schoolClassId?: number | null; // Class assignment
  schoolClassName?: string | null; // Class name
}
```

### CreateStudentDto
```typescript
{
  studentId: string;     // Required, unique
  name: string;          // Required
  surname: string;       // Required
  dateOfBirth: DateTime; // Required
  city?: string | null;
  street?: string | null;
  postalCode?: string | null;
}
```

### UpdateStudentDto
```typescript
{
  name: string;          // Required
  surname: string;       // Required
  dateOfBirth: DateTime; // Required
  city?: string | null;
  street?: string | null;
  postalCode?: string | null;
}
```

### SchoolClassDto
```typescript
{
  id: number;                  // Auto-generated
  name: string;                // Class name
  leadingTeacher: string;      // Teacher name
  studentCount: number;        // Number of students
  students: StudentDto[];      // Array of students
}
```

### CreateSchoolClassDto
```typescript
{
  name: string;           // Required
  leadingTeacher: string; // Required
}
```

### UpdateSchoolClassDto
```typescript
{
  name: string;           // Required
  leadingTeacher: string; // Required
}
```

### AddStudentToClassDto
```typescript
{
  studentId: string; // Required
}
```

---

## ?? Business Rules

1. **Unique Student ID**
   - Student IDs must be unique across the system
   - Attempting to create duplicate IDs returns `409 Conflict`

2. **Maximum Class Size**
   - Each class can have maximum **20 students**
   - Attempting to exceed this returns `400 Bad Request`

3. **Class Deletion**
   - Deleting a class sets all students' `schoolClassId` to `null`
   - Students remain in the system

4. **Student Deletion**
   - Deleting a student removes them from their class
   - Class student count is updated automatically

5. **Moving Students**
   - Adding a student to a new class automatically removes them from the previous class
   - A student can only be in one class at a time

6. **Required Fields**
   - Students: `studentId`, `name`, `surname`, `dateOfBirth`
   - Classes: `name`, `leadingTeacher`

---

## ?? HTTP Status Codes

| Code | Description | Usage |
|------|-------------|-------|
| 200 | OK | Successful GET, PUT, DELETE |
| 201 | Created | Successful POST (resource created) |
| 400 | Bad Request | Validation error, business rule violation |
| 404 | Not Found | Resource doesn't exist |
| 409 | Conflict | Duplicate resource (student ID exists) |

---

## ?? Authentication

No authentication or authorization is required for this API (as per requirements).

---

## ?? Data Persistence

- **Database**: In-Memory (EF Core)
- **Lifetime**: Data exists only while application is running
- **Restart Behavior**: All data is lost on application restart

---

## ?? Additional Information

### Date Format
- **Input**: `YYYY-MM-DD` (e.g., "2005-05-15")
- **Output**: ISO 8601 (e.g., "2005-05-15T00:00:00")

### Content Negotiation
- Only JSON is supported
- Always use `Content-Type: application/json`

### CORS
- Configured for development
- All origins allowed in development mode

### Swagger/OpenAPI
- Available at `/swagger` in development
- OpenAPI JSON at `/openapi/v1.json`

---

## ?? Testing

See **TESTING_GUIDE.md** for comprehensive testing scenarios and examples.

See **QUICKSTART.md** for quick start examples and curl commands.

---

**API Version**: 1.0  
**Last Updated**: 2024  
**Framework**: .NET 9
