using SchoolManagement.Common;
using SchoolManagement.Extensions;
using SchoolManagement.Models;
using SchoolManagement.Repositories;

namespace SchoolManagement.Services;

/// <summary>
/// Service implementation for student business logic operations.
/// Handles validation, business rules, and coordinates between repositories.
/// </summary>
public class StudentService : IStudentService
{
    private readonly IStudentRepository _studentRepository;
    private readonly ISchoolClassRepository _classRepository;

    /// <summary>
    /// Initializes a new instance of the StudentService.
    /// </summary>
    /// <param name="studentRepository">Repository for student data access.</param>
    /// <param name="classRepository">Repository for class data access (needed for class assignment validation).</param>
    public StudentService(IStudentRepository studentRepository, ISchoolClassRepository classRepository)
    {
        _studentRepository = studentRepository;
        _classRepository = classRepository;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<StudentDto>> GetAllStudentsAsync()
    {
        var students = await _studentRepository.GetAllAsync();
        return students.Select(s => s.ToDto());
    }

    /// <inheritdoc />
    public async Task<ServiceResult<StudentDto>> GetStudentByIdAsync(string studentId)
    {
        var student = await _studentRepository.GetByIdAsync(studentId);
        
        if (student == null)
        {
            return ServiceResult<StudentDto>.NotFound(
                string.Format(ValidationMessages.StudentNotFound, studentId));
        }

        return ServiceResult<StudentDto>.Success(student.ToDto());
    }

    /// <inheritdoc />
    public async Task<ServiceResult<StudentDto>> CreateStudentAsync(CreateStudentDto dto)
    {
        // Validate required fields
        if (string.IsNullOrWhiteSpace(dto.StudentId) || 
            string.IsNullOrWhiteSpace(dto.Name) || 
            string.IsNullOrWhiteSpace(dto.Surname))
        {
            return ServiceResult<StudentDto>.BadRequest(
                $"{ValidationMessages.StudentIdRequired} {ValidationMessages.StudentNameRequired} {ValidationMessages.StudentSurnameRequired}");
        }

        // Check for duplicates
        if (await _studentRepository.ExistsAsync(dto.StudentId))
        {
            return ServiceResult<StudentDto>.Conflict(
                string.Format(ValidationMessages.StudentAlreadyExists, dto.StudentId));
        }

        var student = dto.ToEntity();
        await _studentRepository.AddAsync(student);

        return ServiceResult<StudentDto>.Created(student.ToDto());
    }

    /// <inheritdoc />
    public async Task<ServiceResult<StudentDto>> UpdateStudentAsync(string studentId, UpdateStudentDto dto)
    {
        var student = await _studentRepository.GetByIdAsync(studentId);
        
        if (student == null)
        {
            return ServiceResult<StudentDto>.NotFound(
                string.Format(ValidationMessages.StudentNotFound, studentId));
        }

        // Validate required fields
        if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Surname))
        {
            return ServiceResult<StudentDto>.BadRequest(
                $"{ValidationMessages.StudentNameRequired} {ValidationMessages.StudentSurnameRequired}");
        }

        student.UpdateFromDto(dto);
        await _studentRepository.UpdateAsync(student);

        return ServiceResult<StudentDto>.Success(student.ToDto());
    }

    /// <inheritdoc />
    public async Task<ServiceResult<StudentDto>> PatchStudentAsync(string studentId, PatchStudentDto dto)
    {
        var student = await _studentRepository.GetByIdAsync(studentId);
        
        if (student == null)
        {
            return ServiceResult<StudentDto>.NotFound(
                string.Format(ValidationMessages.StudentNotFound, studentId));
        }

        // Apply partial updates only for provided fields
        if (dto.Name != null)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return ServiceResult<StudentDto>.BadRequest(ValidationMessages.StudentNameRequired);
            }
            student.Name = dto.Name;
        }

        if (dto.Surname != null)
        {
            if (string.IsNullOrWhiteSpace(dto.Surname))
            {
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
                student.SchoolClassId = null;
            }
            else if (classId > 0)
            {
                // Validate class exists
                var schoolClass = await _classRepository.GetByIdWithStudentsAsync(classId);
                if (schoolClass == null)
                {
                    return ServiceResult<StudentDto>.NotFound(
                        string.Format(ValidationMessages.ClassNotFound, classId));
                }

                // Check if student is already in this class
                if (student.SchoolClassId == classId)
                {
                    return ServiceResult<StudentDto>.BadRequest(
                        string.Format(ValidationMessages.StudentAlreadyInClass, student.Name, student.Surname));
                }

                // Check class capacity (only if adding to a different class)
                if (student.SchoolClassId != classId && schoolClass.Students?.Count >= BusinessConstants.MaxStudentsPerClass)
                {
                    return ServiceResult<StudentDto>.BadRequest(
                        string.Format(ValidationMessages.ClassFull, schoolClass.Name, BusinessConstants.MaxStudentsPerClass));
                }

                student.SchoolClassId = classId;
            }
            else
            {
                student.SchoolClassId = null;
            }
        }

        await _studentRepository.UpdateAsync(student);

        return ServiceResult<StudentDto>.Success(student.ToDto());
    }

    /// <inheritdoc />
    public async Task<ServiceResult<string>> DeleteStudentAsync(string studentId)
    {
        var student = await _studentRepository.GetByIdAsync(studentId);
        
        if (student == null)
        {
            return ServiceResult<string>.NotFound(
                string.Format(ValidationMessages.StudentNotFound, studentId));
        }

        await _studentRepository.DeleteAsync(student);

        return ServiceResult<string>.Success(
            string.Format(ValidationMessages.StudentDeleted, studentId));
    }
}
