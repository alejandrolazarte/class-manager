using ClassManager.Core.Domain.Fees;
using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.UseCases.Families;

public sealed record FamilyNextClassResponse(
    string Name,
    DateOnly Date,
    string StartTime,
    string EndTime,
    string? InstructorFullName,
    string? Location,
    bool IsPrivateLesson,
    bool IsCancelled);

public sealed record FamilyAttendanceResponse(
    int StreakWeeks,
    DateOnly? StreakSince,
    int AttendedClasses,
    IReadOnlyList<AttendanceWeek> RecentWeeks);

public sealed record FamilyStudentResponse(
    Guid Id,
    string FullName,
    IReadOnlyList<FamilyNextClassResponse> NextClasses,
    FamilyAttendanceResponse Attendance);

public sealed record FamilyMonthlyFeeResponse(string Month, decimal? Fee, decimal Paid, decimal Balance, FeeStatus Status);

public sealed record FamilyClassBalanceResponse(int AvailableClasses, int UnpaidClasses);

public sealed record FamilyBillingResponse(
    BillingPlanKind Kind,
    FamilyMonthlyFeeResponse? MonthlyFee,
    FamilyClassBalanceResponse? Classes);

public sealed record FamilyHomeResponse(
    string BusinessName,
    string CurrencyCode,
    string ClientFullName,
    IReadOnlyList<FamilyStudentResponse> Students,
    FamilyBillingResponse Billing);

public sealed record FamilyInvitationResponse(Guid Id, string Email, DateTimeOffset ExpiresAt);
