using System.Globalization;
using ClassManager.Core.Abstractions.Fees;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Storage;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Fees;
using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.UseCases.Sessions;

public sealed record GetSessionQuery(long ClassGroupId, DateOnly Date) : IQuery;

public sealed record SessionClassBalanceResponse(int AvailableClasses, int UnpaidClasses)
{
    public static SessionClassBalanceResponse From(ClassBalance balance) => new(balance.AvailableClasses, balance.UnpaidClasses);
}

public sealed record SessionStudentResponse(
    long StudentId,
    string StudentFullName,
    string ClientFullName,
    DateOnly? BirthDate,
    AttendanceStatus? Status,
    string? Feedback,
    bool AbsenceNotified,
    bool IsMakeup,
    bool IsPackBooking = false,
    SessionClassBalanceResponse? ClassBalance = null,
    string? PhotoUrl = null);

public sealed record SessionDetailsResponse(
    long ClassGroupId,
    string ClassGroupName,
    long InstructorId,
    string InstructorFullName,
    string? OriginalInstructorFullName,
    DateOnly Date,
    string StartTime,
    string EndTime,
    string? OriginalStartTime,
    bool IsCancelled,
    string? CancellationReason,
    bool CanTakeAttendance,
    bool CanReschedule,
    bool IsPast,
    IReadOnlyList<SessionStudentResponse> Students,
    string? InstructorPhotoUrl = null);

public sealed class GetSessionUseCase(
    IClassGroupRepository classGroupRepository,
    IInstructorRepository instructorRepository,
    IEnrollmentRepository enrollmentRepository,
    IClassSessionRepository sessionRepository,
    IAttendanceRepository attendanceRepository,
    IClassFeedbackRepository feedbackRepository,
    IAbsenceNoticeRepository absenceNoticeRepository,
    IMakeupBookingRepository makeupBookingRepository,
    IPackBookingRepository packBookingRepository,
    IFamilyBillingService familyBillingService,
    IClassBalanceService classBalanceService,
    IBusinessCalendarService businessCalendar,
    IAccessScopes accessScopes,
    IUserPhotoUrlService userPhotoUrls)
    : IUseCase<GetSessionQuery, SessionDetailsResponse>
{
    public async Task<Result<SessionDetailsResponse>> ExecuteAsync(GetSessionQuery command, CancellationToken cancellationToken)
    {
        var classGroup = await SessionRules.FindScheduledClassGroupAsync(classGroupRepository, command.ClassGroupId, command.Date, cancellationToken);
        if (classGroup.IsFailure)
        {
            return classGroup.Error!;
        }

        var session = await sessionRepository.FindForUpdateAsync(command.ClassGroupId, command.Date, cancellationToken);
        var scope = await accessScopes.ForInstructorsAsync(Permissions.Sessions.ViewAll, cancellationToken);
        if (!SessionRules.IsInScope(scope, classGroup.Value!, session))
        {
            return SessionRules.ClassGroupNotFound();
        }

        var usualInstructor = await instructorRepository.GetByIdAsync(classGroup.Value!.InstructorId, cancellationToken);
        var substitute = session?.SubstituteInstructorId is { } substituteInstructorId
            ? await instructorRepository.GetByIdAsync(substituteInstructorId, cancellationToken)
            : null;
        var instructor = substitute ?? usualInstructor;
        var statuses = session is null
            ? new Dictionary<long, AttendanceStatus>()
            : (await attendanceRepository.ListBySessionAsync(session.Id, cancellationToken))
                .ToDictionary(attendance => attendance.StudentId, attendance => attendance.Status);
        var feedbacks = session is null
            ? new Dictionary<long, string>()
            : (await feedbackRepository.ListBySessionAsync(session.Id, cancellationToken))
                .ToDictionary(feedback => feedback.StudentId, feedback => feedback.Text);
        var notifiedStudentIds = session is null
            ? new HashSet<long>()
            : await absenceNoticeRepository.ListStudentIdsBySessionAsync(session.Id, cancellationToken);
        var today = await businessCalendar.TodayAsync(cancellationToken);
        var isPast = command.Date < today;
        var roster = isPast
            ? await enrollmentRepository.ListRosterOnIncludingDeletedAsync(command.ClassGroupId, command.Date, cancellationToken)
            : await enrollmentRepository.ListRosterOnAsync(command.ClassGroupId, command.Date, cancellationToken);
        var makeupStudents = session is null
            ? []
            : isPast
                ? await makeupBookingRepository.ListStudentsBySessionIncludingDeletedAsync(session.Id, cancellationToken)
                : await makeupBookingRepository.ListStudentsBySessionAsync(session.Id, cancellationToken);
        var packStudents = session is null
            ? []
            : isPast
                ? await packBookingRepository.ListStudentsBySessionIncludingDeletedAsync(session.Id, cancellationToken)
                : await packBookingRepository.ListStudentsBySessionAsync(session.Id, cancellationToken);
        var classBalances = await ClassBalancesOfPayingPerClassAsync(
            [.. roster.Select(entry => entry.ClientId), .. makeupStudents.Select(student => student.ClientId)],
            [.. packStudents.Select(student => student.ClientId)],
            command.Date,
            cancellationToken);
        var studentPhotoUrls = await userPhotoUrls.OfStudentsAsync(
            [
                .. roster.Select(entry => entry.StudentId),
                .. makeupStudents.Select(student => student.StudentId),
                .. packStudents.Select(student => student.StudentId),
            ],
            cancellationToken);
        var instructorPhotoUrls = await userPhotoUrls.OfInstructorsAsync([instructor?.Id], cancellationToken);
        var isCancelled = session?.IsCancelled ?? false;
        var usualStartTime = classGroup.Value!.StartTime;
        var startTime = session?.EffectiveStartTime(usualStartTime) ?? usualStartTime;

        return new SessionDetailsResponse(
            classGroup.Value.Id,
            classGroup.Value.Name,
            session?.EffectiveInstructorId(classGroup.Value.InstructorId) ?? classGroup.Value.InstructorId,
            instructor?.User.FullName ?? string.Empty,
            substitute is null ? null : usualInstructor?.User.FullName,
            command.Date,
            FormatTime(startTime),
            FormatTime(startTime.AddMinutes(classGroup.Value.DurationMinutes)),
            session?.RescheduledStartTime is null ? null : FormatTime(usualStartTime),
            isCancelled,
            session?.CancellationReason,
            CanTakeAttendance: !isCancelled && command.Date <= today,
            CanReschedule: !isCancelled && command.Date >= today,
            IsPast: isPast,
            [
                .. roster.Select(entry => new SessionStudentResponse(
                    entry.StudentId,
                    entry.StudentFullName,
                    entry.ClientFullName,
                    entry.BirthDate,
                    statuses.TryGetValue(entry.StudentId, out var status) ? status : null,
                    feedbacks.GetValueOrDefault(entry.StudentId),
                    notifiedStudentIds.Contains(entry.StudentId),
                    IsMakeup: false,
                    ClassBalance: classBalances.GetValueOrDefault(entry.ClientId),
                    PhotoUrl: studentPhotoUrls.GetValueOrDefault(entry.StudentId))),
                .. makeupStudents.Select(student => new SessionStudentResponse(
                    student.StudentId,
                    student.StudentFullName,
                    student.ClientFullName,
                    student.BirthDate,
                    statuses.TryGetValue(student.StudentId, out var status) ? status : null,
                    feedbacks.GetValueOrDefault(student.StudentId),
                    AbsenceNotified: false,
                    IsMakeup: true,
                    ClassBalance: classBalances.GetValueOrDefault(student.ClientId),
                    PhotoUrl: studentPhotoUrls.GetValueOrDefault(student.StudentId))),
                .. packStudents.Select(student => new SessionStudentResponse(
                    student.StudentId,
                    student.StudentFullName,
                    student.ClientFullName,
                    student.BirthDate,
                    statuses.TryGetValue(student.StudentId, out var status) ? status : null,
                    feedbacks.GetValueOrDefault(student.StudentId),
                    AbsenceNotified: false,
                    IsMakeup: false,
                    IsPackBooking: true,
                    ClassBalance: classBalances.GetValueOrDefault(student.ClientId),
                    PhotoUrl: studentPhotoUrls.GetValueOrDefault(student.StudentId))),
            ],
            instructor is null ? null : instructorPhotoUrls.GetValueOrDefault(instructor.Id));
    }

    private async Task<IReadOnlyDictionary<long, SessionClassBalanceResponse>> ClassBalancesOfPayingPerClassAsync(
        IReadOnlyCollection<long> enrolledClientIds,
        IReadOnlyCollection<long> packBookingClientIds,
        DateOnly sessionDate,
        CancellationToken cancellationToken)
    {
        var month = BillingMonth.From(sessionDate);
        var billingByClient = await familyBillingService.CalculateAsync(enrolledClientIds, month, cancellationToken);
        var payingPerClassClientIds = enrolledClientIds
            .Distinct()
            .Where(clientId => !billingByClient[clientId].HasMonthlyPlan)
            .Concat(packBookingClientIds)
            .Distinct()
            .ToList();
        var balances = await classBalanceService.CalculateAsync(payingPerClassClientIds, cancellationToken);
        return balances.ToDictionary(balance => balance.Key, balance => SessionClassBalanceResponse.From(balance.Value));
    }

    private static string FormatTime(TimeOnly time) => time.ToString(ClassSchedule.TimeFormat, CultureInfo.InvariantCulture);
}
