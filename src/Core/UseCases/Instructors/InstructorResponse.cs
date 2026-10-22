using ClassManager.Core.Domain.Instructors;

namespace ClassManager.Core.UseCases.Instructors;

public sealed record InstructorResponse(long Id, string FullName, string? Email, bool IsActive, long? SamePersonClientId = null)
{
    public static InstructorResponse From(Instructor instructor) => new(instructor.Id, instructor.User.FullName, instructor.User.Email, instructor.IsActive);
}
