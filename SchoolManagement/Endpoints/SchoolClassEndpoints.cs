using SchoolManagement.Models;
using SchoolManagement.Services;
using Asp.Versioning;
using Asp.Versioning.Builder;

namespace SchoolManagement.Endpoints;

public static class SchoolClassEndpoints
{
    public static RouteGroupBuilder MapSchoolClassEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v{version:apiVersion}/classes")
            .WithTags("Classes")
            .WithOpenApi();

        group.MapGet("/", GetAllClasses)
            .WithName("GetAllClasses")
            .Produces<IEnumerable<SchoolClassDto>>()
            .MapToApiVersion(1.0);

        group.MapGet("/{id}", GetClassById)
            .WithName("GetClassById")
            .Produces<SchoolClassDto>()
            .Produces(StatusCodes.Status404NotFound)
            .MapToApiVersion(1.0);

        group.MapPost("/", CreateClass)
            .WithName("CreateClass")
            .Produces<SchoolClassDto>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .MapToApiVersion(1.0);

        group.MapPut("/{id}", UpdateClass)
            .WithName("UpdateClass")
            .Produces<SchoolClassDto>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .MapToApiVersion(1.0);

        group.MapPatch("/{id}", PatchClass)
            .WithName("PatchClass")
            .WithDescription("Partially update a class (name or teacher)")
            .Produces<SchoolClassDto>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .MapToApiVersion(1.0);

        group.MapDelete("/{id}", DeleteClass)
            .WithName("DeleteClass")
            .Produces<object>()
            .Produces(StatusCodes.Status404NotFound)
            .MapToApiVersion(1.0);

        // Keep these for backward compatibility, but document as deprecated
        group.MapPost("/{classId}/students", AddStudentToClass)
            .WithName("AddStudentToClass")
            .WithDescription("?? Deprecated: Use PATCH /api/v1/students/{id} with {\"schoolClassId\": classId} instead")
            .Produces<object>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .MapToApiVersion(1.0);

        group.MapDelete("/{classId}/students/{studentId}", RemoveStudentFromClass)
            .WithName("RemoveStudentFromClass")
            .WithDescription("?? Deprecated: Use PATCH /api/v1/students/{id} with {\"schoolClassId\": null} instead")
            .Produces<object>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .MapToApiVersion(1.0);

        return group;
    }

    private static async Task<IResult> GetAllClasses(ISchoolClassService classService)
    {
        var classes = await classService.GetAllClassesAsync();
        return Results.Ok(classes);
    }

    private static async Task<IResult> GetClassById(int id, ISchoolClassService classService)
    {
        var result = await classService.GetClassByIdAsync(id);
        return result.ToHttpResult();
    }

    private static async Task<IResult> CreateClass(CreateSchoolClassDto dto, ISchoolClassService classService)
    {
        var result = await classService.CreateClassAsync(dto);
        return result.Status == ServiceResultStatus.Created
            ? Results.Created($"/api/v1/classes/{result.Data?.Id}", result.Data)
            : result.ToHttpResult();
    }

    private static async Task<IResult> UpdateClass(int id, UpdateSchoolClassDto dto, ISchoolClassService classService)
    {
        var result = await classService.UpdateClassAsync(id, dto);
        return result.ToHttpResult();
    }

    private static async Task<IResult> PatchClass(int id, PatchSchoolClassDto dto, ISchoolClassService classService)
    {
        var result = await classService.PatchClassAsync(id, dto);
        return result.ToHttpResult();
    }

    private static async Task<IResult> DeleteClass(int id, ISchoolClassService classService)
    {
        var result = await classService.DeleteClassAsync(id);
        return result.ToHttpResult(data => new { message = data });
    }

    private static async Task<IResult> AddStudentToClass(
        int classId, 
        AddStudentToClassDto dto, 
        ISchoolClassService classService)
    {
        var result = await classService.AddStudentToClassAsync(classId, dto.StudentId);
        return result.ToHttpResult(data => new { message = data });
    }

    private static async Task<IResult> RemoveStudentFromClass(
        int classId, 
        string studentId, 
        ISchoolClassService classService)
    {
        var result = await classService.RemoveStudentFromClassAsync(classId, studentId);
        return result.ToHttpResult(data => new { message = data });
    }
}
