using SchoolManagement.Models;

namespace SchoolManagement.Services;

/// <summary>
/// Service interface for school class business logic operations.
/// Defines the contract for school class-related business operations.
/// </summary>
public interface ISchoolClassService
{
    /// <summary>
    /// Retrieves all school classes from the system.
    /// </summary>
    /// <returns>A collection of all school classes as DTOs.</returns>
    Task<IEnumerable<SchoolClassDto>> GetAllClassesAsync();

    /// <summary>
    /// Retrieves a specific school class by its ID, including all students.
    /// </summary>
    /// <param name="id">The unique identifier of the class.</param>
    /// <returns>A service result containing the class DTO if found, or error information.</returns>
    Task<ServiceResult<SchoolClassDto>> GetClassByIdAsync(int id);

    /// <summary>
    /// Creates a new school class in the system.
    /// Validates required fields.
    /// </summary>
    /// <param name="dto">The class creation data.</param>
    /// <returns>A service result containing the created class DTO or error information.</returns>
    Task<ServiceResult<SchoolClassDto>> CreateClassAsync(CreateSchoolClassDto dto);

    /// <summary>
    /// Updates an existing school class with new information (full update).
    /// All fields must be provided.
    /// </summary>
    /// <param name="id">The unique identifier of the class to update.</param>
    /// <param name="dto">The updated class data.</param>
    /// <returns>A service result containing the updated class DTO or error information.</returns>
    Task<ServiceResult<SchoolClassDto>> UpdateClassAsync(int id, UpdateSchoolClassDto dto);

    /// <summary>
    /// Partially updates a school class (PATCH operation).
    /// Only provided fields will be updated.
    /// </summary>
    /// <param name="id">The unique identifier of the class to update.</param>
    /// <param name="dto">The partial update data (all fields optional).</param>
    /// <returns>A service result containing the updated class DTO or error information.</returns>
    Task<ServiceResult<SchoolClassDto>> PatchClassAsync(int id, PatchSchoolClassDto dto);

    /// <summary>
    /// Deletes a school class from the system.
    /// All students in the class will have their SchoolClassId set to null.
    /// </summary>
    /// <param name="id">The unique identifier of the class to delete.</param>
    /// <returns>A service result containing a success message or error information.</returns>
    Task<ServiceResult<string>> DeleteClassAsync(int id);

    /// <summary>
    /// Adds a student to a school class (legacy endpoint).
    /// Validates class capacity (max 20 students) and student existence.
    /// </summary>
    /// <param name="classId">The ID of the class.</param>
    /// <param name="studentId">The ID of the student to add.</param>
    /// <returns>A service result containing a success message or error information.</returns>
    /// <remarks>
    /// ?? DEPRECATED: For new development, use PatchStudentAsync in IStudentService instead.
    /// This method is kept for backward compatibility.
    /// </remarks>
    Task<ServiceResult<string>> AddStudentToClassAsync(int classId, string studentId);

    /// <summary>
    /// Removes a student from a school class (legacy endpoint).
    /// Sets the student's SchoolClassId to null.
    /// </summary>
    /// <param name="classId">The ID of the class.</param>
    /// <param name="studentId">The ID of the student to remove.</param>
    /// <returns>A service result containing a success message or error information.</returns>
    /// <remarks>
    /// ?? DEPRECATED: For new development, use PatchStudentAsync in IStudentService instead.
    /// This method is kept for backward compatibility.
    /// </remarks>
    Task<ServiceResult<string>> RemoveStudentFromClassAsync(int classId, string studentId);
}
