using Microsoft.EntityFrameworkCore;
using SchoolManagement.Data;
using SchoolManagement.Models;

namespace SchoolManagement.Repositories;

/// <summary>
/// Repository implementation for SchoolClass entity data access operations.
/// Handles all database interactions for school classes using Entity Framework Core.
/// </summary>
public class SchoolClassRepository : ISchoolClassRepository
{
    private readonly SchoolDbContext _context;

    /// <summary>
    /// Initializes a new instance of the SchoolClassRepository.
    /// </summary>
    /// <param name="context">The database context.</param>
    public SchoolClassRepository(SchoolDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<SchoolClass>> GetAllAsync()
    {
        return await _context.SchoolClasses
            .Include(c => c.Students)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<SchoolClass?> GetByIdAsync(int id)
    {
        return await _context.SchoolClasses.FindAsync(id);
    }

    /// <inheritdoc />
    public async Task<SchoolClass?> GetByIdWithStudentsAsync(int id)
    {
        return await _context.SchoolClasses
            .Include(c => c.Students)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    /// <inheritdoc />
    public async Task<SchoolClass> AddAsync(SchoolClass schoolClass)
    {
        _context.SchoolClasses.Add(schoolClass);
        await _context.SaveChangesAsync();
        return schoolClass;
    }

    /// <inheritdoc />
    public async Task UpdateAsync(SchoolClass schoolClass)
    {
        _context.SchoolClasses.Update(schoolClass);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task DeleteAsync(SchoolClass schoolClass)
    {
        _context.SchoolClasses.Remove(schoolClass);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task<int> GetStudentCountAsync(int classId)
    {
        return await _context.Students.CountAsync(s => s.SchoolClassId == classId);
    }
}
