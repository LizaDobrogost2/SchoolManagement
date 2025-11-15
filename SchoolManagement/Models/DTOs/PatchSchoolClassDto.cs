namespace SchoolManagement.Models;

/// <summary>
/// Data Transfer Object for partially updating a school class (PATCH operation).
/// All fields are optional - only provided fields will be updated.
/// Use this for efficient updates when you only need to change specific fields.
/// </summary>
/// <param name="Name">Name of the class (optional).</param>
/// <param name="LeadingTeacher">Name of the teacher leading the class (optional).</param>
/// <remarks>
/// Example: PATCH /api/classes/1 with {"name": "Advanced Mathematics"} to only update the class name.
/// Example: PATCH /api/classes/1 with {"leadingTeacher": "Dr. Smith"} to only update the teacher.
/// </remarks>
public record PatchSchoolClassDto(
    string? Name,
    string? LeadingTeacher
);
