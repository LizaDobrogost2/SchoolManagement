using SchoolManagement.Common;
using SchoolManagement.Extensions;
using SchoolManagement.Models;
using SchoolManagement.Repositories;
using Microsoft.Extensions.Logging;

namespace SchoolManagement.Services;

/// <summary>
/// Service implementation for student business logic operations.
/// Handles validation, business rules, and coordinates between repositories.
/// </summary>
public class StudentService : IStudentService
{
    private readonly IStudentRepository _studentRepository;
    private readonly ISchoolClassRepository _classRepository;
    private readonly ILogger<StudentService> _logger;

    /// <summary>
    /// Initializes a new instance of the StudentService.
    /// </summary>
    /// <param name="studentRepository">Repository for student data access.</param>
    /// <param name="classRepository">Repository for class data access (needed for class assignment validation).</param>
    /// <param name="logger">Logger for structured logging.</param>
    public StudentService(
        IStudentRepository studentRepository, 
        ISchoolClassRepository classRepository,
        ILogger<StudentService> logger)
    {
        _studentRepository = studentRepository;
        _classRepository = classRepository;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<StudentDto>> GetAllStudentsAsync()
    {
        _logger.LogInformation("Retrieving all students");
        var students = await _studentRepository.GetAllAsync();
        _logger.LogInformation("Retrieved {Count} students", students.Count());
        return students.Select(s => s.ToDto());
    }

    /// <inheritdoc />
    public async Task<ServiceResult<StudentDto>> GetStudentByIdAsync(string studentId)
    {
        _logger.LogInformation("Retrieving student with ID: {StudentId}", studentId);
        var student = await _studentRepository.GetByIdAsync(studentId);
        
        if (student == null)
        {
            _logger.LogWarning("Student not found: {StudentId}", studentId);
            return ServiceResult<StudentDto>.NotFound(
                string.Format(ValidationMessages.StudentNotFound, studentId));
        }

        _logger.LogInformation("Successfully retrieved student: {StudentId}", studentId);
        return ServiceResult<StudentDto>.Success(student.ToDto());
    }

    /// <inheritdoc />
    public async Task<ServiceResult<StudentDto>> CreateStudentAsync(CreateStudentDto dto)
    {
        _logger.LogInformation("Creating new student with ID: {StudentId}", dto.StudentId);
        
        // Validate required fields
        if (string.IsNullOrWhiteSpace(dto.StudentId) || 
            string.IsNullOrWhiteSpace(dto.Name) || 
            string.IsNullOrWhiteSpace(dto.Surname))
        {
            _logger.LogWarning("Student creation failed: missing required fields for {StudentId}", dto.StudentId);
            return ServiceResult<StudentDto>.BadRequest(
                $"{ValidationMessages.StudentIdRequired} {ValidationMessages.StudentNameRequired} {ValidationMessages.StudentSurnameRequired}");
        }

        // Check for duplicates
        if (await _studentRepository.ExistsAsync(dto.StudentId))
        {
            _logger.LogWarning("Student creation failed: duplicate StudentId {StudentId}", dto.StudentId);
            return ServiceResult<StudentDto>.Conflict(
                string.Format(ValidationMessages.StudentAlreadyExists, dto.StudentId));
        }

        var student = dto.ToEntity();
        await _studentRepository.AddAsync(student);

        _logger.LogInformation("Successfully created student: {StudentId} - {Name} {Surname}", 
            student.StudentId, student.Name, student.Surname);
        return ServiceResult<StudentDto>.Created(student.ToDto());
    }

    /// <inheritdoc />
    public async Task<ServiceResult<StudentDto>> UpdateStudentAsync(string studentId, UpdateStudentDto dto)
    {
        _logger.LogInformation("Updating student: {StudentId}", studentId);
        var student = await _studentRepository.GetByIdAsync(studentId);
        
        if (student == null)
        {
            _logger.LogWarning("Update failed: Student not found {StudentId}", studentId);
            return ServiceResult<StudentDto>.NotFound(
                string.Format(ValidationMessages.StudentNotFound, studentId));
        }

        // Validate required fields
        if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Surname))
        {
            _logger.LogWarning("Update failed: missing required fields for {StudentId}", studentId);
            return ServiceResult<StudentDto>.BadRequest(
                $"{ValidationMessages.StudentNameRequired} {ValidationMessages.StudentSurnameRequired}");
        }

        student.UpdateFromDto(dto);
        await _studentRepository.UpdateAsync(student);

        _logger.LogInformation("Successfully updated student: {StudentId}", studentId);
        return ServiceResult<StudentDto>.Success(student.ToDto());
    }

    /// <inheritdoc />
    public async Task<ServiceResult<StudentDto>> PatchStudentAsync(string studentId, PatchStudentDto dto)
    {
        _logger.LogInformation("Partially updating student: {StudentId}", studentId);
        var student = await _studentRepository.GetByIdAsync(studentId);
        
        if (student == null)
        {
            _logger.LogWarning("Patch failed: Student not found {StudentId}", studentId);
            return ServiceResult<StudentDto>.NotFound(
                string.Format(ValidationMessages.StudentNotFound, studentId));
        }

        // Apply partial updates only for provided fields
        if (dto.Name != null)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                _logger.LogWarning("Patch failed: invalid name for {StudentId}", studentId);
                return ServiceResult<StudentDto>.BadRequest(ValidationMessages.StudentNameRequired);
            }
            student.Name = dto.Name;
        }

        if (dto.Surname != null)
        {
            if (string.IsNullOrWhiteSpace(dto.Surname))
            {
                _logger.LogWarning("Patch failed: invalid surname for {StudentId}", studentId);
                return ServiceResult<StudentDto>.BadRequest(ValidationMessages.StudentSurnameRequired);
            }
            student.Surname = dto.Surname;
        }

        if (dto.DateOfBirth.HasValue)
        {
            student.DateOfBirth = dto.DateOfBirth.Value;
        }

        if (dto.City != null)
        {
            student.City = dto.City;
        }

        if (dto.Street != null)
        {
            student.Street = dto.Street;
        }

        if (dto.PostalCode != null)
        {
            student.PostalCode = dto.PostalCode;
        }

        // Handle class assignment with validation
        if (dto.SchoolClassId.HasValue)
        {
            var classId = dto.SchoolClassId.Value;
            
            // Allow null to unassign from class
            if (classId == 0)
            {
                _logger.LogInformation("Unassigning student {StudentId} from class", studentId);
                student.SchoolClassId = null;
            }
            else if (classId > 0)
            {
                // Validate class exists
                var schoolClass = await _classRepository.GetByIdWithStudentsAsync(classId);
                if (schoolClass == null)
                {
                    _logger.LogWarning("Patch failed: Class not found {ClassId}", classId);
                    return ServiceResult<StudentDto>.NotFound(
                        string.Format(ValidationMessages.ClassNotFound, classId));
                }

                // Check if student is already in this class
                if (student.SchoolClassId == classId)
                {
                    _logger.LogWarning("Patch failed: Student {StudentId} already in class {ClassId}", studentId, classId);
                    return ServiceResult<StudentDto>.BadRequest(
                        string.Format(ValidationMessages.StudentAlreadyInClass, student.Name, student.Surname));
                }

                // Check class capacity (only if adding to a different class)
                if (student.SchoolClassId != classId && schoolClass.Students?.Count >= BusinessConstants.MaxStudentsPerClass)
                {
                    _logger.LogWarning("Patch failed: Class {ClassId} is full (max: {MaxStudents})", 
                        classId, BusinessConstants.MaxStudentsPerClass);
                    return ServiceResult<StudentDto>.BadRequest(
                        string.Format(ValidationMessages.ClassFull, schoolClass.Name, BusinessConstants.MaxStudentsPerClass));
                }

                _logger.LogInformation("Assigning student {StudentId} to class {ClassId}", studentId, classId);
                student.SchoolClassId = classId;
            }
            else
            {
                student.SchoolClassId = null;
            }
        }

        await _studentRepository.UpdateAsync(student);

        _logger.LogInformation("Successfully patched student: {StudentId}", studentId);
        return ServiceResult<StudentDto>.Success(student.ToDto());
    }

    /// <inheritdoc />
    public async Task<ServiceResult<string>> DeleteStudentAsync(string studentId)
    {
        _logger.LogInformation("Deleting student: {StudentId}", studentId);
        var student = await _studentRepository.GetByIdAsync(studentId);
        
        if (student == null)
        {
            _logger.LogWarning("Delete failed: Student not found {StudentId}", studentId);
            return ServiceResult<string>.NotFound(
                string.Format(ValidationMessages.StudentNotFound, studentId));
        }

        await _studentRepository.DeleteAsync(student);

        _logger.LogInformation("Successfully deleted student: {StudentId}", studentId);
        return ServiceResult<string>.Success(
            string.Format(ValidationMessages.StudentDeleted, studentId));
    }
}
