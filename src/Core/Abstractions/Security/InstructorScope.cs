namespace ClassManager.Core.Abstractions.Security;

public sealed record InstructorScope(bool IncludesEveryInstructor, long? InstructorId)
{
    public static InstructorScope EveryInstructor { get; } = new(true, null);

    public static InstructorScope Only(long? instructorId) => new(false, instructorId);

    public bool Includes(long instructorId) => IncludesEveryInstructor || InstructorId == instructorId;
}
