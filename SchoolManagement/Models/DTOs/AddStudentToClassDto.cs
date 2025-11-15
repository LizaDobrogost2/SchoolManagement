namespace SchoolManagement.Models;

/// <summary>
/// Data Transfer Object for adding a student to a class (legacy endpoint).
/// </summary>
/// <param name="StudentId">ID of the student to add to the class.</param>
/// <remarks>
/// ?? DEPRECATED: This is used by the legacy endpoint POST /api/classes/{id}/students.
/// For new development, use the RESTful approach:
/// PATCH /api/students/{id} with {"schoolClassId": classId}
/// </remarks>
public record AddStudentToClassDto(
    string StudentId
);
