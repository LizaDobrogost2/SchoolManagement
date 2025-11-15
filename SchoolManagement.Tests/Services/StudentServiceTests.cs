using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SchoolManagement.Common;
using SchoolManagement.Models;
using SchoolManagement.Repositories;
using SchoolManagement.Services;

namespace SchoolManagement.Tests.Services;

/// <summary>
/// Unit tests for StudentService
/// </summary>
public class StudentServiceTests
{
    private readonly Mock<IStudentRepository> _mockStudentRepository;
    private readonly Mock<ISchoolClassRepository> _mockClassRepository;
    private readonly Mock<ILogger<StudentService>> _mockLogger;
    private readonly StudentService _sut; // System Under Test

    public StudentServiceTests()
    {
        _mockStudentRepository = new Mock<IStudentRepository>();
        _mockClassRepository = new Mock<ISchoolClassRepository>();
        _mockLogger = new Mock<ILogger<StudentService>>();
        
        _sut = new StudentService(
            _mockStudentRepository.Object,
            _mockClassRepository.Object,
            _mockLogger.Object);
    }

    #region GetAllStudentsAsync Tests

    [Fact]
    public async Task GetAllStudentsAsync_ShouldReturnAllStudents()
    {
        // Arrange
        var students = new List<Student>
        {
            new() { StudentId = "S001", Name = "John", Surname = "Doe", DateOfBirth = new DateTime(2005, 1, 1) },
            new() { StudentId = "S002", Name = "Jane", Surname = "Smith", DateOfBirth = new DateTime(2006, 2, 2) }
        };
        
        _mockStudentRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(students);

        // Act
        var result = await _sut.GetAllStudentsAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(s => s.StudentId == "S001");
        result.Should().Contain(s => s.StudentId == "S002");
        _mockStudentRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllStudentsAsync_WhenNoStudents_ShouldReturnEmptyList()
    {
        // Arrange
        _mockStudentRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Student>());

        // Act
        var result = await _sut.GetAllStudentsAsync();

        // Assert
        result.Should().BeEmpty();
    }

    #endregion

    #region GetStudentByIdAsync Tests

    [Fact]
    public async Task GetStudentByIdAsync_WithValidId_ShouldReturnStudent()
    {
        // Arrange
        var student = new Student 
        { 
            StudentId = "S001", 
            Name = "John", 
            Surname = "Doe", 
            DateOfBirth = new DateTime(2005, 1, 1) 
        };
        
        _mockStudentRepository.Setup(r => r.GetByIdAsync("S001"))
            .ReturnsAsync(student);

        // Act
        var result = await _sut.GetStudentByIdAsync("S001");

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(ServiceResultStatus.Ok);
        result.Data.Should().NotBeNull();
        result.Data!.StudentId.Should().Be("S001");
        result.Data.Name.Should().Be("John");
    }

    [Fact]
    public async Task GetStudentByIdAsync_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        _mockStudentRepository.Setup(r => r.GetByIdAsync("INVALID"))
            .ReturnsAsync((Student?)null);

        // Act
        var result = await _sut.GetStudentByIdAsync("INVALID");

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(ServiceResultStatus.NotFound);
        result.ErrorMessage.Should().Contain("not found");
    }

    #endregion

    #region CreateStudentAsync Tests

    [Fact]
    public async Task CreateStudentAsync_WithValidData_ShouldCreateStudent()
    {
        // Arrange
        var dto = new CreateStudentDto(
            StudentId: "S001",
            Name: "John",
            Surname: "Doe",
            DateOfBirth: new DateTime(2005, 1, 1),
            City: "New York",
            Street: null,
            PostalCode: null
        );

        _mockStudentRepository.Setup(r => r.ExistsAsync("S001"))
            .ReturnsAsync(false);
        
        _mockStudentRepository.Setup(r => r.AddAsync(It.IsAny<Student>()))
            .ReturnsAsync((Student s) => s);

        // Act
        var result = await _sut.CreateStudentAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(ServiceResultStatus.Created);
        result.Data.Should().NotBeNull();
        result.Data!.StudentId.Should().Be("S001");
        result.Data.Name.Should().Be("John");
        _mockStudentRepository.Verify(r => r.AddAsync(It.IsAny<Student>()), Times.Once);
    }

    [Fact]
    public async Task CreateStudentAsync_WithDuplicateId_ShouldReturnConflict()
    {
        // Arrange
        var dto = new CreateStudentDto(
            StudentId: "S001",
            Name: "John",
            Surname: "Doe",
            DateOfBirth: new DateTime(2005, 1, 1),
            City: null,
            Street: null,
            PostalCode: null
        );

        _mockStudentRepository.Setup(r => r.ExistsAsync("S001"))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.CreateStudentAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(ServiceResultStatus.Conflict);
        result.ErrorMessage.Should().Contain("already exists");
        _mockStudentRepository.Verify(r => r.AddAsync(It.IsAny<Student>()), Times.Never);
    }

    [Fact]
    public async Task CreateStudentAsync_WithMissingStudentId_ShouldReturnBadRequest()
    {
        // Arrange
        var dto = new CreateStudentDto(
            StudentId: "",
            Name: "John",
            Surname: "Doe",
            DateOfBirth: new DateTime(2005, 1, 1),
            City: null,
            Street: null,
            PostalCode: null
        );

        // Act
        var result = await _sut.CreateStudentAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(ServiceResultStatus.BadRequest);
    }

    [Fact]
    public async Task CreateStudentAsync_WithMissingName_ShouldReturnBadRequest()
    {
        // Arrange
        var dto = new CreateStudentDto(
            StudentId: "S001",
            Name: "",
            Surname: "Doe",
            DateOfBirth: new DateTime(2005, 1, 1),
            City: null,
            Street: null,
            PostalCode: null
        );

        // Act
        var result = await _sut.CreateStudentAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(ServiceResultStatus.BadRequest);
    }

    #endregion

    #region UpdateStudentAsync Tests

    [Fact]
    public async Task UpdateStudentAsync_WithValidData_ShouldUpdateStudent()
    {
        // Arrange
        var existingStudent = new Student
        {
            StudentId = "S001",
            Name = "John",
            Surname = "Doe",
            DateOfBirth = new DateTime(2005, 1, 1)
        };

        var dto = new UpdateStudentDto(
            Name: "John Updated",
            Surname: "Doe Updated",
            DateOfBirth: new DateTime(2005, 1, 1),
            City: "Los Angeles",
            Street: null,
            PostalCode: null
        );

        _mockStudentRepository.Setup(r => r.GetByIdAsync("S001"))
            .ReturnsAsync(existingStudent);
        
        _mockStudentRepository.Setup(r => r.UpdateAsync(It.IsAny<Student>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.UpdateStudentAsync("S001", dto);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(ServiceResultStatus.Ok);
        result.Data!.Name.Should().Be("John Updated");
        _mockStudentRepository.Verify(r => r.UpdateAsync(It.IsAny<Student>()), Times.Once);
    }

    [Fact]
    public async Task UpdateStudentAsync_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var dto = new UpdateStudentDto(
            Name: "John",
            Surname: "Doe",
            DateOfBirth: new DateTime(2005, 1, 1),
            City: null,
            Street: null,
            PostalCode: null
        );

        _mockStudentRepository.Setup(r => r.GetByIdAsync("INVALID"))
            .ReturnsAsync((Student?)null);

        // Act
        var result = await _sut.UpdateStudentAsync("INVALID", dto);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(ServiceResultStatus.NotFound);
        _mockStudentRepository.Verify(r => r.UpdateAsync(It.IsAny<Student>()), Times.Never);
    }

    #endregion

    #region PatchStudentAsync Tests

    [Fact]
    public async Task PatchStudentAsync_WithPartialData_ShouldUpdateOnlyProvidedFields()
    {
        // Arrange
        var existingStudent = new Student
        {
            StudentId = "S001",
            Name = "John",
            Surname = "Doe",
            DateOfBirth = new DateTime(2005, 1, 1),
            City = "New York"
        };

        var dto = new PatchStudentDto(
            Name: null,
            Surname: null,
            DateOfBirth: null,
            City: "Los Angeles",
            Street: null,
            PostalCode: null,
            SchoolClassId: null
        );

        _mockStudentRepository.Setup(r => r.GetByIdAsync("S001"))
            .ReturnsAsync(existingStudent);
        
        _mockStudentRepository.Setup(r => r.UpdateAsync(It.IsAny<Student>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.PatchStudentAsync("S001", dto);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(ServiceResultStatus.Ok);
        result.Data!.Name.Should().Be("John"); // Unchanged
        result.Data.City.Should().Be("Los Angeles"); // Changed
    }

    [Fact]
    public async Task PatchStudentAsync_AssignToClass_ShouldUpdateClassId()
    {
        // Arrange
        var existingStudent = new Student
        {
            StudentId = "S001",
            Name = "John",
            Surname = "Doe",
            DateOfBirth = new DateTime(2005, 1, 1)
        };

        var schoolClass = new SchoolClass
        {
            Id = 1,
            Name = "Class 5A",
            LeadingTeacher = "Mrs. Smith",
            Students = new List<Student>()
        };

        var dto = new PatchStudentDto(
            Name: null,
            Surname: null,
            DateOfBirth: null,
            City: null,
            Street: null,
            PostalCode: null,
            SchoolClassId: 1
        );

        _mockStudentRepository.Setup(r => r.GetByIdAsync("S001"))
            .ReturnsAsync(existingStudent);
        
        _mockClassRepository.Setup(r => r.GetByIdWithStudentsAsync(1))
            .ReturnsAsync(schoolClass);
        
        _mockStudentRepository.Setup(r => r.UpdateAsync(It.IsAny<Student>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.PatchStudentAsync("S001", dto);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(ServiceResultStatus.Ok);
        _mockStudentRepository.Verify(r => r.UpdateAsync(It.Is<Student>(s => s.SchoolClassId == 1)), Times.Once);
    }

    [Fact]
    public async Task PatchStudentAsync_AssignToFullClass_ShouldReturnBadRequest()
    {
        // Arrange
        var existingStudent = new Student
        {
            StudentId = "S001",
            Name = "John",
            Surname = "Doe",
            DateOfBirth = new DateTime(2005, 1, 1)
        };

        var fullClass = new SchoolClass
        {
            Id = 1,
            Name = "Class 5A",
            LeadingTeacher = "Mrs. Smith",
            Students = Enumerable.Range(1, BusinessConstants.MaxStudentsPerClass)
                .Select(i => new Student { StudentId = $"S{i:000}" })
                .ToList()
        };

        var dto = new PatchStudentDto(
            Name: null,
            Surname: null,
            DateOfBirth: null,
            City: null,
            Street: null,
            PostalCode: null,
            SchoolClassId: 1
        );

        _mockStudentRepository.Setup(r => r.GetByIdAsync("S001"))
            .ReturnsAsync(existingStudent);
        
        _mockClassRepository.Setup(r => r.GetByIdWithStudentsAsync(1))
            .ReturnsAsync(fullClass);

        // Act
        var result = await _sut.PatchStudentAsync("S001", dto);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(ServiceResultStatus.BadRequest);
        result.ErrorMessage.Should().Contain("maximum");
    }

    #endregion

    #region DeleteStudentAsync Tests

    [Fact]
    public async Task DeleteStudentAsync_WithValidId_ShouldDeleteStudent()
    {
        // Arrange
        var student = new Student
        {
            StudentId = "S001",
            Name = "John",
            Surname = "Doe",
            DateOfBirth = new DateTime(2005, 1, 1)
        };

        _mockStudentRepository.Setup(r => r.GetByIdAsync("S001"))
            .ReturnsAsync(student);
        
        _mockStudentRepository.Setup(r => r.DeleteAsync(It.IsAny<Student>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.DeleteStudentAsync("S001");

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(ServiceResultStatus.Ok);
        _mockStudentRepository.Verify(r => r.DeleteAsync(It.IsAny<Student>()), Times.Once);
    }

    [Fact]
    public async Task DeleteStudentAsync_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        _mockStudentRepository.Setup(r => r.GetByIdAsync("INVALID"))
            .ReturnsAsync((Student?)null);

        // Act
        var result = await _sut.DeleteStudentAsync("INVALID");

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(ServiceResultStatus.NotFound);
        _mockStudentRepository.Verify(r => r.DeleteAsync(It.IsAny<Student>()), Times.Never);
    }

    #endregion
}
