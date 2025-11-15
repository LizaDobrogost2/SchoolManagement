namespace SchoolManagement.Models;

/// <summary>
/// Data Transfer Object for partially updating a school class (PATCH operation).
/// </summary>
/// <param name="Name">Name of the class (optional).</param>
/// <param name="LeadingTeacher">Name of the teacher leading the class (optional).</param>
public record PatchSchoolClassDto(
    string? Name,
    string? LeadingTeacher
);
