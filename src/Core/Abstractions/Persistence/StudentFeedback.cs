namespace ClassManager.Core.Abstractions.Persistence;

public sealed record StudentFeedback(
    long StudentId, DateOnly Date, string ClassGroupName, long InstructorId, string Text, DateTimeOffset UpdatedAt);
