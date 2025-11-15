namespace SchoolManagement.Models;

/// <summary>
/// Data Transfer Object for creating a new school class.
/// </summary>
/// <param name="Name">Name of the class (required). Example: "Class 5A", "Mathematics 101".</param>
/// <param name="LeadingTeacher">Name of the teacher leading the class (required). Example: "Mrs. Smith".</param>
public record CreateSchoolClassDto(
    string Name,
    string LeadingTeacher
);
