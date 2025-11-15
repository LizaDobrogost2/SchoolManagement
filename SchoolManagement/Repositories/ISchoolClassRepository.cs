using SchoolManagement.Models;

namespace SchoolManagement.Repositories;

/// <summary>
/// Repository interface for SchoolClass entity data access operations.
/// </summary>
public interface ISchoolClassRepository
{
    /// <summary>
    /// Retrieves all school classes with their student lists.
    /// </summary>
    /// <returns>A collection of all school classes.</returns>
    Task<IEnumerable<SchoolClass>> GetAllAsync();

    /// <summary>
    /// Retrieves a school class by its ID (without students).
    /// </summary>
    /// <param name="id">The unique identifier of the class.</param>
    /// <returns>The school class if found, otherwise null.</returns>
    Task<SchoolClass?> GetByIdAsync(int id);

    /// <summary>
    /// Retrieves a school class by its ID, including all assigned students.
    /// </summary>
    /// <param name="id">The unique identifier of the class.</param>
    /// <returns>The school class with students if found, otherwise null.</returns>
    Task<SchoolClass?> GetByIdWithStudentsAsync(int id);

    /// <summary>
    /// Adds a new school class to the database.
    /// </summary>
    /// <param name="schoolClass">The school class entity to add.</param>
    /// <returns>The added school class with any database-generated values (like ID).</returns>
    Task<SchoolClass> AddAsync(SchoolClass schoolClass);

    /// <summary>
    /// Updates an existing school class in the database.
    /// </summary>
    /// <param name="schoolClass">The school class entity with updated values.</param>
    Task UpdateAsync(SchoolClass schoolClass);

    /// <summary>
    /// Removes a school class from the database.
    /// </summary>
    /// <param name="schoolClass">The school class entity to remove.</param>
    Task DeleteAsync(SchoolClass schoolClass);

    /// <summary>
    /// Gets the count of students currently assigned to a specific class.
    /// Used for validating the 20-student limit per class.
    /// </summary>
    /// <param name="classId">The unique identifier of the class.</param>
    /// <returns>The number of students assigned to this class.</returns>
    Task<int> GetStudentCountAsync(int classId);
}
