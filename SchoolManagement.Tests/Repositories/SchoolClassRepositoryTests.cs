using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Data;
using SchoolManagement.Models;
using SchoolManagement.Repositories;

namespace SchoolManagement.Tests.Repositories;

/// <summary>
/// Unit tests for SchoolClassRepository
/// </summary>
public class SchoolClassRepositoryTests : IDisposable
{
    private readonly SchoolDbContext _context;
    private readonly SchoolClassRepository _sut;

    public SchoolClassRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<SchoolDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new SchoolDbContext(options);
        _sut = new SchoolClassRepository(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllClasses()
    {
        // Arrange
        var classes = new List<SchoolClass>
        {
            new() { Name = "Class 5A", LeadingTeacher = "Mrs. Smith" },
            new() { Name = "Class 5B", LeadingTeacher = "Mr. Johnson" }
        };

        await _context.SchoolClasses.AddRangeAsync(classes);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(c => c.Name == "Class 5A");
        result.Should().Contain(c => c.Name == "Class 5B");
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnClass()
    {
        // Arrange
        var schoolClass = new SchoolClass
        {
            Name = "Class 5A",
            LeadingTeacher = "Mrs. Smith"
        };

        await _context.SchoolClasses.AddAsync(schoolClass);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetByIdAsync(schoolClass.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Class 5A");
        result.LeadingTeacher.Should().Be("Mrs. Smith");
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Act
        var result = await _sut.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdWithStudentsAsync_ShouldIncludeStudents()
    {
        // Arrange
        var schoolClass = new SchoolClass
        {
            Name = "Class 5A",
            LeadingTeacher = "Mrs. Smith"
        };

        var students = new List<Student>
        {
            new() { StudentId = "S001", Name = "John", Surname = "Doe", DateOfBirth = new DateTime(2005, 1, 1) },
            new() { StudentId = "S002", Name = "Jane", Surname = "Smith", DateOfBirth = new DateTime(2006, 2, 2) }
        };

        await _context.SchoolClasses.AddAsync(schoolClass);
        await _context.SaveChangesAsync();

        foreach (var student in students)
        {
            student.SchoolClassId = schoolClass.Id;
            await _context.Students.AddAsync(student);
        }
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetByIdWithStudentsAsync(schoolClass.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Students.Should().HaveCount(2);
        result.Students.Should().Contain(s => s.StudentId == "S001");
        result.Students.Should().Contain(s => s.StudentId == "S002");
    }

    [Fact]
    public async Task AddAsync_ShouldAddClass()
    {
        // Arrange
        var schoolClass = new SchoolClass
        {
            Name = "Class 5A",
            LeadingTeacher = "Mrs. Smith"
        };

        // Act
        await _sut.AddAsync(schoolClass);

        // Assert
        var savedClass = await _context.SchoolClasses.FindAsync(schoolClass.Id);
        savedClass.Should().NotBeNull();
        savedClass!.Name.Should().Be("Class 5A");
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateClass()
    {
        // Arrange
        var schoolClass = new SchoolClass
        {
            Name = "Class 5A",
            LeadingTeacher = "Mrs. Smith"
        };

        await _context.SchoolClasses.AddAsync(schoolClass);
        await _context.SaveChangesAsync();

        // Act
        schoolClass.Name = "Class 5A Advanced";
        await _sut.UpdateAsync(schoolClass);

        // Assert
        var updatedClass = await _context.SchoolClasses.FindAsync(schoolClass.Id);
        updatedClass!.Name.Should().Be("Class 5A Advanced");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveClass()
    {
        // Arrange
        var schoolClass = new SchoolClass
        {
            Name = "Class 5A",
            LeadingTeacher = "Mrs. Smith"
        };

        await _context.SchoolClasses.AddAsync(schoolClass);
        await _context.SaveChangesAsync();
        var classId = schoolClass.Id;

        // Act
        await _sut.DeleteAsync(schoolClass);

        // Assert
        var deletedClass = await _context.SchoolClasses.FindAsync(classId);
        deletedClass.Should().BeNull();
    }

    [Fact]
    public async Task GetStudentCountAsync_ShouldReturnCorrectCount()
    {
        // Arrange
        var schoolClass = new SchoolClass
        {
            Name = "Class 5A",
            LeadingTeacher = "Mrs. Smith"
        };

        await _context.SchoolClasses.AddAsync(schoolClass);
        await _context.SaveChangesAsync();

        var students = new List<Student>
        {
            new() { StudentId = "S001", Name = "John", Surname = "Doe", DateOfBirth = new DateTime(2005, 1, 1), SchoolClassId = schoolClass.Id },
            new() { StudentId = "S002", Name = "Jane", Surname = "Smith", DateOfBirth = new DateTime(2006, 2, 2), SchoolClassId = schoolClass.Id },
            new() { StudentId = "S003", Name = "Bob", Surname = "Johnson", DateOfBirth = new DateTime(2005, 3, 3), SchoolClassId = schoolClass.Id }
        };

        await _context.Students.AddRangeAsync(students);
        await _context.SaveChangesAsync();

        // Act
        var count = await _sut.GetStudentCountAsync(schoolClass.Id);

        // Assert
        count.Should().Be(3);
    }

    [Fact]
    public async Task GetStudentCountAsync_ForEmptyClass_ShouldReturnZero()
    {
        // Arrange
        var schoolClass = new SchoolClass
        {
            Name = "Class 5A",
            LeadingTeacher = "Mrs. Smith"
        };

        await _context.SchoolClasses.AddAsync(schoolClass);
        await _context.SaveChangesAsync();

        // Act
        var count = await _sut.GetStudentCountAsync(schoolClass.Id);

        // Assert
        count.Should().Be(0);
    }

    [Fact]
    public async Task GetAllAsync_IncludesStudents()
    {
        // Arrange
        var schoolClass = new SchoolClass
        {
            Name = "Class 5A",
            LeadingTeacher = "Mrs. Smith"
        };

        await _context.SchoolClasses.AddAsync(schoolClass);
        await _context.SaveChangesAsync();

        var student = new Student
        {
            StudentId = "S001",
            Name = "John",
            Surname = "Doe",
            DateOfBirth = new DateTime(2005, 1, 1),
            SchoolClassId = schoolClass.Id
        };

        await _context.Students.AddAsync(student);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        var retrievedClass = result.First();
        retrievedClass.Students.Should().HaveCount(1);
        retrievedClass.Students.First().StudentId.Should().Be("S001");
    }
}
