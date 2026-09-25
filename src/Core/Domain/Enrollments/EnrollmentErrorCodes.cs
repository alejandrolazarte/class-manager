namespace ClassManager.Core.Domain.Enrollments;

public static class EnrollmentErrorCodes
{
    public const string NotFound = "enrollment.not_found";
    public const string AlreadyEnrolled = "enrollment.already_enrolled";
    public const string AlreadyEnded = "enrollment.already_ended";
    public const string ExistingEnrollmentIdDetail = "enrollmentId";
}
