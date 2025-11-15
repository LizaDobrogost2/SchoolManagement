# Quick Start Guide - School Management API

## ?? Run the Application

```bash
cd SchoolManagement
dotnet run
```

The API will be available at:
- **HTTPS**: `https://localhost:5001`
- **HTTP**: `http://localhost:5000`

### Access Swagger UI
Open your browser and navigate to:
```
https://localhost:5001/swagger
```

## ?? Complete Test Scenario

Follow this step-by-step guide to test all features:

### Step 1: Create School Classes

```bash
# Create Class 1
curl -k -X POST https://localhost:5001/api/classes \
  -H "Content-Type: application/json" \
  -d '{"name":"Class 5A","leadingTeacher":"Mrs. Smith"}'

# Create Class 2
curl -k -X POST https://localhost:5001/api/classes \
  -H "Content-Type: application/json" \
  -d '{"name":"Class 6B","leadingTeacher":"Mr. Johnson"}'
```

**Expected Response (Class 1):**
```json
{
  "id": 1,
  "name": "Class 5A",
  "leadingTeacher": "Mrs. Smith",
  "studentCount": 0,
  "students": []
}
```

### Step 2: Create Students

```bash
# Student 1
curl -k -X POST https://localhost:5001/api/students \
  -H "Content-Type: application/json" \
  -d '{"studentId":"S001","name":"John","surname":"Doe","dateOfBirth":"2005-05-15","city":"New York","street":"123 Main St","postalCode":"10001"}'

# Student 2
curl -k -X POST https://localhost:5001/api/students \
  -H "Content-Type: application/json" \
  -d '{"studentId":"S002","name":"Jane","surname":"Smith","dateOfBirth":"2006-03-20","city":"Boston"}'

# Student 3 (minimal - only required fields)
curl -k -X POST https://localhost:5001/api/students \
  -H "Content-Type: application/json" \
  -d '{"studentId":"S003","name":"Bob","surname":"Wilson","dateOfBirth":"2005-11-30"}'
```

**Expected Response (Student 1):**
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

### Step 3: Add Students to Class

```bash
# Add Student 1 to Class 1
curl -k -X POST https://localhost:5001/api/classes/1/students \
  -H "Content-Type: application/json" \
  -d '{"studentId":"S001"}'

# Add Student 2 to Class 1
curl -k -X POST https://localhost:5001/api/classes/1/students \
  -H "Content-Type: application/json" \
  -d '{"studentId":"S002"}'

# Add Student 3 to Class 2
curl -k -X POST https://localhost:5001/api/classes/2/students \
  -H "Content-Type: application/json" \
  -d '{"studentId":"S003"}'
```

**Expected Response:**
```json
{
  "message": "Student 'John Doe' has been added to class 'Class 5A'."
}
```

### Step 4: View Class with Students

```bash
curl -k https://localhost:5001/api/classes/1
```

**Expected Response:**
```json
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
    },
    {
      "studentId": "S002",
      "name": "Jane",
      "surname": "Smith",
      "dateOfBirth": "2006-03-20T00:00:00",
      "city": "Boston",
      "street": null,
      "postalCode": null,
      "schoolClassId": 1,
      "schoolClassName": "Class 5A"
    }
  ]
}
```

### Step 5: List All Students

```bash
curl -k https://localhost:5001/api/students
```

### Step 6: List All Classes

```bash
curl -k https://localhost:5001/api/classes
```

### Step 7: Update a Student

```bash
curl -k -X PUT https://localhost:5001/api/students/S001 \
  -H "Content-Type: application/json" \
  -d '{"name":"John Updated","surname":"Doe","dateOfBirth":"2005-05-15","city":"Los Angeles","street":"456 Oak Ave","postalCode":"90001"}'
```

**Expected Response:**
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

### Step 8: Update a Class

```bash
curl -k -X PUT https://localhost:5001/api/classes/1 \
  -H "Content-Type: application/json" \
  -d '{"name":"Class 5A Advanced","leadingTeacher":"Dr. Smith"}'
```

### Step 9: Remove Student from Class

```bash
curl -k -X DELETE https://localhost:5001/api/classes/1/students/S002
```

**Expected Response:**
```json
{
  "message": "Student 'Jane Smith' has been removed from class 'Class 5A Advanced'."
}
```

### Step 10: Delete a Student

```bash
curl -k -X DELETE https://localhost:5001/api/students/S003
```

**Expected Response:**
```json
{
  "message": "Student with ID 'S003' has been deleted."
}
```

### Step 11: Delete a Class

```bash
curl -k -X DELETE https://localhost:5001/api/classes/2
```

**Expected Response:**
```json
{
  "message": "School class with ID 2 has been deleted."
}
```

## ?? Test Business Rules & Validations

### Test 1: Duplicate Student ID (Should Fail)
```bash
# Try to create a student with existing ID
curl -k -X POST https://localhost:5001/api/students \
  -H "Content-Type: application/json" \
  -d '{"studentId":"S001","name":"Another","surname":"Person","dateOfBirth":"2005-01-01"}'
```

**Expected Response (409 Conflict):**
```json
{
  "message": "Student with ID 'S001' already exists."
}
```

### Test 2: Missing Required Fields (Should Fail)
```bash
# Try to create student without name
curl -k -X POST https://localhost:5001/api/students \
  -H "Content-Type: application/json" \
  -d '{"studentId":"S999","surname":"Test","dateOfBirth":"2005-01-01"}'
```

**Expected Response (400 Bad Request):**
```json
{
  "message": "StudentId is required. Name is required. Surname is required."
}
```

### Test 3: Class Size Limit (Should Fail After 20 Students)
```bash
# First, create and add 20 students to a class
# Then try to add the 21st student:

curl -k -X POST https://localhost:5001/api/students \
  -H "Content-Type: application/json" \
  -d '{"studentId":"S021","name":"Extra","surname":"Student","dateOfBirth":"2005-01-01"}'

curl -k -X POST https://localhost:5001/api/classes/1/students \
  -H "Content-Type: application/json" \
  -d '{"studentId":"S021"}'
```

**Expected Response (400 Bad Request):**
```json
{
  "message": "Class 'Class 5A Advanced' already has the maximum of 20 students."
}
```

### Test 4: Non-Existent Student (Should Fail)
```bash
curl -k https://localhost:5001/api/students/S999
```

**Expected Response (404 Not Found):**
```json
{
  "message": "Student with ID 'S999' not found."
}
```

### Test 5: Add Non-Existent Student to Class (Should Fail)
```bash
curl -k -X POST https://localhost:5001/api/classes/1/students \
  -H "Content-Type: application/json" \
  -d '{"studentId":"S999"}'
```

**Expected Response (404 Not Found):**
```json
{
  "message": "Student with ID 'S999' not found."
}
```

## ?? Windows PowerShell Version

If you're using PowerShell on Windows:

```powershell
# Create a class
Invoke-RestMethod -Uri "https://localhost:5001/api/classes" `
  -Method POST `
  -ContentType "application/json" `
  -Body '{"name":"Class 5A","leadingTeacher":"Mrs. Smith"}' `
  -SkipCertificateCheck

# Create a student
Invoke-RestMethod -Uri "https://localhost:5001/api/students" `
  -Method POST `
  -ContentType "application/json" `
  -Body '{"studentId":"S001","name":"John","surname":"Doe","dateOfBirth":"2005-05-15"}' `
  -SkipCertificateCheck

# Get all students
Invoke-RestMethod -Uri "https://localhost:5001/api/students" `
  -Method GET `
  -SkipCertificateCheck

# Get specific student
Invoke-RestMethod -Uri "https://localhost:5001/api/students/S001" `
  -Method GET `
  -SkipCertificateCheck

# Update student
Invoke-RestMethod -Uri "https://localhost:5001/api/students/S001" `
  -Method PUT `
  -ContentType "application/json" `
  -Body '{"name":"John Updated","surname":"Doe","dateOfBirth":"2005-05-15"}' `
  -SkipCertificateCheck

# Add student to class
Invoke-RestMethod -Uri "https://localhost:5001/api/classes/1/students" `
  -Method POST `
  -ContentType "application/json" `
  -Body '{"studentId":"S001"}' `
  -SkipCertificateCheck

# Remove student from class
Invoke-RestMethod -Uri "https://localhost:5001/api/classes/1/students/S001" `
  -Method DELETE `
  -SkipCertificateCheck

# Delete student
Invoke-RestMethod -Uri "https://localhost:5001/api/students/S001" `
  -Method DELETE `
  -SkipCertificateCheck
```

## ?? Using .http Files (VS Code REST Client)

Create a file named `test-requests.http`:

```http
### Variables
@baseUrl = https://localhost:5001
@contentType = application/json

### Create a Class
POST {{baseUrl}}/api/classes
Content-Type: {{contentType}}

{
  "name": "Class 5A",
  "leadingTeacher": "Mrs. Smith"
}

### Create a Student
POST {{baseUrl}}/api/students
Content-Type: {{contentType}}

{
  "studentId": "S001",
  "name": "John",
  "surname": "Doe",
  "dateOfBirth": "2005-05-15",
  "city": "New York"
}

### Get All Students
GET {{baseUrl}}/api/students

### Get Student by ID
GET {{baseUrl}}/api/students/S001

### Add Student to Class
POST {{baseUrl}}/api/classes/1/students
Content-Type: {{contentType}}

{
  "studentId": "S001"
}

### Get Class with Students
GET {{baseUrl}}/api/classes/1

### Update Student
PUT {{baseUrl}}/api/students/S001
Content-Type: {{contentType}}

{
  "name": "John Updated",
  "surname": "Doe",
  "dateOfBirth": "2005-05-15",
  "city": "Los Angeles"
}

### Remove Student from Class
DELETE {{baseUrl}}/api/classes/1/students/S001

### Delete Student
DELETE {{baseUrl}}/api/students/S001

### Delete Class
DELETE {{baseUrl}}/api/classes/1
```

Then click "Send Request" above each request block.

## ?? Using Swagger UI (Recommended for Beginners)

1. Start the application: `dotnet run`
2. Open browser: `https://localhost:5001/swagger`
3. You'll see an interactive API documentation
4. Click on any endpoint to expand it
5. Click "Try it out" button
6. Fill in the request parameters/body
7. Click "Execute"
8. View the response below

## ?? Sample Test Data

### Students (Copy-Paste Ready)
```json
{"studentId":"S001","name":"Alice","surname":"Johnson","dateOfBirth":"2005-01-15","city":"New York","street":"123 Main St","postalCode":"10001"}
{"studentId":"S002","name":"Bob","surname":"Williams","dateOfBirth":"2006-05-20","city":"Boston","street":"456 Oak Ave","postalCode":"02101"}
{"studentId":"S003","name":"Charlie","surname":"Brown","dateOfBirth":"2005-11-30","city":"Chicago","street":"789 Pine Rd","postalCode":"60601"}
{"studentId":"S004","name":"Diana","surname":"Davis","dateOfBirth":"2006-02-14","city":"Seattle"}
{"studentId":"S005","name":"Ethan","surname":"Miller","dateOfBirth":"2005-07-22"}
```

### Classes (Copy-Paste Ready)
```json
{"name":"Mathematics 101","leadingTeacher":"Prof. Anderson"}
{"name":"Physics 201","leadingTeacher":"Dr. Martinez"}
{"name":"Chemistry 101","leadingTeacher":"Prof. Lee"}
{"name":"Biology 101","leadingTeacher":"Dr. Taylor"}
```

## ?? Troubleshooting

### Port Already in Use
If port 5001 is already in use, modify `Properties/launchSettings.json`:
```json
"applicationUrl": "https://localhost:5002;http://localhost:5003"
```

### SSL Certificate Warning
The `-k` flag in curl commands skips SSL certificate verification. This is normal for development.

### JSON Formatting Issues
Make sure your JSON is properly formatted:
- Use double quotes for property names
- Use double quotes for string values
- Date format: "YYYY-MM-DD"

## ?? API Endpoint Reference

| Method | Endpoint | Body Required | Description |
|--------|----------|---------------|-------------|
| GET | `/api/students` | No | Get all students |
| GET | `/api/students/{id}` | No | Get student by ID |
| POST | `/api/students` | Yes | Create student |
| PUT | `/api/students/{id}` | Yes | Update student |
| DELETE | `/api/students/{id}` | No | Delete student |
| GET | `/api/classes` | No | Get all classes |
| GET | `/api/classes/{id}` | No | Get class by ID |
| POST | `/api/classes` | Yes | Create class |
| PUT | `/api/classes/{id}` | Yes | Update class |
| DELETE | `/api/classes/{id}` | No | Delete class |
| POST | `/api/classes/{classId}/students` | Yes | Add student to class |
| DELETE | `/api/classes/{classId}/students/{studentId}` | No | Remove student from class |

## ?? Important Notes

- ?? All data is stored **in-memory** and will be lost when the application stops
- ? Student ID must be unique across all students
- ? Each class can have a maximum of **20 students**
- ? Deleting a class will **unassign** all students (sets `schoolClassId` to `null`)
- ? Students can exist without being assigned to any class
- ? Optional fields (`city`, `street`, `postalCode`) can be `null` or omitted

## ?? Quick Testing Checklist

- [ ] Create a class
- [ ] Create 3 students
- [ ] Add 2 students to the class
- [ ] View class with students
- [ ] Update a student's information
- [ ] Remove a student from the class
- [ ] Try to create a duplicate student ID (should fail)
- [ ] Try to add 21st student to a class (should fail)
- [ ] Delete a student
- [ ] Delete a class
- [ ] View all students
- [ ] View all classes

---

**Happy Testing! ??**

For more details, see:
- **SchoolManagement/PROJECT_OVERVIEW.md** - Business requirements
- **ARCHITECTURE.md** - Architecture details
- **README.md** - Complete documentation
