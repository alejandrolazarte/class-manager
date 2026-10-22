namespace ClassManager.Core.Abstractions.Persistence;

public sealed record PaidTrialLesson(long PrivateLessonId, DateOnly Date, string StudentFullName, decimal TrialPrice);
