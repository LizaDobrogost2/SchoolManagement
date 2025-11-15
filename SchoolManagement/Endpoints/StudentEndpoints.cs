using SchoolManagement.Models;
using SchoolManagement.Services;
using Asp.Versioning;
using Asp.Versioning.Builder;

namespace SchoolManagement.Endpoints;

public static class StudentEndpoints
{
    public static RouteGroupBuilder MapStudentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v{version:apiVersion}/students")
            .WithTags("Students")
            .WithOpenApi();

        group.MapGet("/", GetAllStudents)
            .WithName("GetAllStudents")
            .Produces<IEnumerable<StudentDto>>()
            .MapToApiVersion(1.0);

        group.MapGet("/{id}", GetStudentById)
            .WithName("GetStudentById")
            .Produces<StudentDto>()
            .Produces(StatusCodes.Status404NotFound)
            .MapToApiVersion(1.0);

        group.MapPost("/", CreateStudent)
            .WithName("CreateStudent")
            .Produces<StudentDto>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status409Conflict)
            .MapToApiVersion(1.0);

        group.MapPut("/{id}", UpdateStudent)
            .WithName("UpdateStudent")
            .Produces<StudentDto>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .MapToApiVersion(1.0);

        group.MapPatch("/{id}", PatchStudent)
            .WithName("PatchStudent")
            .WithDescription("Partially update a student. Use this to assign/unassign students to/from classes.")
            .Produces<StudentDto>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .MapToApiVersion(1.0);

        group.MapDelete("/{id}", DeleteStudent)
            .WithName("DeleteStudent")
            .Produces<object>()
            .Produces(StatusCodes.Status404NotFound)
            .MapToApiVersion(1.0);

        return group;
    }

    private static async Task<IResult> GetAllStudents(IStudentService studentService)
    {
        var students = await studentService.GetAllStudentsAsync();
        return Results.Ok(students);
    }

    private static async Task<IResult> GetStudentById(string id, IStudentService studentService)
    {
        var result = await studentService.GetStudentByIdAsync(id);
        return result.ToHttpResult();
    }

    private static async Task<IResult> CreateStudent(CreateStudentDto dto, IStudentService studentService)
    {
        var result = await studentService.CreateStudentAsync(dto);
        return result.Status == ServiceResultStatus.Created
            ? Results.Created($"/api/v1/students/{result.Data?.StudentId}", result.Data)
            : result.ToHttpResult();
    }

    private static async Task<IResult> UpdateStudent(string id, UpdateStudentDto dto, IStudentService studentService)
    {
        var result = await studentService.UpdateStudentAsync(id, dto);
        return result.ToHttpResult();
    }

    private static async Task<IResult> PatchStudent(string id, PatchStudentDto dto, IStudentService studentService)
    {
        var result = await studentService.PatchStudentAsync(id, dto);
        return result.ToHttpResult();
    }

    private static async Task<IResult> DeleteStudent(string id, IStudentService studentService)
    {
        var result = await studentService.DeleteStudentAsync(id);
        return result.ToHttpResult(data => new { message = data });
    }
}
