namespace ClassManager.Core.Abstractions.Security;

public sealed record ClientScope(Guid? InstructorId, Guid RegisteredByUserId);
