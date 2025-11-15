using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using SchoolManagement.Models;

namespace SchoolManagement.Tests.Integration;

/// <summary>
/// Integration tests for SchoolClass endpoints
/// </summary>
public class SchoolClassEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public SchoolClassEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllClasses_ShouldReturnOk()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/classes");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var classes = await response.Content.ReadFromJsonAsync<List<SchoolClassDto>>();
        classes.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateClass_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var newClass = new CreateSchoolClassDto
        {
            Name = $"TestClass{Guid.NewGuid():N}".Substring(0, 20),
            LeadingTeacher = "Test Teacher"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/classes", newClass);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdClass = await response.Content.ReadFromJsonAsync<SchoolClassDto>();
        createdClass.Should().NotBeNull();
        createdClass!.LeadingTeacher.Should().Be("Test Teacher");
        response.Headers.Location.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateClass_WithMissingData_ShouldReturnBadRequest()
    {
        // Arrange
        var invalidClass = new CreateSchoolClassDto
        {
            Name = "",
            LeadingTeacher = "Teacher"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/classes", invalidClass);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetClassById_WithValidId_ShouldReturnClass()
    {
        // Arrange
        var newClass = new CreateSchoolClassDto
        {
            Name = $"GetTest{Guid.NewGuid():N}".Substring(0, 20),
            LeadingTeacher = "Get Teacher"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/v1/classes", newClass);
        var createdClass = await createResponse.Content.ReadFromJsonAsync<SchoolClassDto>();

        // Act
        var response = await _client.GetAsync($"/api/v1/classes/{createdClass!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var retrievedClass = await response.Content.ReadFromJsonAsync<SchoolClassDto>();
        retrievedClass.Should().NotBeNull();
        retrievedClass!.Id.Should().Be(createdClass.Id);
    }

    [Fact]
    public async Task GetClassById_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/classes/999999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateClass_WithValidData_ShouldReturnOk()
    {
        // Arrange
        var newClass = new CreateSchoolClassDto
        {
            Name = $"UpdateTest{Guid.NewGuid():N}".Substring(0, 20),
            LeadingTeacher = "Original Teacher"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/v1/classes", newClass);
        var createdClass = await createResponse.Content.ReadFromJsonAsync<SchoolClassDto>();

        var updateDto = new UpdateSchoolClassDto
        {
            Name = "Updated Class Name",
            LeadingTeacher = "Updated Teacher"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/classes/{createdClass!.Id}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedClass = await response.Content.ReadFromJsonAsync<SchoolClassDto>();
        updatedClass!.Name.Should().Be("Updated Class Name");
        updatedClass.LeadingTeacher.Should().Be("Updated Teacher");
    }

    [Fact]
    public async Task PatchClass_WithPartialData_ShouldReturnOk()
    {
        // Arrange
        var newClass = new CreateSchoolClassDto
        {
            Name = $"PatchTest{Guid.NewGuid():N}".Substring(0, 20),
            LeadingTeacher = "Original Teacher"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/v1/classes", newClass);
        var createdClass = await createResponse.Content.ReadFromJsonAsync<SchoolClassDto>();

        var patchDto = new PatchSchoolClassDto
        {
            LeadingTeacher = "Patched Teacher"
        };

        // Act
        var response = await _client.PatchAsJsonAsync($"/api/v1/classes/{createdClass!.Id}", patchDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var patchedClass = await response.Content.ReadFromJsonAsync<SchoolClassDto>();
        patchedClass!.LeadingTeacher.Should().Be("Patched Teacher");
    }

    [Fact]
    public async Task DeleteClass_WithValidId_ShouldReturnOk()
    {
        // Arrange
        var newClass = new CreateSchoolClassDto
        {
            Name = $"DeleteTest{Guid.NewGuid():N}".Substring(0, 20),
            LeadingTeacher = "Delete Teacher"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/v1/classes", newClass);
        var createdClass = await createResponse.Content.ReadFromJsonAsync<SchoolClassDto>();

        // Act
        var response = await _client.DeleteAsync($"/api/v1/classes/{createdClass!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify deletion
        var getResponse = await _client.GetAsync($"/api/v1/classes/{createdClass.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AddStudentToClass_WithValidData_ShouldReturnOk()
    {
        // Arrange
        // Create a class
        var newClass = new CreateSchoolClassDto
        {
            Name = $"AssignTest{Guid.NewGuid():N}".Substring(0, 20),
            LeadingTeacher = "Assignment Teacher"
        };
        var classResponse = await _client.PostAsJsonAsync("/api/v1/classes", newClass);
        var createdClass = await classResponse.Content.ReadFromJsonAsync<SchoolClassDto>();

        // Create a student
        var studentId = $"ASN{Guid.NewGuid():N}".Substring(0, 10);
        var newStudent = new CreateStudentDto
        {
            StudentId = studentId,
            Name = "Assign",
            Surname = "Test",
            DateOfBirth = new DateTime(2005, 1, 1)
        };
        await _client.PostAsJsonAsync("/api/v1/students", newStudent);

        var assignDto = new AddStudentToClassDto
        {
            StudentId = studentId
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/v1/classes/{createdClass!.Id}/students", assignDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RemoveStudentFromClass_WithValidData_ShouldReturnOk()
    {
        // Arrange
        // Create a class
        var newClass = new CreateSchoolClassDto
        {
            Name = $"RemoveTest{Guid.NewGuid():N}".Substring(0, 20),
            LeadingTeacher = "Remove Teacher"
        };
        var classResponse = await _client.PostAsJsonAsync("/api/v1/classes", newClass);
        var createdClass = await classResponse.Content.ReadFromJsonAsync<SchoolClassDto>();

        // Create and assign a student
        var studentId = $"REM{Guid.NewGuid():N}".Substring(0, 10);
        var newStudent = new CreateStudentDto
        {
            StudentId = studentId,
            Name = "Remove",
            Surname = "Test",
            DateOfBirth = new DateTime(2005, 1, 1)
        };
        await _client.PostAsJsonAsync("/api/v1/students", newStudent);

        var assignDto = new AddStudentToClassDto { StudentId = studentId };
        await _client.PostAsJsonAsync($"/api/v1/classes/{createdClass!.Id}/students", assignDto);

        // Act
        var response = await _client.DeleteAsync($"/api/v1/classes/{createdClass.Id}/students/{studentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
