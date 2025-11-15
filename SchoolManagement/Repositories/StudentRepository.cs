using Microsoft.EntityFrameworkCore;
using SchoolManagement.Data;
using SchoolManagement.Models;

namespace SchoolManagement.Repositories;

/// <summary>
/// Repository implementation for Student entity data access operations.
/// Handles all database interactions for students using Entity Framework Core.
/// </summary>
public class StudentRepository : IStudentRepository
{
    private readonly SchoolDbContext _context;

    /// <summary>
    /// Initializes a new instance of the StudentRepository.
    /// </summary>
    /// <param name="context">The database context.</param>
    public StudentRepository(SchoolDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Student>> GetAllAsync()
    {
        return await _context.Students
            .Include(s => s.SchoolClass)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<Student?> GetByIdAsync(string studentId)
    {
        return await _context.Students
            .Include(s => s.SchoolClass)
            .FirstOrDefaultAsync(s => s.StudentId == studentId);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(string studentId)
    {
        return await _context.Students.AnyAsync(s => s.StudentId == studentId);
    }

    /// <inheritdoc />
    public async Task<Student> AddAsync(Student student)
    {
        _context.Students.Add(student);
        await _context.SaveChangesAsync();
        return student;
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Student student)
    {
        _context.Students.Update(student);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Student student)
    {
        _context.Students.Remove(student);
        await _context.SaveChangesAsync();
    }
}
