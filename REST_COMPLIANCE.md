# REST Compliance Guide - School Management API

## ?? REST Maturity Level

This API now achieves **Level 2** of the Richardson Maturity Model with improvements toward **Level 3**.

### Richardson Maturity Model:
- ? **Level 0**: Single URI, single HTTP method - Not applicable
- ? **Level 1**: Multiple URIs, resources - Implemented
- ? **Level 2**: HTTP verbs, status codes - Fully implemented
- ?? **Level 3**: HATEOAS - Partially implemented

---

## ? REST Improvements Implemented

### 1. **PATCH for Partial Updates** ? NEW

Following REST principles, PATCH is now used for partial resource updates.

#### Students
```http
PATCH /api/students/S001
Content-Type: application/json

{
  "city": "Los Angeles"
}
```

**Benefits:**
- ? Only send fields that need updating
- ? More efficient than PUT
- ? True to REST semantics

#### Classes
```http
PATCH /api/classes/1
Content-Type: application/json

{
  "leadingTeacher": "Dr. Smith"
}
```

---

### 2. **Resource-Based Class Assignment** ? RECOMMENDED

Instead of managing students through class endpoints, we now update the student resource directly.

#### ? Old Way (Deprecated but still supported)
```http
POST /api/classes/1/students
{
  "studentId": "S001"
}
```

**Issues:**
- URL suggests modifying class resource
- Actually modifies student resource
- Not pure REST

#### ? New RESTful Way (Recommended)
```http
PATCH /api/students/S001
{
  "schoolClassId": 1
}
```

**Benefits:**
- ? URL reflects what's being modified (student)
- ? Clear intent - updating student property
- ? Pure REST principles
- ? Consistent with other updates

---

### 3. **Proper HTTP Methods**

| Method | Purpose | Idempotent | Safe |
|--------|---------|------------|------|
| GET | Retrieve resource(s) | ? Yes | ? Yes |
| POST | Create new resource | ? No | ? No |
| PUT | Replace entire resource | ? Yes | ? No |
| PATCH | Update part of resource | ? No | ? No |
| DELETE | Remove resource | ? Yes | ? No |

---

## ?? RESTful Patterns in This API

### Pattern 1: Resource URLs (Nouns, Not Verbs)

? **Good:**
```
GET    /api/students
GET    /api/students/{id}
POST   /api/students
PUT    /api/students/{id}
PATCH  /api/students/{id}
DELETE /api/students/{id}
```

? **Bad (not implemented):**
```
POST   /api/students/create
POST   /api/students/update
POST   /api/students/delete
```

---

### Pattern 2: HTTP Status Codes

| Code | Usage | Example |
|------|-------|---------|
| 200 OK | Successful GET, PUT, PATCH, DELETE | Resource retrieved/updated |
| 201 Created | Successful POST | Resource created |
| 400 Bad Request | Validation failed | Missing required fields |
| 404 Not Found | Resource doesn't exist | Student ID not found |
| 409 Conflict | Duplicate resource | Student ID already exists |

---

### Pattern 3: Consistent Response Formats

#### Success (Resource)
```json
{
  "studentId": "S001",
  "name": "John",
  "surname": "Doe",
  "dateOfBirth": "2005-05-15T00:00:00",
  "city": "New York",
  "schoolClassId": 1,
  "schoolClassName": "Class 5A"
}
```

#### Error
```json
{
  "message": "Student with ID 'S001' not found."
}
```

---

## ?? Usage Examples

### Scenario 1: Assign Student to Class

#### RESTful Way ? RECOMMENDED
```bash
# Assign student to class
PATCH /api/students/S001
{
  "schoolClassId": 1
}

# Response: Full student object with updated class
{
  "studentId": "S001",
  "schoolClassId": 1,
  "schoolClassName": "Class 5A",
  ...
}
```

#### Legacy Way (Still Supported)
```bash
# Add student to class
POST /api/classes/1/students
{
  "studentId": "S001"
}

# Response: Message only
{
  "message": "Student 'John Doe' has been added to class 'Class 5A'."
}
```

**Why RESTful is better:**
1. URL reflects the resource being modified (student)
2. Returns the updated resource state
3. Consistent with other PATCH operations
4. Can update multiple fields in one request

---

### Scenario 2: Move Student Between Classes

#### RESTful Way ? RECOMMENDED
```bash
# Single operation - automatically removes from old class
PATCH /api/students/S001
{
  "schoolClassId": 2
}
```

#### Legacy Way (Requires 2 Requests)
```bash
# Step 1: Remove from old class
DELETE /api/classes/1/students/S001

# Step 2: Add to new class
POST /api/classes/2/students
{
  "studentId": "S001"
}
```

**RESTful advantage:** Single atomic operation

---

### Scenario 3: Unassign Student from Class

#### RESTful Way ? RECOMMENDED
```bash
PATCH /api/students/S001
{
  "schoolClassId": null
}

# Or use 0 as alternative
PATCH /api/students/S001
{
  "schoolClassId": 0
}
```

#### Legacy Way
```bash
DELETE /api/classes/1/students/S001
```

---

### Scenario 4: Update Multiple Student Fields

#### With PATCH ? NEW
```bash
# Update city and class assignment in one request
PATCH /api/students/S001
{
  "city": "Los Angeles",
  "schoolClassId": 2
}
```

#### With PUT (Old Way)
```bash
# Must provide ALL fields
PUT /api/students/S001
{
  "name": "John",
  "surname": "Doe",
  "dateOfBirth": "2005-05-15",
  "city": "Los Angeles",
  "street": "123 Main St",
  "postalCode": "10001"
}

# Then assign class separately
POST /api/classes/2/students
{
  "studentId": "S001"
}
```

**PATCH advantage:** One request, only changed fields

---

## ?? Migration Guide

### For Existing Clients

The API maintains **backward compatibility**. Old endpoints still work:

```bash
# These still work (marked as deprecated in Swagger)
POST   /api/classes/{id}/students
DELETE /api/classes/{id}/students/{studentId}
```

### Recommended Migration Path

1. **Phase 1** - Start using PATCH for new features
2. **Phase 2** - Update existing code to use PATCH
3. **Phase 3** - Remove usage of deprecated endpoints

### Code Examples

#### Before
```javascript
// Old way - add student to class
await fetch('/api/classes/1/students', {
  method: 'POST',
  body: JSON.stringify({ studentId: 'S001' })
});
```

#### After
```javascript
// New way - update student with class assignment
await fetch('/api/students/S001', {
  method: 'PATCH',
  body: JSON.stringify({ schoolClassId: 1 })
});
```

---

## ?? REST Compliance Checklist

### Resource Design
- [x] URLs represent resources (nouns)
- [x] No verbs in URLs
- [x] Plural nouns for collections
- [x] Consistent naming

### HTTP Methods
- [x] GET for retrieval
- [x] POST for creation
- [x] PUT for full updates
- [x] PATCH for partial updates ? NEW
- [x] DELETE for removal

### Status Codes
- [x] 200 OK for successful operations
- [x] 201 Created for POST success
- [x] 400 Bad Request for validation errors
- [x] 404 Not Found for missing resources
- [x] 409 Conflict for duplicates

### Response Design
- [x] Return resources on success
- [x] Consistent error format
- [x] Appropriate content-type headers
- [ ] HATEOAS links (future enhancement)

### Idempotency
- [x] GET is idempotent and safe
- [x] PUT is idempotent
- [x] DELETE is idempotent
- [x] POST is not idempotent
- [x] PATCH is not idempotent

---

## ?? REST Principles Applied

### 1. **Uniform Interface**
- Standard HTTP methods (GET, POST, PUT, PATCH, DELETE)
- Predictable URL patterns
- Consistent response formats

### 2. **Stateless**
- Each request contains all necessary information
- No server-side session state
- Authentication tokens (if added) in headers

### 3. **Resource-Based**
- URLs identify resources: `/api/students/S001`
- Resources represented as JSON
- Relationships reflected in data structure

### 4. **Self-Descriptive Messages**
- HTTP headers indicate content type
- Status codes convey outcome
- Error messages explain failures

---

## ?? Future REST Enhancements

### Level 3: HATEOAS (Planned)

Add hypermedia links to responses:

```json
{
  "studentId": "S001",
  "name": "John",
  "surname": "Doe",
  "schoolClassId": 1,
  "_links": {
    "self": { "href": "/api/students/S001" },
    "class": { "href": "/api/classes/1" },
    "update": { "href": "/api/students/S001", "method": "PATCH" },
    "delete": { "href": "/api/students/S001", "method": "DELETE" }
  }
}
```

### Additional Enhancements

1. **ETags for Concurrency Control**
```http
GET /api/students/S001
ETag: "v1"

PUT /api/students/S001
If-Match: "v1"
```

2. **Query Parameters for Filtering**
```http
GET /api/students?classId=1
GET /api/students?name=John&surname=Doe
```

3. **Pagination**
```http
GET /api/students?page=1&pageSize=20
Link: </api/students?page=2>; rel="next"
```

4. **OPTIONS Method**
```http
OPTIONS /api/students/S001
Allow: GET, PUT, PATCH, DELETE
```

---

## ?? References

- [Richardson Maturity Model](https://martinfowler.com/articles/richardsonMaturityModel.html)
- [RFC 7231 - HTTP/1.1 Semantics](https://tools.ietf.org/html/rfc7231)
- [RFC 5789 - PATCH Method](https://tools.ietf.org/html/rfc5789)
- [REST API Design Best Practices](https://restfulapi.net/)

---

## ? Summary

### What Changed
- ? Added PATCH endpoints for partial updates
- ? RESTful student-class assignment via PATCH
- ? Deprecated old class-based student endpoints
- ? Maintained backward compatibility

### What Stayed the Same
- ? All existing endpoints still work
- ? No breaking changes
- ? Same authentication requirements (none)
- ? Same data models

### Best Practices to Follow
- ? Use PATCH for partial updates
- ? Use PATCH to assign students to classes
- ? Return updated resources from operations
- ? Use appropriate HTTP status codes
- ? Keep URLs resource-oriented

---

**REST Level**: 2+ (Industry Standard)  
**Backward Compatible**: Yes ?  
**Migration Required**: Optional (Recommended)
