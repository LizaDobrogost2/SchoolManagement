# Testing Guide - School Management API

## ?? Overview

This guide provides comprehensive testing scenarios for the School Management API to verify all business requirements and validation rules.

## ?? Testing Objectives

1. Verify CRUD operations for Students and Classes
2. Test business rules (unique student ID, 20 students per class)
3. Validate error handling and status codes
4. Confirm data relationships and integrity

## ?? Prerequisites

### Start the Application
```bash
cd SchoolManagement
dotnet run
```

### Testing Tools (Choose One)

1. **Swagger UI** (Easiest)
   - URL: `https://localhost:5001/swagger`
   - No installation required
   - Visual interface

2. **VS Code REST Client**
   - Install: "REST Client" extension
   - Use: `test-requests.http` file
   - Click "Send Request"

3. **curl** (Command Line)
   - Pre-installed on most systems
   - See QUICKSTART.md for examples

4. **Postman/Insomnia**
   - Full-featured API clients
   - Import OpenAPI spec from Swagger

## ?? Test Scenarios

### Scenario 1: Basic Student Management ?

**Objective**: Test student CRUD operations

| Step | Action | Expected Result | Status Code |
|------|--------|----------------|-------------|
| 1 | Create student S001 | Student created successfully | 201 Created |
| 2 | Get student S001 | Returns student details | 200 OK |
| 3 | List all students | Returns array with S001 | 200 OK |
| 4 | Update student S001 | Student updated successfully | 200 OK |
| 5 | Delete student S001 | Student deleted successfully | 200 OK |
| 6 | Get deleted student | Student not found error | 404 Not Found |

**Test Data**:
```json
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

### Scenario 2: Basic Class Management ?

**Objective**: Test class CRUD operations

| Step | Action | Expected Result | Status Code |
|------|--------|----------------|-------------|
| 1 | Create class "Math 101" | Class created successfully | 201 Created |
| 2 | Get class by ID | Returns class details | 200 OK |
| 3 | List all classes | Returns array with class | 200 OK |
| 4 | Update class name | Class updated successfully | 200 OK |
| 5 | Delete class | Class deleted successfully | 200 OK |
| 6 | Get deleted class | Class not found error | 404 Not Found |

**Test Data**:
```json
{
  "name": "Mathematics 101",
  "leadingTeacher": "Prof. Anderson"
}
```

### Scenario 3: Student-Class Relationship ?

**Objective**: Test adding/removing students to/from classes

| Step | Action | Expected Result | Status Code |
|------|--------|----------------|-------------|
| 1 | Create class | Class created | 201 Created |
| 2 | Create student | Student created | 201 Created |
| 3 | Add student to class | Student added successfully | 200 OK |
| 4 | Get class details | Shows student in class | 200 OK |
| 5 | Get student details | Shows class assignment | 200 OK |
| 6 | Remove student from class | Student removed successfully | 200 OK |
| 7 | Get class details | Student not in class | 200 OK |
| 8 | Get student details | No class assignment | 200 OK |

### Scenario 4: Unique Student ID Validation ?

**Objective**: Verify duplicate student ID prevention

| Step | Action | Expected Result | Status Code |
|------|--------|----------------|-------------|
| 1 | Create student S001 | Student created | 201 Created |
| 2 | Create another S001 | Conflict error | 409 Conflict |
| 3 | Delete student S001 | Student deleted | 200 OK |
| 4 | Create student S001 again | Student created | 201 Created |

**Expected Error Message**:
```json
{
  "message": "Student with ID 'S001' already exists."
}
```

### Scenario 5: Class Size Limit (20 Students) ?

**Objective**: Verify maximum 20 students per class

| Step | Action | Expected Result | Status Code |
|------|--------|----------------|-------------|
| 1 | Create class | Class created | 201 Created |
| 2 | Create 20 students | All created | 201 Created |
| 3 | Add all 20 to class | All added successfully | 200 OK |
| 4 | Create 21st student | Student created | 201 Created |
| 5 | Try to add 21st student | Bad request error | 400 Bad Request |

**Expected Error Message**:
```json
{
  "message": "Class 'Math 101' already has the maximum of 20 students."
}
```

### Scenario 6: Required Field Validation ?

**Objective**: Test validation for required fields

#### Student Required Fields

| Field Missing | Expected Result | Status Code |
|--------------|----------------|-------------|
| studentId | Bad request | 400 Bad Request |
| name | Bad request | 400 Bad Request |
| surname | Bad request | 400 Bad Request |
| All required fields | Bad request | 400 Bad Request |

**Test - Missing Name**:
```json
{
  "studentId": "S999",
  "surname": "Test",
  "dateOfBirth": "2005-01-01"
}
```

#### Class Required Fields

| Field Missing | Expected Result | Status Code |
|--------------|----------------|-------------|
| name | Bad request | 400 Bad Request |
| leadingTeacher | Bad request | 400 Bad Request |
| Both fields | Bad request | 400 Bad Request |

### Scenario 7: Optional Fields ?

**Objective**: Verify optional fields work correctly

| Step | Action | Expected Result | Status Code |
|------|--------|----------------|-------------|
| 1 | Create student without city | Student created | 201 Created |
| 2 | Create student without street | Student created | 201 Created |
| 3 | Create student without postal code | Student created | 201 Created |
| 4 | Create student without all optional | Student created | 201 Created |
| 5 | Get student | Optional fields are null | 200 OK |

**Test Data (Minimal)**:
```json
{
  "studentId": "S003",
  "name": "Bob",
  "surname": "Wilson",
  "dateOfBirth": "2005-11-30"
}
```

### Scenario 8: Non-Existent Resource Handling ?

**Objective**: Test 404 error handling

| Action | Expected Result | Status Code |
|--------|----------------|-------------|
| Get student "S999" | Not found error | 404 Not Found |
| Update student "S999" | Not found error | 404 Not Found |
| Delete student "S999" | Not found error | 404 Not Found |
| Get class 999 | Not found error | 404 Not Found |
| Update class 999 | Not found error | 404 Not Found |
| Delete class 999 | Not found error | 404 Not Found |
| Add student to class 999 | Not found error | 404 Not Found |
| Add non-existent student to class | Not found error | 404 Not Found |

### Scenario 9: Class Deletion Cascade ?

**Objective**: Verify students are unassigned when class is deleted

| Step | Action | Expected Result | Status Code |
|------|--------|----------------|-------------|
| 1 | Create class | Class created | 201 Created |
| 2 | Create 5 students | All created | 201 Created |
| 3 | Add all students to class | All added | 200 OK |
| 4 | Get class | Shows 5 students | 200 OK |
| 5 | Delete class | Class deleted | 200 OK |
| 6 | Get each student | All exist, no class assignment | 200 OK |

**Verification**:
All students should have:
```json
{
  "schoolClassId": null,
  "schoolClassName": null
}
```

### Scenario 10: Student Already in Class ?

**Objective**: Test adding student to class twice

| Step | Action | Expected Result | Status Code |
|------|--------|----------------|-------------|
| 1 | Create class and student | Both created | 201 Created |
| 2 | Add student to class | Added successfully | 200 OK |
| 3 | Try to add same student again | Bad request error | 400 Bad Request |

**Expected Error**:
```json
{
  "message": "Student 'John Doe' is already in this class."
}
```

### Scenario 11: Moving Student Between Classes ?

**Objective**: Test moving a student from one class to another

| Step | Action | Expected Result | Status Code |
|------|--------|----------------|-------------|
| 1 | Create 2 classes | Both created | 201 Created |
| 2 | Create student | Student created | 201 Created |
| 3 | Add student to class 1 | Added successfully | 200 OK |
| 4 | Get student | Shows class 1 assignment | 200 OK |
| 5 | Add student to class 2 | Added successfully | 200 OK |
| 6 | Get student | Shows class 2 assignment | 200 OK |

**Note**: Adding a student to a new class automatically moves them.

### Scenario 12: Data Persistence (In-Memory) ??

**Objective**: Verify in-memory database behavior

| Step | Action | Expected Result |
|------|--------|----------------|
| 1 | Create students and classes | Data exists |
| 2 | Stop application | Application stops |
| 3 | Restart application | Database is empty |

**Note**: This is expected behavior with in-memory database.

## ?? Automated Test Script

Here's a bash script to run all basic tests:

```bash
#!/bin/bash

BASE_URL="https://localhost:5001"

echo "?? Running School Management API Tests..."

# Test 1: Create Class
echo -e "\n? Test 1: Create Class"
curl -k -s -X POST $BASE_URL/api/classes \
  -H "Content-Type: application/json" \
  -d '{"name":"Test Class","leadingTeacher":"Mr. Test"}' | jq

# Test 2: Create Student
echo -e "\n? Test 2: Create Student"
curl -k -s -X POST $BASE_URL/api/students \
  -H "Content-Type: application/json" \
  -d '{"studentId":"TEST001","name":"Test","surname":"Student","dateOfBirth":"2005-01-01"}' | jq

# Test 3: Add Student to Class
echo -e "\n? Test 3: Add Student to Class"
curl -k -s -X POST $BASE_URL/api/classes/1/students \
  -H "Content-Type: application/json" \
  -d '{"studentId":"TEST001"}' | jq

# Test 4: Get Class with Students
echo -e "\n? Test 4: Get Class with Students"
curl -k -s $BASE_URL/api/classes/1 | jq

# Test 5: Test Duplicate Student (Should Fail)
echo -e "\n? Test 5: Duplicate Student (Should Fail)"
curl -k -s -X POST $BASE_URL/api/students \
  -H "Content-Type: application/json" \
  -d '{"studentId":"TEST001","name":"Duplicate","surname":"Test","dateOfBirth":"2005-01-01"}' | jq

echo -e "\n? Tests Complete!"
```

Save as `run-tests.sh` and execute:
```bash
chmod +x run-tests.sh
./run-tests.sh
```

## ?? Test Checklist

Use this checklist to verify all functionality:

### Student Operations
- [ ] Create student with all fields
- [ ] Create student with only required fields
- [ ] Get all students
- [ ] Get student by ID
- [ ] Update student
- [ ] Delete student
- [ ] Verify duplicate ID prevention (409)
- [ ] Verify required field validation (400)

### Class Operations
- [ ] Create class
- [ ] Get all classes
- [ ] Get class by ID
- [ ] Update class
- [ ] Delete class
- [ ] Verify required field validation (400)

### Student-Class Relationships
- [ ] Add student to class
- [ ] Remove student from class
- [ ] Move student between classes
- [ ] Verify student in class (GET class shows student)
- [ ] Verify class assignment (GET student shows class)
- [ ] Verify 20-student limit (400)
- [ ] Verify student already in class error (400)

### Error Handling
- [ ] Get non-existent student (404)
- [ ] Get non-existent class (404)
- [ ] Add non-existent student to class (404)
- [ ] Add student to non-existent class (404)
- [ ] Missing required fields (400)
- [ ] Duplicate student ID (409)

### Data Integrity
- [ ] Delete class unassigns students
- [ ] Deleted students removed from class
- [ ] Optional fields can be null
- [ ] Date format handling

## ?? Expected HTTP Status Codes

| Code | Meaning | When Used |
|------|---------|-----------|
| 200 OK | Success | GET, PUT, DELETE successful |
| 201 Created | Resource created | POST successful |
| 400 Bad Request | Invalid input | Validation failed, business rule violation |
| 404 Not Found | Resource missing | Resource doesn't exist |
| 409 Conflict | Duplicate resource | Student ID already exists |

## ?? Test Results Template

| Scenario | Status | Notes |
|----------|--------|-------|
| Student CRUD | ? Pass ? Fail | |
| Class CRUD | ? Pass ? Fail | |
| Student-Class Relationship | ? Pass ? Fail | |
| Unique Student ID | ? Pass ? Fail | |
| 20 Student Limit | ? Pass ? Fail | |
| Required Fields | ? Pass ? Fail | |
| Optional Fields | ? Pass ? Fail | |
| 404 Handling | ? Pass ? Fail | |
| Class Deletion Cascade | ? Pass ? Fail | |
| Duplicate Prevention | ? Pass ? Fail | |

## ?? Debugging Tips

1. **Check Application Logs**: Console output shows request details
2. **Use Swagger UI**: Shows request/response with detailed errors
3. **Verify JSON Format**: Ensure proper quotes and commas
4. **Check Date Format**: Use "YYYY-MM-DD" format
5. **Watch for Case Sensitivity**: Property names are case-sensitive in JSON

## ?? Additional Resources

- **QUICKSTART.md**: Quick examples and curl commands
- **test-requests.http**: Ready-to-use HTTP requests
- **Swagger UI**: Interactive API documentation at `/swagger`
- **ARCHITECTURE.md**: System architecture details

---

**Testing Status**: Ready for comprehensive testing  
**Last Updated**: 2024  
**Version**: 1.0
