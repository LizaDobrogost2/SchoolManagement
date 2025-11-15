using SchoolManagement.Models;

namespace SchoolManagement.Extensions;

public static class MappingExtensions
{
    public static StudentDto ToDto(this Student student)
    {
        return new StudentDto(
            student.StudentId,
            student.Name,
            student.Surname,
            student.DateOfBirth,
            student.City,
            student.Street,
            student.PostalCode,
            student.SchoolClassId,
            student.SchoolClass?.Name
        );
    }

    public static SchoolClassDto ToDto(this SchoolClass schoolClass)
    {
        return new SchoolClassDto(
            schoolClass.Id,
            schoolClass.Name,
            schoolClass.LeadingTeacher,
            schoolClass.Students?.Count ?? 0,
            schoolClass.Students?.Select(s => s.ToDto()).ToList() ?? new List<StudentDto>()
        );
    }

    public static Student ToEntity(this CreateStudentDto dto)
    {
        return new Student
        {
            StudentId = dto.StudentId,
            Name = dto.Name,
            Surname = dto.Surname,
            DateOfBirth = dto.DateOfBirth,
            City = dto.City,
            Street = dto.Street,
            PostalCode = dto.PostalCode
        };
    }

    public static void UpdateFromDto(this Student student, UpdateStudentDto dto)
    {
        student.Name = dto.Name;
        student.Surname = dto.Surname;
        student.DateOfBirth = dto.DateOfBirth;
        student.City = dto.City;
        student.Street = dto.Street;
        student.PostalCode = dto.PostalCode;
    }

    public static SchoolClass ToEntity(this CreateSchoolClassDto dto)
    {
        return new SchoolClass
        {
            Name = dto.Name,
            LeadingTeacher = dto.LeadingTeacher
        };
    }

    public static void UpdateFromDto(this SchoolClass schoolClass, UpdateSchoolClassDto dto)
    {
        schoolClass.Name = dto.Name;
        schoolClass.LeadingTeacher = dto.LeadingTeacher;
    }
}
