namespace ClassManager.Core.Abstractions.Security;

public sealed record InstructorScope(bool IncludesEveryInstructor, Guid? InstructorId)
{
    public static InstructorScope EveryInstructor { get; } = new(true, null);

    public static InstructorScope Only(Guid? instructorId) => new(false, instructorId);

    public bool Includes(Guid instructorId) => IncludesEveryInstructor || InstructorId == instructorId;
}
