namespace SchoolManagement.Models;

/// <summary>
/// Data Transfer Object for updating an existing school class (full update with PUT).
/// Note: Class ID cannot be changed - it's part of the route parameter.
/// </summary>
/// <param name="Name">Name of the class (required).</param>
/// <param name="LeadingTeacher">Name of the teacher leading the class (required).</param>
public record UpdateSchoolClassDto(
    string Name,
    string LeadingTeacher
);
