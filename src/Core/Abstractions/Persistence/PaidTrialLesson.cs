namespace ClassManager.Core.Abstractions.Persistence;

public sealed record PaidTrialLesson(Guid PrivateLessonId, DateOnly Date, string StudentFullName, decimal TrialPrice);
