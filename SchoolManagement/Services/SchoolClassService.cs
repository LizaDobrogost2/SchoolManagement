using SchoolManagement.Common;
using SchoolManagement.Extensions;
using SchoolManagement.Models;
using SchoolManagement.Repositories;
using Microsoft.Extensions.Logging;

namespace SchoolManagement.Services;

/// <summary>
/// Service implementation for school class business logic operations.
/// Handles validation, business rules, and coordinates between repositories.
/// </summary>
public class SchoolClassService : ISchoolClassService
{
    private readonly ISchoolClassRepository _classRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly ILogger<SchoolClassService> _logger;

    /// <summary>
    /// Initializes a new instance of the SchoolClassService.
    /// </summary>
    /// <param name="classRepository">Repository for class data access.</param>
    /// <param name="studentRepository">Repository for student data access (needed for student assignment operations).</param>
    /// <param name="logger">Logger for structured logging.</param>
    public SchoolClassService(
        ISchoolClassRepository classRepository,
        IStudentRepository studentRepository,
        ILogger<SchoolClassService> logger)
    {
        _classRepository = classRepository;
        _studentRepository = studentRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<SchoolClassDto>> GetAllClassesAsync()
    {
        _logger.LogInformation("Retrieving all classes");
        var classes = await _classRepository.GetAllAsync();
        _logger.LogInformation("Retrieved {Count} classes", classes.Count());
        return classes.Select(c => c.ToDto());
    }

    public async Task<ServiceResult<SchoolClassDto>> GetClassByIdAsync(int id)
    {
        _logger.LogInformation("Retrieving class with ID: {ClassId}", id);
        var schoolClass = await _classRepository.GetByIdWithStudentsAsync(id);
        
        if (schoolClass == null)
        {
            _logger.LogWarning("Class not found: {ClassId}", id);
            return ServiceResult<SchoolClassDto>.NotFound(
                string.Format(ValidationMessages.ClassNotFound, id));
        }

        _logger.LogInformation("Successfully retrieved class: {ClassId} - {ClassName}", id, schoolClass.Name);
        return ServiceResult<SchoolClassDto>.Success(schoolClass.ToDto());
    }

    public async Task<ServiceResult<SchoolClassDto>> CreateClassAsync(CreateSchoolClassDto dto)
    {
        _logger.LogInformation("Creating new class: {ClassName}", dto.Name);
        
        // Validate required fields
        if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.LeadingTeacher))
        {
            _logger.LogWarning("Class creation failed: missing required fields");
            return ServiceResult<SchoolClassDto>.BadRequest(
                $"{ValidationMessages.ClassNameRequired} {ValidationMessages.ClassLeadingTeacherRequired}");
        }

        var schoolClass = dto.ToEntity();
        await _classRepository.AddAsync(schoolClass);

        _logger.LogInformation("Successfully created class: {ClassId} - {ClassName} (Teacher: {Teacher})", 
            schoolClass.Id, schoolClass.Name, schoolClass.LeadingTeacher);
        return ServiceResult<SchoolClassDto>.Created(schoolClass.ToDto());
    }

    public async Task<ServiceResult<SchoolClassDto>> UpdateClassAsync(int id, UpdateSchoolClassDto dto)
    {
        _logger.LogInformation("Updating class: {ClassId}", id);
        var schoolClass = await _classRepository.GetByIdAsync(id);
        
        if (schoolClass == null)
        {
            _logger.LogWarning("Update failed: Class not found {ClassId}", id);
            return ServiceResult<SchoolClassDto>.NotFound(
                string.Format(ValidationMessages.ClassNotFound, id));
        }

        // Validate required fields
        if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.LeadingTeacher))
        {
            _logger.LogWarning("Update failed: missing required fields for {ClassId}", id);
            return ServiceResult<SchoolClassDto>.BadRequest(
                $"{ValidationMessages.ClassNameRequired} {ValidationMessages.ClassLeadingTeacherRequired}");
        }

        schoolClass.UpdateFromDto(dto);
        await _classRepository.UpdateAsync(schoolClass);

        var studentCount = await _classRepository.GetStudentCountAsync(id);
        var result = schoolClass.ToDto() with { StudentCount = studentCount };

        _logger.LogInformation("Successfully updated class: {ClassId} - {ClassName}", id, schoolClass.Name);
        return ServiceResult<SchoolClassDto>.Success(result);
    }

    public async Task<ServiceResult<SchoolClassDto>> PatchClassAsync(int id, PatchSchoolClassDto dto)
    {
        _logger.LogInformation("Partially updating class: {ClassId}", id);
        var schoolClass = await _classRepository.GetByIdAsync(id);
        
        if (schoolClass == null)
        {
            _logger.LogWarning("Patch failed: Class not found {ClassId}", id);
            return ServiceResult<SchoolClassDto>.NotFound(
                string.Format(ValidationMessages.ClassNotFound, id));
        }

        // Apply partial updates only for provided fields
        if (dto.Name != null)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                _logger.LogWarning("Patch failed: invalid name for {ClassId}", id);
                return ServiceResult<SchoolClassDto>.BadRequest(ValidationMessages.ClassNameRequired);
            }
            schoolClass.Name = dto.Name;
        }

        if (dto.LeadingTeacher != null)
        {
            if (string.IsNullOrWhiteSpace(dto.LeadingTeacher))
            {
                _logger.LogWarning("Patch failed: invalid teacher for {ClassId}", id);
                return ServiceResult<SchoolClassDto>.BadRequest(ValidationMessages.ClassLeadingTeacherRequired);
            }
            schoolClass.LeadingTeacher = dto.LeadingTeacher;
        }

        await _classRepository.UpdateAsync(schoolClass);

        var studentCount = await _classRepository.GetStudentCountAsync(id);
        var result = schoolClass.ToDto() with { StudentCount = studentCount };

        _logger.LogInformation("Successfully patched class: {ClassId}", id);
        return ServiceResult<SchoolClassDto>.Success(result);
    }

    public async Task<ServiceResult<string>> DeleteClassAsync(int id)
    {
        _logger.LogInformation("Deleting class: {ClassId}", id);
        var schoolClass = await _classRepository.GetByIdWithStudentsAsync(id);
        
        if (schoolClass == null)
        {
            _logger.LogWarning("Delete failed: Class not found {ClassId}", id);
            return ServiceResult<string>.NotFound(
                string.Format(ValidationMessages.ClassNotFound, id));
        }

        // Remove class assignment from all students
        if (schoolClass.Students != null)
        {
            var studentCount = schoolClass.Students.Count;
            _logger.LogInformation("Unassigning {StudentCount} students from class {ClassId}", studentCount, id);
            foreach (var student in schoolClass.Students)
            {
                student.SchoolClassId = null;
            }
        }

        await _classRepository.DeleteAsync(schoolClass);

        _logger.LogInformation("Successfully deleted class: {ClassId}", id);
        return ServiceResult<string>.Success(
            string.Format(ValidationMessages.ClassDeleted, id));
    }

    public async Task<ServiceResult<string>> AddStudentToClassAsync(int classId, string studentId)
    {
        _logger.LogInformation("Adding student {StudentId} to class {ClassId}", studentId, classId);
        var schoolClass = await _classRepository.GetByIdWithStudentsAsync(classId);
        
        if (schoolClass == null)
        {
            _logger.LogWarning("Add student failed: Class not found {ClassId}", classId);
            return ServiceResult<string>.NotFound(
                string.Format(ValidationMessages.ClassNotFound, classId));
        }

        // Check if class is full
        if (schoolClass.Students?.Count >= BusinessConstants.MaxStudentsPerClass)
        {
            _logger.LogWarning("Add student failed: Class {ClassId} is full (max: {MaxStudents})", 
                classId, BusinessConstants.MaxStudentsPerClass);
            return ServiceResult<string>.BadRequest(
                string.Format(ValidationMessages.ClassFull, schoolClass.Name, BusinessConstants.MaxStudentsPerClass));
        }

        var student = await _studentRepository.GetByIdAsync(studentId);
        
        if (student == null)
        {
            _logger.LogWarning("Add student failed: Student not found {StudentId}", studentId);
            return ServiceResult<string>.NotFound(
                string.Format(ValidationMessages.StudentNotFound, studentId));
        }

        // Check if student is already in this class
        if (student.SchoolClassId == classId)
        {
            _logger.LogWarning("Add student failed: Student {StudentId} already in class {ClassId}", studentId, classId);
            return ServiceResult<string>.BadRequest(
                string.Format(ValidationMessages.StudentAlreadyInClass, student.Name, student.Surname));
        }

        student.SchoolClassId = classId;
        await _studentRepository.UpdateAsync(student);

        _logger.LogInformation("Successfully added student {StudentId} to class {ClassId}", studentId, classId);
        return ServiceResult<string>.Success(
            string.Format(ValidationMessages.StudentAddedToClass, student.Name, student.Surname, schoolClass.Name));
    }

    public async Task<ServiceResult<string>> RemoveStudentFromClassAsync(int classId, string studentId)
    {
        _logger.LogInformation("Removing student {StudentId} from class {ClassId}", studentId, classId);
        var schoolClass = await _classRepository.GetByIdAsync(classId);
        
        if (schoolClass == null)
        {
            _logger.LogWarning("Remove student failed: Class not found {ClassId}", classId);
            return ServiceResult<string>.NotFound(
                string.Format(ValidationMessages.ClassNotFound, classId));
        }

        var student = await _studentRepository.GetByIdAsync(studentId);
        
        if (student == null)
        {
            _logger.LogWarning("Remove student failed: Student not found {StudentId}", studentId);
            return ServiceResult<string>.NotFound(
                string.Format(ValidationMessages.StudentNotFound, studentId));
        }

        if (student.SchoolClassId != classId)
        {
            _logger.LogWarning("Remove student failed: Student {StudentId} not in class {ClassId}", studentId, classId);
            return ServiceResult<string>.BadRequest(
                string.Format(ValidationMessages.StudentNotInClass, student.Name, student.Surname));
        }

        student.SchoolClassId = null;
        await _studentRepository.UpdateAsync(student);

        _logger.LogInformation("Successfully removed student {StudentId} from class {ClassId}", studentId, classId);
        return ServiceResult<string>.Success(
            string.Format(ValidationMessages.StudentRemovedFromClass, student.Name, student.Surname, schoolClass.Name));
    }
}
