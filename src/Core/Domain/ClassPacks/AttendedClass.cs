namespace ClassManager.Core.Domain.ClassPacks;

public sealed record AttendedClass(DateOnly Date, string StudentFullName, string ClassGroupName, bool IsPrivateLesson = false);
