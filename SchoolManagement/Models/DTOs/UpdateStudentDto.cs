namespace SchoolManagement.Models;

/// <summary>
/// Data Transfer Object for updating an existing student (full update with PUT).
/// Note: StudentId cannot be changed - it's part of the route parameter.
/// </summary>
/// <param name="Name">Student's first name (required).</param>
/// <param name="Surname">Student's last name (required).</param>
/// <param name="DateOfBirth">Student's date of birth (required).</param>
/// <param name="City">Student's city of residence (optional).</param>
/// <param name="Street">Student's street address (optional).</param>
/// <param name="PostalCode">Student's postal code (optional).</param>
public record UpdateStudentDto(
    string Name,
    string Surname,
    DateTime DateOfBirth,
    string? City,
    string? Street,
    string? PostalCode
);
