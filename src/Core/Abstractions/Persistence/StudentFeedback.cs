namespace ClassManager.Core.Abstractions.Persistence;

public sealed record StudentFeedback(Guid StudentId, DateOnly Date, string ClassGroupName, Guid InstructorId, string Text);
