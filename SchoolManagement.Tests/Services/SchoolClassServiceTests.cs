using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SchoolManagement.Common;
using SchoolManagement.Models;
using SchoolManagement.Repositories;
using SchoolManagement.Services;

namespace SchoolManagement.Tests.Services;

/// <summary>
/// Unit tests for SchoolClassService
/// </summary>
public class SchoolClassServiceTests
{
    private readonly Mock<ISchoolClassRepository> _mockClassRepository;
    private readonly Mock<ILogger<SchoolClassService>> _mockLogger;
    private readonly SchoolClassService _sut;

    public SchoolClassServiceTests()
    {
        _mockClassRepository = new Mock<ISchoolClassRepository>();
        _mockLogger = new Mock<ILogger<SchoolClassService>>();
        
        _sut = new SchoolClassService(
            _mockClassRepository.Object,
            _mockLogger.Object);
    }

    #region GetAllClassesAsync Tests

    [Fact]
    public async Task GetAllClassesAsync_ShouldReturnAllClasses()
    {
        // Arrange
        var classes = new List<SchoolClass>
        {
            new() { Id = 1, Name = "Class 5A", LeadingTeacher = "Mrs. Smith", Students = new List<Student>() },
            new() { Id = 2, Name = "Class 5B", LeadingTeacher = "Mr. Johnson", Students = new List<Student>() }
        };
        
        _mockClassRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(classes);

        // Act
        var result = await _sut.GetAllClassesAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(c => c.Name == "Class 5A");
        result.Should().Contain(c => c.Name == "Class 5B");
    }

    [Fact]
    public async Task GetAllClassesAsync_WhenNoClasses_ShouldReturnEmptyList()
    {
        // Arrange
        _mockClassRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<SchoolClass>());

        // Act
        var result = await _sut.GetAllClassesAsync();

        // Assert
        result.Should().BeEmpty();
    }

    #endregion

    #region GetClassByIdAsync Tests

    [Fact]
    public async Task GetClassByIdAsync_WithValidId_ShouldReturnClass()
    {
        // Arrange
        var schoolClass = new SchoolClass
        {
            Id = 1,
            Name = "Class 5A",
            LeadingTeacher = "Mrs. Smith",
            Students = new List<Student>
            {
                new() { StudentId = "S001", Name = "John", Surname = "Doe", DateOfBirth = new DateTime(2005, 1, 1) }
            }
        };
        
        _mockClassRepository.Setup(r => r.GetByIdWithStudentsAsync(1))
            .ReturnsAsync(schoolClass);

        // Act
        var result = await _sut.GetClassByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(ServiceResultStatus.Ok);
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be("Class 5A");
        result.Data.Students.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetClassByIdAsync_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        _mockClassRepository.Setup(r => r.GetByIdWithStudentsAsync(999))
            .ReturnsAsync((SchoolClass?)null);

        // Act
        var result = await _sut.GetClassByIdAsync(999);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(ServiceResultStatus.NotFound);
        result.ErrorMessage.Should().Contain("not found");
    }

    #endregion

    #region CreateClassAsync Tests

    [Fact]
    public async Task CreateClassAsync_WithValidData_ShouldCreateClass()
    {
        // Arrange
        var dto = new CreateSchoolClassDto(
            Name: "Class 5A",
            LeadingTeacher: "Mrs. Smith"
        );

        _mockClassRepository.Setup(r => r.AddAsync(It.IsAny<SchoolClass>()))
            .ReturnsAsync((SchoolClass c) => c);

        // Act
        var result = await _sut.CreateClassAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(ServiceResultStatus.Created);
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be("Class 5A");
        result.Data.LeadingTeacher.Should().Be("Mrs. Smith");
        _mockClassRepository.Verify(r => r.AddAsync(It.IsAny<SchoolClass>()), Times.Once);
    }

    [Fact]
    public async Task CreateClassAsync_WithMissingName_ShouldReturnBadRequest()
    {
        // Arrange
        var dto = new CreateSchoolClassDto(
            Name: "",
            LeadingTeacher: "Mrs. Smith"
        );

        // Act
        var result = await _sut.CreateClassAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(ServiceResultStatus.BadRequest);
        _mockClassRepository.Verify(r => r.AddAsync(It.IsAny<SchoolClass>()), Times.Never);
    }

    [Fact]
    public async Task CreateClassAsync_WithMissingTeacher_ShouldReturnBadRequest()
    {
        // Arrange
        var dto = new CreateSchoolClassDto(
            Name: "Class 5A",
            LeadingTeacher: ""
        );

        // Act
        var result = await _sut.CreateClassAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(ServiceResultStatus.BadRequest);
    }

    #endregion

    #region UpdateClassAsync Tests

    [Fact]
    public async Task UpdateClassAsync_WithValidData_ShouldUpdateClass()
    {
        // Arrange
        var existingClass = new SchoolClass
        {
            Id = 1,
            Name = "Class 5A",
            LeadingTeacher = "Mrs. Smith",
            Students = new List<Student>()
        };

        var dto = new UpdateSchoolClassDto(
            Name: "Class 5A Advanced",
            LeadingTeacher: "Mrs. Smith-Brown"
        );

        _mockClassRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(existingClass);
        
        _mockClassRepository.Setup(r => r.UpdateAsync(It.IsAny<SchoolClass>()))
            .Returns(Task.CompletedTask);
        
        _mockClassRepository.Setup(r => r.GetStudentCountAsync(1))
            .ReturnsAsync(0);

        // Act
        var result = await _sut.UpdateClassAsync(1, dto);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(ServiceResultStatus.Ok);
        result.Data!.Name.Should().Be("Class 5A Advanced");
        result.Data.LeadingTeacher.Should().Be("Mrs. Smith-Brown");
        _mockClassRepository.Verify(r => r.UpdateAsync(It.IsAny<SchoolClass>()), Times.Once);
    }

    [Fact]
    public async Task UpdateClassAsync_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var dto = new UpdateSchoolClassDto(
            Name: "Class 5A",
            LeadingTeacher: "Mrs. Smith"
        );

        _mockClassRepository.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((SchoolClass?)null);

        // Act
        var result = await _sut.UpdateClassAsync(999, dto);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(ServiceResultStatus.NotFound);
        _mockClassRepository.Verify(r => r.UpdateAsync(It.IsAny<SchoolClass>()), Times.Never);
    }

    #endregion

    #region DeleteClassAsync Tests

    [Fact]
    public async Task DeleteClassAsync_WithValidId_ShouldDeleteClass()
    {
        // Arrange
        var schoolClass = new SchoolClass
        {
            Id = 1,
            Name = "Class 5A",
            LeadingTeacher = "Mrs. Smith",
            Students = new List<Student>()
        };

        _mockClassRepository.Setup(r => r.GetByIdWithStudentsAsync(1))
            .ReturnsAsync(schoolClass);
        
        _mockClassRepository.Setup(r => r.DeleteAsync(It.IsAny<SchoolClass>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.DeleteClassAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(ServiceResultStatus.Ok);
        _mockClassRepository.Verify(r => r.DeleteAsync(It.IsAny<SchoolClass>()), Times.Once);
    }

    [Fact]
    public async Task DeleteClassAsync_WithStudents_ShouldUnassignStudentsAndDelete()
    {
        // Arrange
        var students = new List<Student>
        {
            new() { StudentId = "S001", Name = "John", Surname = "Doe", DateOfBirth = new DateTime(2005, 1, 1), SchoolClassId = 1 },
            new() { StudentId = "S002", Name = "Jane", Surname = "Smith", DateOfBirth = new DateTime(2006, 2, 2), SchoolClassId = 1 }
        };

        var schoolClass = new SchoolClass
        {
            Id = 1,
            Name = "Class 5A",
            LeadingTeacher = "Mrs. Smith",
            Students = students
        };

        _mockClassRepository.Setup(r => r.GetByIdWithStudentsAsync(1))
            .ReturnsAsync(schoolClass);
        
        _mockClassRepository.Setup(r => r.DeleteAsync(It.IsAny<SchoolClass>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.DeleteClassAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(ServiceResultStatus.Ok);
        students.All(s => s.SchoolClassId == null).Should().BeTrue();
        _mockClassRepository.Verify(r => r.DeleteAsync(It.IsAny<SchoolClass>()), Times.Once);
    }

    #endregion
}
