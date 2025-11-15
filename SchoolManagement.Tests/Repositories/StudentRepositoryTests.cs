using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Data;
using SchoolManagement.Models;
using SchoolManagement.Repositories;

namespace SchoolManagement.Tests.Repositories;

/// <summary>
/// Unit tests for StudentRepository
/// </summary>
public class StudentRepositoryTests : IDisposable
{
    private readonly SchoolDbContext _context;
    private readonly StudentRepository _sut;

    public StudentRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<SchoolDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new SchoolDbContext(options);
        _sut = new StudentRepository(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllStudents()
    {
        // Arrange
        var students = new List<Student>
        {
            new() { StudentId = "S001", Name = "John", Surname = "Doe", DateOfBirth = new DateTime(2005, 1, 1) },
            new() { StudentId = "S002", Name = "Jane", Surname = "Smith", DateOfBirth = new DateTime(2006, 2, 2) }
        };

        await _context.Students.AddRangeAsync(students);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(s => s.StudentId == "S001");
        result.Should().Contain(s => s.StudentId == "S002");
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnStudent()
    {
        // Arrange
        var student = new Student
        {
            StudentId = "S001",
            Name = "John",
            Surname = "Doe",
            DateOfBirth = new DateTime(2005, 1, 1)
        };

        await _context.Students.AddAsync(student);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetByIdAsync("S001");

        // Assert
        result.Should().NotBeNull();
        result!.StudentId.Should().Be("S001");
        result.Name.Should().Be("John");
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Act
        var result = await _sut.GetByIdAsync("INVALID");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_ShouldAddStudent()
    {
        // Arrange
        var student = new Student
        {
            StudentId = "S001",
            Name = "John",
            Surname = "Doe",
            DateOfBirth = new DateTime(2005, 1, 1)
        };

        // Act
        await _sut.AddAsync(student);

        // Assert
        var savedStudent = await _context.Students.FindAsync("S001");
        savedStudent.Should().NotBeNull();
        savedStudent!.Name.Should().Be("John");
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateStudent()
    {
        // Arrange
        var student = new Student
        {
            StudentId = "S001",
            Name = "John",
            Surname = "Doe",
            DateOfBirth = new DateTime(2005, 1, 1)
        };

        await _context.Students.AddAsync(student);
        await _context.SaveChangesAsync();

        // Act
        student.Name = "John Updated";
        await _sut.UpdateAsync(student);

        // Assert
        var updatedStudent = await _context.Students.FindAsync("S001");
        updatedStudent!.Name.Should().Be("John Updated");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveStudent()
    {
        // Arrange
        var student = new Student
        {
            StudentId = "S001",
            Name = "John",
            Surname = "Doe",
            DateOfBirth = new DateTime(2005, 1, 1)
        };

        await _context.Students.AddAsync(student);
        await _context.SaveChangesAsync();

        // Act
        await _sut.DeleteAsync(student);

        // Assert
        var deletedStudent = await _context.Students.FindAsync("S001");
        deletedStudent.Should().BeNull();
    }

    [Fact]
    public async Task ExistsAsync_WithExistingId_ShouldReturnTrue()
    {
        // Arrange
        var student = new Student
        {
            StudentId = "S001",
            Name = "John",
            Surname = "Doe",
            DateOfBirth = new DateTime(2005, 1, 1)
        };

        await _context.Students.AddAsync(student);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.ExistsAsync("S001");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistingId_ShouldReturnFalse()
    {
        // Act
        var result = await _sut.ExistsAsync("INVALID");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetAllAsync_IncludesSchoolClass()
    {
        // Arrange
        var schoolClass = new SchoolClass
        {
            Name = "Class 5A",
            LeadingTeacher = "Mrs. Smith"
        };

        var student = new Student
        {
            StudentId = "S001",
            Name = "John",
            Surname = "Doe",
            DateOfBirth = new DateTime(2005, 1, 1),
            SchoolClass = schoolClass
        };

        await _context.SchoolClasses.AddAsync(schoolClass);
        await _context.Students.AddAsync(student);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        var retrievedStudent = result.First();
        retrievedStudent.SchoolClass.Should().NotBeNull();
        retrievedStudent.SchoolClass!.Name.Should().Be("Class 5A");
    }
}
