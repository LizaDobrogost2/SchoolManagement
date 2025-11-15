namespace SchoolManagement.Models;

/// <summary>
/// Data Transfer Object for returning student information from the API.
/// </summary>
/// <param name="StudentId">Unique identifier for the student.</param>
/// <param name="Name">Student's first name.</param>
/// <param name="Surname">Student's last name.</param>
/// <param name="DateOfBirth">Student's date of birth.</param>
/// <param name="City">Student's city of residence (may be null).</param>
/// <param name="Street">Student's street address (may be null).</param>
/// <param name="PostalCode">Student's postal code (may be null).</param>
/// <param name="SchoolClassId">ID of the assigned class (null if not assigned to any class).</param>
/// <param name="SchoolClassName">Name of the assigned class (null if not assigned to any class).</param>
public record StudentDto(
    string StudentId,
    string Name,
    string Surname,
    DateTime DateOfBirth,
    string? City,
    string? Street,
    string? PostalCode,
    int? SchoolClassId,
    string? SchoolClassName
);
