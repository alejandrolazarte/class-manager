namespace ClassManager.Core.Domain.Students;

public static class StudentErrorCodes
{
    public const string NotFound = "student.not_found";
    public const string AlreadyRegistered = "student.already_registered";
    public const string DuplicateName = "student.duplicate_name";
    public const string TooMany = "student.too_many";
    public const string EmailOfAnotherPerson = "student.email_of_another_person";
    public const string ExistingStudentIdDetail = "studentId";
}
