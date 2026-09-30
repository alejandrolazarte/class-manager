namespace ClassManager.Core.Domain.Sessions;

public sealed record AttendanceStreak(int Weeks, DateOnly? Since, IReadOnlyList<AttendanceWeek> RecentWeeks, int BestWeeks);
