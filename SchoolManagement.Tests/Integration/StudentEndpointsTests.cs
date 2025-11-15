using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using SchoolManagement.Models;

namespace SchoolManagement.Tests.Integration;

/// <summary>
/// Integration tests for Student endpoints
/// </summary>
public class StudentEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public StudentEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllStudents_ShouldReturnOk()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/students");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var students = await response.Content.ReadFromJsonAsync<List<StudentDto>>();
        students.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateStudent_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var studentId = $"TEST{Guid.NewGuid():N}".Substring(0, 10);
        var newStudent = new CreateStudentDto(
            StudentId: studentId,
            Name: "Integration",
            Surname: "Test",
            DateOfBirth: new DateTime(2005, 1, 1),
            City: "TestCity",
            Street: null,
            PostalCode: null
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/students", newStudent);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdStudent = await response.Content.ReadFromJsonAsync<StudentDto>();
        createdStudent.Should().NotBeNull();
        createdStudent!.Name.Should().Be("Integration");
        response.Headers.Location.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateStudent_WithDuplicateId_ShouldReturnConflict()
    {
        // Arrange
        var studentId = $"DUP{Guid.NewGuid():N}".Substring(0, 10);
        var student = new CreateStudentDto(
            StudentId: studentId,
            Name: "Duplicate",
            Surname: "Test",
            DateOfBirth: new DateTime(2005, 1, 1),
            City: null,
            Street: null,
            PostalCode: null
        );

        // Create first student
        await _client.PostAsJsonAsync("/api/v1/students", student);

        // Act - Try to create duplicate
        var response = await _client.PostAsJsonAsync("/api/v1/students", student);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task GetStudentById_WithValidId_ShouldReturnStudent()
    {
        // Arrange
        var studentId = $"GET{Guid.NewGuid():N}".Substring(0, 10);
        var newStudent = new CreateStudentDto(
            StudentId: studentId,
            Name: "GetTest",
            Surname: "Student",
            DateOfBirth: new DateTime(2005, 1, 1),
            City: null,
            Street: null,
            PostalCode: null
        );

        await _client.PostAsJsonAsync("/api/v1/students", newStudent);

        // Act
        var response = await _client.GetAsync($"/api/v1/students/{studentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var student = await response.Content.ReadFromJsonAsync<StudentDto>();
        student.Should().NotBeNull();
        student!.StudentId.Should().Be(studentId);
    }

    [Fact]
    public async Task GetStudentById_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/students/NONEXISTENT");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateStudent_WithValidData_ShouldReturnOk()
    {
        // Arrange
        var studentId = $"UPD{Guid.NewGuid():N}".Substring(0, 10);
        var newStudent = new CreateStudentDto(
            StudentId: studentId,
            Name: "Original",
            Surname: "Name",
            DateOfBirth: new DateTime(2005, 1, 1),
            City: null,
            Street: null,
            PostalCode: null
        );

        await _client.PostAsJsonAsync("/api/v1/students", newStudent);

        var updateDto = new UpdateStudentDto(
            Name: "Updated",
            Surname: "Name",
            DateOfBirth: new DateTime(2005, 1, 1),
            City: "UpdatedCity",
            Street: null,
            PostalCode: null
        );

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/students/{studentId}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedStudent = await response.Content.ReadFromJsonAsync<StudentDto>();
        updatedStudent!.Name.Should().Be("Updated");
        updatedStudent.City.Should().Be("UpdatedCity");
    }

    [Fact]
    public async Task PatchStudent_WithPartialData_ShouldReturnOk()
    {
        // Arrange
        var studentId = $"PAT{Guid.NewGuid():N}".Substring(0, 10);
        var newStudent = new CreateStudentDto(
            StudentId: studentId,
            Name: "Patch",
            Surname: "Test",
            DateOfBirth: new DateTime(2005, 1, 1),
            City: "OriginalCity",
            Street: null,
            PostalCode: null
        );

        await _client.PostAsJsonAsync("/api/v1/students", newStudent);

        var patchDto = new PatchStudentDto(
            Name: null,
            Surname: null,
            DateOfBirth: null,
            City: "PatchedCity",
            Street: null,
            PostalCode: null,
            SchoolClassId: null
        );

        // Act
        var response = await _client.PatchAsJsonAsync($"/api/v1/students/{studentId}", patchDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var patchedStudent = await response.Content.ReadFromJsonAsync<StudentDto>();
        patchedStudent!.Name.Should().Be("Patch"); // Unchanged
        patchedStudent.City.Should().Be("PatchedCity"); // Changed
    }

    [Fact]
    public async Task DeleteStudent_WithValidId_ShouldReturnOk()
    {
        // Arrange
        var studentId = $"DEL{Guid.NewGuid():N}".Substring(0, 10);
        var newStudent = new CreateStudentDto(
            StudentId: studentId,
            Name: "Delete",
            Surname: "Test",
            DateOfBirth: new DateTime(2005, 1, 1),
            City: null,
            Street: null,
            PostalCode: null
        );

        await _client.PostAsJsonAsync("/api/v1/students", newStudent);

        // Act
        var response = await _client.DeleteAsync($"/api/v1/students/{studentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify deletion
        var getResponse = await _client.GetAsync($"/api/v1/students/{studentId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
