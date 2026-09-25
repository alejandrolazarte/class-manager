namespace ClassManager.Core.Domain.Instructors;

public static class InstructorErrorCodes
{
    public const string NotFound = "instructor.not_found";
    public const string NameTaken = "instructor.name_taken";
    public const string Inactive = "instructor.inactive";
    public const string HasActiveClassGroups = "instructor.has_active_class_groups";
    public const string ExistingInstructorIdDetail = "instructorId";
    public const string ClassGroupCountDetail = "classGroupCount";
}
