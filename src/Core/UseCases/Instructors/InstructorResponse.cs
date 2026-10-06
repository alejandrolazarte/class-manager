using ClassManager.Core.Domain.Instructors;

namespace ClassManager.Core.UseCases.Instructors;

public sealed record InstructorResponse(Guid Id, string FullName, string? Email, bool IsActive)
{
    public static InstructorResponse From(Instructor instructor) => new(instructor.Id, instructor.FullName, instructor.Email, instructor.IsActive);
}
