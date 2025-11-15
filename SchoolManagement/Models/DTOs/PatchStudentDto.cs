namespace SchoolManagement.Models;

/// <summary>
/// Data Transfer Object for partially updating a student (PATCH operation).
/// </summary>
/// <param name="Name">Student's first name (optional).</param>
/// <param name="Surname">Student's last name (optional).</param>
/// <param name="DateOfBirth">Student's date of birth (optional).</param>
/// <param name="City">Student's city of residence (optional).</param>
/// <param name="Street">Student's street address (optional).</param>
/// <param name="PostalCode">Student's postal code (optional).</param>
/// <param name="SchoolClassId">ID of the class to assign student to, or null to unassign (optional).</param>
public record PatchStudentDto(
    string? Name,
    string? Surname,
    DateTime? DateOfBirth,
    string? City,
    string? Street,
    string? PostalCode,
    int? SchoolClassId
);
