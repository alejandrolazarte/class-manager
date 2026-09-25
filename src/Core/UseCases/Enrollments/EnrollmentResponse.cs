using ClassManager.Core.Domain.Enrollments;

namespace ClassManager.Core.UseCases.Enrollments;

public sealed record EnrollmentResponse(Guid Id, Guid ClassGroupId, Guid StudentId, DateOnly StartDate, DateOnly? EndDate)
{
    public static EnrollmentResponse From(Enrollment enrollment) =>
        new(enrollment.Id, enrollment.ClassGroupId, enrollment.StudentId, enrollment.StartDate, enrollment.EndDate);
}
