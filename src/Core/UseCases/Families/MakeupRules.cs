using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.Domain.Makeups;
using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.UseCases.Families;

internal sealed record MissedClass(Guid ClassGroupId, MakeupSource Source);

public sealed record MakeupRepositories(
    IAbsenceNoticeRepository Notices,
    IMakeupBookingRepository Bookings,
    IClassSessionRepository Sessions,
    IEnrollmentRepository Enrollments);

internal static class MakeupRules
{
    public const int LoadAheadDays = 2 * MakeupCredits.ValidityDays;

    public static async Task<IReadOnlyList<MissedClass>> ListMissedClassesAsync(
        MakeupRepositories repositories,
        Guid studentId,
        DateOnly today,
        CancellationToken cancellationToken)
    {
        var firstDate = MakeupCredits.FirstMissedDayToLoad(today);
        var lastDate = today.AddDays(LoadAheadDays);
        var cancelledSessions = (await repositories.Sessions.ListBetweenAsync(firstDate, lastDate, cancellationToken))
            .Where(session => session.IsCancelled)
            .ToList();
        var missedClasses = new List<MissedClass>();
        foreach (var session in cancelledSessions)
        {
            var roster = await repositories.Enrollments.ListRosterOnAsync(session.ClassGroupId, session.Date, cancellationToken);
            if (roster.Any(entry => entry.StudentId == studentId))
            {
                missedClasses.Add(new MissedClass(session.ClassGroupId, new MakeupSource(session.Date, MakeupReason.Cancelled)));
            }
        }

        var cancelledClasses = cancelledSessions.Select(session => (session.ClassGroupId, session.Date)).ToHashSet();
        var notices = await repositories.Notices.ListByStudentsBetweenAsync([studentId], firstDate, lastDate, cancellationToken);
        missedClasses.AddRange(notices
            .Where(notice => !cancelledClasses.Contains((notice.ClassGroupId, notice.Date)))
            .Select(notice => new MissedClass(notice.ClassGroupId, new MakeupSource(notice.Date, MakeupReason.Notice))));
        return missedClasses;
    }

    public static async Task<IReadOnlyList<DateOnly>> ListBookedDatesAsync(
        MakeupRepositories repositories,
        Guid studentId,
        DateOnly today,
        CancellationToken cancellationToken) =>
    [
        .. (await repositories.Bookings.ListByStudentsBetweenAsync(
                [studentId], MakeupCredits.FirstBookingDayToLoad(today), today.AddDays(LoadAheadDays), cancellationToken))
            .Where(booking => !booking.IsCancelled)
            .Select(booking => booking.Date),
    ];

    public static bool HasStarted(ClassGroup classGroup, ClassSession? session, DateOnly date, DateTime localNow) =>
        date.ToDateTime(session?.EffectiveStartTime(classGroup.StartTime) ?? classGroup.StartTime) <= localNow;
}
