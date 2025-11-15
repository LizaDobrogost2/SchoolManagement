namespace SchoolManagement.Models;

/// <summary>
/// Data Transfer Object for updating an existing school class.
/// </summary>
/// <param name="Name">Name of the class (required).</param>
/// <param name="LeadingTeacher">Name of the teacher leading the class (required).</param>
public record UpdateSchoolClassDto(
    string Name,
    string LeadingTeacher
);
