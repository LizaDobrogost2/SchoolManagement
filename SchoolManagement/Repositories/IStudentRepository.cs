using SchoolManagement.Models;

namespace SchoolManagement.Repositories;

/// <summary>
/// Repository interface for Student entity data access operations.
/// </summary>
public interface IStudentRepository
{
    /// <summary>
    /// Retrieves all students with their class information.
    /// </summary>
    /// <returns>A collection of all students.</returns>
    Task<IEnumerable<Student>> GetAllAsync();

    /// <summary>
    /// Retrieves a student by their unique ID, including class information.
    /// </summary>
    /// <param name="studentId">The unique identifier of the student.</param>
    /// <returns>The student if found, otherwise null.</returns>
    Task<Student?> GetByIdAsync(string studentId);

    /// <summary>
    /// Checks if a student with the given ID exists.
    /// </summary>
    /// <param name="studentId">The student ID to check.</param>
    /// <returns>True if a student with this ID exists, otherwise false.</returns>
    Task<bool> ExistsAsync(string studentId);

    /// <summary>
    /// Adds a new student to the database.
    /// </summary>
    /// <param name="student">The student entity to add.</param>
    /// <returns>The added student with any database-generated values.</returns>
    Task<Student> AddAsync(Student student);

    /// <summary>
    /// Updates an existing student in the database.
    /// </summary>
    /// <param name="student">The student entity with updated values.</param>
    Task UpdateAsync(Student student);

    /// <summary>
    /// Removes a student from the database.
    /// </summary>
    /// <param name="student">The student entity to remove.</param>
    Task DeleteAsync(Student student);
}
