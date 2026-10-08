using ClassManager.Core.Domain.Achievements;
using ClassManager.Core.Domain.Fees;
using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.UseCases.StudentApp;

public sealed record StudentAppNextClassResponse(
    string Name,
    DateOnly Date,
    string StartTime,
    string EndTime,
    string? InstructorFullName,
    string? Location,
    bool IsPrivateLesson,
    bool IsCancelled,
    Guid? ClassGroupId,
    bool AbsenceNotified,
    bool IsMakeup,
    bool IsPackBooking = false);

public sealed record StudentAppAttendanceResponse(
    int StreakWeeks,
    DateOnly? StreakSince,
    int AttendedClasses,
    IReadOnlyList<AttendanceWeek> RecentWeeks,
    int BestStreakWeeks,
    int Level,
    IReadOnlyList<Medal> Medals);

public sealed record StudentAppFeedbackResponse(DateOnly Date, string ClassName, string? InstructorFullName, string Text);

public sealed record AccountStudentResponse(
    Guid Id,
    string FullName,
    IReadOnlyList<StudentAppNextClassResponse> NextClasses,
    StudentAppAttendanceResponse Attendance,
    StudentAppFeedbackResponse? LatestFeedback);

public sealed record StudentAppMonthlyFeeResponse(string Month, decimal? Fee, decimal Paid, decimal Balance, FeeStatus Status);

public sealed record StudentAppClassBalanceResponse(int AvailableClasses, int UnpaidClasses);

public sealed record StudentAppBillingResponse(
    BillingPlanKind Kind,
    StudentAppMonthlyFeeResponse? MonthlyFee,
    StudentAppClassBalanceResponse? Classes);

public sealed record StudentAppHomeResponse(
    string BusinessName,
    string CurrencyCode,
    string ClientFullName,
    IReadOnlyList<AccountStudentResponse> Students,
    StudentAppBillingResponse Billing,
    IReadOnlyList<LevelDefinition> Levels,
    string SignedInFullName,
    IReadOnlyList<StudentAppGuardianConsentResponse> PendingGuardianConsents);

public sealed record StudentAppGuardianConsentResponse(Guid InvitationId, string StudentFullName, string StudentEmail);

public sealed record StudentAppInvitationResponse(Guid Id, string Email, DateTimeOffset ExpiresAt, bool AwaitsGuardianConsent = false);
