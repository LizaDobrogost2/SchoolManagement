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
        var className = $"TestClass{Guid.NewGuid():N}".Substring(0, 20);
        var newClass = new CreateSchoolClassDto(
            Name: className,
            LeadingTeacher: "Test Teacher"
        );

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
        var invalidClass = new CreateSchoolClassDto(
            Name: "",
            LeadingTeacher: "Teacher"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/classes", invalidClass);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetClassById_WithValidId_ShouldReturnClass()
    {
        // Arrange
        var className = $"GetTest{Guid.NewGuid():N}".Substring(0, 20);
        var newClass = new CreateSchoolClassDto(
            Name: className,
            LeadingTeacher: "Get Teacher"
        );

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
        var className = $"UpdateTest{Guid.NewGuid():N}".Substring(0, 20);
        var newClass = new CreateSchoolClassDto(
            Name: className,
            LeadingTeacher: "Original Teacher"
        );

        var createResponse = await _client.PostAsJsonAsync("/api/v1/classes", newClass);
        var createdClass = await createResponse.Content.ReadFromJsonAsync<SchoolClassDto>();

        var updateDto = new UpdateSchoolClassDto(
            Name: "Updated Class Name",
            LeadingTeacher: "Updated Teacher"
        );

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
        var className = $"PatchTest{Guid.NewGuid():N}".Substring(0, 20);
        var newClass = new CreateSchoolClassDto(
            Name: className,
            LeadingTeacher: "Original Teacher"
        );

        var createResponse = await _client.PostAsJsonAsync("/api/v1/classes", newClass);
        var createdClass = await createResponse.Content.ReadFromJsonAsync<SchoolClassDto>();

        var patchDto = new PatchSchoolClassDto(
            Name: null,
            LeadingTeacher: "Patched Teacher"
        );

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
        var className = $"DeleteTest{Guid.NewGuid():N}".Substring(0, 20);
        var newClass = new CreateSchoolClassDto(
            Name: className,
            LeadingTeacher: "Delete Teacher"
        );

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
}
