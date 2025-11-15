using SchoolManagement.Models;

namespace SchoolManagement.Services;

/// <summary>
/// Service interface for student business logic operations.
/// Defines the contract for student-related business operations.
/// </summary>
public interface IStudentService
{
    /// <summary>
    /// Retrieves all students from the system.
    /// </summary>
    /// <returns>A collection of all students as DTOs.</returns>
    Task<IEnumerable<StudentDto>> GetAllStudentsAsync();

    /// <summary>
    /// Retrieves a specific student by their ID.
    /// </summary>
    /// <param name="studentId">The unique identifier of the student.</param>
    /// <returns>A service result containing the student DTO if found, or error information.</returns>
    Task<ServiceResult<StudentDto>> GetStudentByIdAsync(string studentId);

    /// <summary>
    /// Creates a new student in the system.
    /// Validates required fields and checks for duplicate student IDs.
    /// </summary>
    /// <param name="dto">The student creation data.</param>
    /// <returns>A service result containing the created student DTO or error information.</returns>
    Task<ServiceResult<StudentDto>> CreateStudentAsync(CreateStudentDto dto);

    /// <summary>
    /// Updates an existing student with new information (full update).
    /// All fields except StudentId must be provided.
    /// </summary>
    /// <param name="studentId">The unique identifier of the student to update.</param>
    /// <param name="dto">The updated student data.</param>
    /// <returns>A service result containing the updated student DTO or error information.</returns>
    Task<ServiceResult<StudentDto>> UpdateStudentAsync(string studentId, UpdateStudentDto dto);

    /// <summary>
    /// Partially updates a student (PATCH operation).
    /// Only provided fields will be updated. This is the RESTful way to assign students to classes.
    /// </summary>
    /// <param name="studentId">The unique identifier of the student to update.</param>
    /// <param name="dto">The partial update data (all fields optional).</param>
    /// <returns>A service result containing the updated student DTO or error information.</returns>
    Task<ServiceResult<StudentDto>> PatchStudentAsync(string studentId, PatchStudentDto dto);

    /// <summary>
    /// Deletes a student from the system.
    /// The student will be automatically removed from their class if assigned.
    /// </summary>
    /// <param name="studentId">The unique identifier of the student to delete.</param>
    /// <returns>A service result containing a success message or error information.</returns>
    Task<ServiceResult<string>> DeleteStudentAsync(string studentId);
}
