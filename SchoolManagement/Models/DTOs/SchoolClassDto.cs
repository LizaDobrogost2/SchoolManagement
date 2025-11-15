namespace SchoolManagement.Models;

/// <summary>
/// Data Transfer Object for returning school class information from the API.
/// </summary>
/// <param name="Id">Unique identifier for the class (auto-generated).</param>
/// <param name="Name">Name of the class.</param>
/// <param name="LeadingTeacher">Name of the teacher leading the class.</param>
/// <param name="StudentCount">Number of students currently assigned to this class (max 20).</param>
/// <param name="Students">List of students assigned to this class.</param>
public record SchoolClassDto(
    int Id,
    string Name,
    string LeadingTeacher,
    int StudentCount,
    List<StudentDto> Students
);
