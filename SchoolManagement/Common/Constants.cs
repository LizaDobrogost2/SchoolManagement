namespace SchoolManagement.Common;

public static class BusinessConstants
{
    public const int MaxStudentsPerClass = 20;
}

public static class ValidationMessages
{
    // Student validation messages
    public const string StudentIdRequired = "StudentId is required.";
    public const string StudentNameRequired = "Name is required.";
    public const string StudentSurnameRequired = "Surname is required.";
    public const string StudentNotFound = "Student with ID '{0}' not found.";
    public const string StudentAlreadyExists = "Student with ID '{0}' already exists.";
    public const string StudentDeleted = "Student with ID '{0}' has been deleted.";
    
    // School class validation messages
    public const string ClassNameRequired = "Name is required.";
    public const string ClassLeadingTeacherRequired = "LeadingTeacher is required.";
    public const string ClassNotFound = "School class with ID {0} not found.";
    public const string ClassDeleted = "School class with ID {0} has been deleted.";
    public const string ClassFull = "Class '{0}' already has the maximum of {1} students.";
    public const string StudentAlreadyInClass = "Student '{0} {1}' is already in this class.";
    public const string StudentNotInClass = "Student '{0} {1}' is not in this class.";
    public const string StudentAddedToClass = "Student '{0} {1}' has been added to class '{2}'.";
    public const string StudentRemovedFromClass = "Student '{0} {1}' has been removed from class '{2}'.";
}
