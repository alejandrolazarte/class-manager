using ClassManager.Core.Domain.Achievements;
using ClassManager.Core.Domain.Businesses;
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
    long? ClassGroupId,
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

public sealed record StudentAppClassMaterialResponse(long ClassGroupId, string ClassName, string? LinkUrl, StudentAppClassMaterialFileResponse? File);

public sealed record StudentAppClassMaterialFileResponse(string Url, long SizeInBytes);

public sealed record AccountStudentResponse(
    long Id,
    string FullName,
    IReadOnlyList<StudentAppNextClassResponse> NextClasses,
    StudentAppAttendanceResponse Attendance,
    StudentAppFeedbackResponse? LatestFeedback,
    IReadOnlyList<StudentAppClassMaterialResponse> Materials);

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
    IReadOnlyList<StudentAppGuardianConsentResponse> PendingGuardianConsents,
    StudentAppLocationResponse? Location);

public sealed record StudentAppLocationResponse(string Name, string? MapsUrl)
{
    public static StudentAppLocationResponse? From(Business business) =>
        business.LocationName is null ? null : new(business.LocationName, business.LocationMapsUrl);
}

public sealed record StudentAppGuardianConsentResponse(long InvitationId, string StudentFullName, string StudentEmail);

public sealed record StudentAppInvitationResponse(long Id, string Email, DateTimeOffset ExpiresAt, bool AwaitsGuardianConsent = false);
