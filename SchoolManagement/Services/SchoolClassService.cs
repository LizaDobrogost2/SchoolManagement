using SchoolManagement.Common;
using SchoolManagement.Extensions;
using SchoolManagement.Models;
using SchoolManagement.Repositories;

namespace SchoolManagement.Services;

/// <summary>
/// Service implementation for school class business logic operations.
/// Handles validation, business rules, and coordinates between repositories.
/// </summary>
public class SchoolClassService : ISchoolClassService
{
    private readonly ISchoolClassRepository _classRepository;
    private readonly IStudentRepository _studentRepository;

    /// <summary>
    /// Initializes a new instance of the SchoolClassService.
    /// </summary>
    /// <param name="classRepository">Repository for class data access.</param>
    /// <param name="studentRepository">Repository for student data access (needed for student assignment operations).</param>
    public SchoolClassService(
        ISchoolClassRepository classRepository,
        IStudentRepository studentRepository)
    {
        _classRepository = classRepository;
        _studentRepository = studentRepository;
    }

    public async Task<IEnumerable<SchoolClassDto>> GetAllClassesAsync()
    {
        var classes = await _classRepository.GetAllAsync();
        return classes.Select(c => c.ToDto());
    }

    public async Task<ServiceResult<SchoolClassDto>> GetClassByIdAsync(int id)
    {
        var schoolClass = await _classRepository.GetByIdWithStudentsAsync(id);
        
        if (schoolClass == null)
        {
            return ServiceResult<SchoolClassDto>.NotFound(
                string.Format(ValidationMessages.ClassNotFound, id));
        }

        return ServiceResult<SchoolClassDto>.Success(schoolClass.ToDto());
    }

    public async Task<ServiceResult<SchoolClassDto>> CreateClassAsync(CreateSchoolClassDto dto)
    {
        // Validate required fields
        if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.LeadingTeacher))
        {
            return ServiceResult<SchoolClassDto>.BadRequest(
                $"{ValidationMessages.ClassNameRequired} {ValidationMessages.ClassLeadingTeacherRequired}");
        }

        var schoolClass = dto.ToEntity();
        await _classRepository.AddAsync(schoolClass);

        return ServiceResult<SchoolClassDto>.Created(schoolClass.ToDto());
    }

    public async Task<ServiceResult<SchoolClassDto>> UpdateClassAsync(int id, UpdateSchoolClassDto dto)
    {
        var schoolClass = await _classRepository.GetByIdAsync(id);
        
        if (schoolClass == null)
        {
            return ServiceResult<SchoolClassDto>.NotFound(
                string.Format(ValidationMessages.ClassNotFound, id));
        }

        // Validate required fields
        if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.LeadingTeacher))
        {
            return ServiceResult<SchoolClassDto>.BadRequest(
                $"{ValidationMessages.ClassNameRequired} {ValidationMessages.ClassLeadingTeacherRequired}");
        }

        schoolClass.UpdateFromDto(dto);
        await _classRepository.UpdateAsync(schoolClass);

        var studentCount = await _classRepository.GetStudentCountAsync(id);
        var result = schoolClass.ToDto() with { StudentCount = studentCount };

        return ServiceResult<SchoolClassDto>.Success(result);
    }

    public async Task<ServiceResult<SchoolClassDto>> PatchClassAsync(int id, PatchSchoolClassDto dto)
    {
        var schoolClass = await _classRepository.GetByIdAsync(id);
        
        if (schoolClass == null)
        {
            return ServiceResult<SchoolClassDto>.NotFound(
                string.Format(ValidationMessages.ClassNotFound, id));
        }

        // Apply partial updates only for provided fields
        if (dto.Name != null)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return ServiceResult<SchoolClassDto>.BadRequest(ValidationMessages.ClassNameRequired);
            }
            schoolClass.Name = dto.Name;
        }

        if (dto.LeadingTeacher != null)
        {
            if (string.IsNullOrWhiteSpace(dto.LeadingTeacher))
            {
                return ServiceResult<SchoolClassDto>.BadRequest(ValidationMessages.ClassLeadingTeacherRequired);
            }
            schoolClass.LeadingTeacher = dto.LeadingTeacher;
        }

        await _classRepository.UpdateAsync(schoolClass);

        var studentCount = await _classRepository.GetStudentCountAsync(id);
        var result = schoolClass.ToDto() with { StudentCount = studentCount };

        return ServiceResult<SchoolClassDto>.Success(result);
    }

    public async Task<ServiceResult<string>> DeleteClassAsync(int id)
    {
        var schoolClass = await _classRepository.GetByIdWithStudentsAsync(id);
        
        if (schoolClass == null)
        {
            return ServiceResult<string>.NotFound(
                string.Format(ValidationMessages.ClassNotFound, id));
        }

        // Remove class assignment from all students
        if (schoolClass.Students != null)
        {
            foreach (var student in schoolClass.Students)
            {
                student.SchoolClassId = null;
            }
        }

        await _classRepository.DeleteAsync(schoolClass);

        return ServiceResult<string>.Success(
            string.Format(ValidationMessages.ClassDeleted, id));
    }

    public async Task<ServiceResult<string>> AddStudentToClassAsync(int classId, string studentId)
    {
        var schoolClass = await _classRepository.GetByIdWithStudentsAsync(classId);
        
        if (schoolClass == null)
        {
            return ServiceResult<string>.NotFound(
                string.Format(ValidationMessages.ClassNotFound, classId));
        }

        // Check if class is full
        if (schoolClass.Students?.Count >= BusinessConstants.MaxStudentsPerClass)
        {
            return ServiceResult<string>.BadRequest(
                string.Format(ValidationMessages.ClassFull, schoolClass.Name, BusinessConstants.MaxStudentsPerClass));
        }

        var student = await _studentRepository.GetByIdAsync(studentId);
        
        if (student == null)
        {
            return ServiceResult<string>.NotFound(
                string.Format(ValidationMessages.StudentNotFound, studentId));
        }

        // Check if student is already in this class
        if (student.SchoolClassId == classId)
        {
            return ServiceResult<string>.BadRequest(
                string.Format(ValidationMessages.StudentAlreadyInClass, student.Name, student.Surname));
        }

        student.SchoolClassId = classId;
        await _studentRepository.UpdateAsync(student);

        return ServiceResult<string>.Success(
            string.Format(ValidationMessages.StudentAddedToClass, student.Name, student.Surname, schoolClass.Name));
    }

    public async Task<ServiceResult<string>> RemoveStudentFromClassAsync(int classId, string studentId)
    {
        var schoolClass = await _classRepository.GetByIdAsync(classId);
        
        if (schoolClass == null)
        {
            return ServiceResult<string>.NotFound(
                string.Format(ValidationMessages.ClassNotFound, classId));
        }

        var student = await _studentRepository.GetByIdAsync(studentId);
        
        if (student == null)
        {
            return ServiceResult<string>.NotFound(
                string.Format(ValidationMessages.StudentNotFound, studentId));
        }

        if (student.SchoolClassId != classId)
        {
            return ServiceResult<string>.BadRequest(
                string.Format(ValidationMessages.StudentNotInClass, student.Name, student.Surname));
        }

        student.SchoolClassId = null;
        await _studentRepository.UpdateAsync(student);

        return ServiceResult<string>.Success(
            string.Format(ValidationMessages.StudentRemovedFromClass, student.Name, student.Surname, schoolClass.Name));
    }
}
