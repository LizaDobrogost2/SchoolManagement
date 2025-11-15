namespace SchoolManagement.Models;

/// <summary>
/// Data Transfer Object for creating a new school class.
/// </summary>
/// <param name="Name">Name of the class (required).</param>
/// <param name="LeadingTeacher">Name of the teacher leading the class (required).</param>
public record CreateSchoolClassDto(
    string Name,
    string LeadingTeacher
);
