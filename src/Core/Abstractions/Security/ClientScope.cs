namespace ClassManager.Core.Abstractions.Security;

public sealed record ClientScope(long? InstructorId, long RegisteredByUserId);
