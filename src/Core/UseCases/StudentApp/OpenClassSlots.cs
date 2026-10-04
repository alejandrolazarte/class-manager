using System.Globalization;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.UseCases.StudentApp;

internal sealed record OpenClassSlot(
    Guid ClassGroupId,
    string Name,
    DateOnly Date,
    string StartTime,
    string EndTime,
    string? InstructorFullName,
    string? Location,
    int SpotsLeft);

internal static class OpenClassSlots
{
    public static int SpotsLeft(ClassGroup classGroup, int rosterCount, int notices, int makeups, int packBookings) =>
        classGroup.Capacity - rosterCount + notices - makeups - packBookings;

    public static async Task<int> SpotsLeftAsync(
        ClassGroup classGroup,
        int rosterCount,
        ClassSession? session,
        MakeupRepositories repositories,
        CancellationToken cancellationToken)
    {
        if (session is null)
        {
            return SpotsLeft(classGroup, rosterCount, 0, 0, 0);
        }

        IReadOnlyCollection<Guid> sessionIds = [session.Id];
        var notices = (await repositories.Notices.CountBySessionsAsync(sessionIds, cancellationToken)).GetValueOrDefault(session.Id);
        var makeups = (await repositories.Bookings.CountBySessionsAsync(sessionIds, cancellationToken)).GetValueOrDefault(session.Id);
        var packBookings = (await repositories.PackBookings.CountBySessionsAsync(sessionIds, cancellationToken)).GetValueOrDefault(session.Id);
        return SpotsLeft(classGroup, rosterCount, notices, makeups, packBookings);
    }

    public static async Task<IReadOnlyList<OpenClassSlot>> ListAsync(
        Guid studentId,
        Func<ClassGroup, bool> isOffered,
        IClassGroupRepository classGroupRepository,
        IInstructorRepository instructorRepository,
        MakeupRepositories repositories,
        DateOnly today,
        DateTime localNow,
        CancellationToken cancellationToken)
    {
        var lastDate = today.AddDays(GetStudentAppHomeUseCase.LookAheadDays);
        var sessions = (await repositories.Sessions.ListBetweenAsync(today, lastDate, cancellationToken))
            .ToDictionary(session => (session.ClassGroupId, session.Date));
        IReadOnlyCollection<Guid> sessionIds = [.. sessions.Values.Select(session => session.Id)];
        var noticeCounts = await repositories.Notices.CountBySessionsAsync(sessionIds, cancellationToken);
        var makeupCounts = await repositories.Bookings.CountBySessionsAsync(sessionIds, cancellationToken);
        var packBookingCounts = await repositories.PackBookings.CountBySessionsAsync(sessionIds, cancellationToken);
        var enrollments = await repositories.Enrollments.ListCurrentByStudentAsync(studentId, today, cancellationToken);
        var classGroups = (await classGroupRepository.ListActiveAsync(cancellationToken)).Where(isOffered).ToList();
        var instructorNames = (await instructorRepository.ListAllAsync(cancellationToken))
            .ToDictionary(instructor => instructor.Id, instructor => instructor.FullName);

        var slots = new List<OpenClassSlot>();
        for (var date = today; date <= lastDate; date = date.AddDays(1))
        {
            var enrolledCounts = await repositories.Enrollments.CountActiveOnByClassGroupAsync(date, cancellationToken);
            foreach (var classGroup in classGroups.Where(classGroup => classGroup.Schedule.MeetsOn(date.DayOfWeek)))
            {
                var session = sessions.GetValueOrDefault((classGroup.Id, date));
                if (session?.IsCancelled == true
                    || MakeupRules.HasStarted(classGroup, session, date, localNow)
                    || enrollments.Any(enrollment => enrollment.ClassGroupId == classGroup.Id && enrollment.IsActiveOn(date)))
                {
                    continue;
                }

                var spotsLeft = SpotsLeft(
                    classGroup,
                    enrolledCounts.GetValueOrDefault(classGroup.Id),
                    session is null ? 0 : noticeCounts.GetValueOrDefault(session.Id),
                    session is null ? 0 : makeupCounts.GetValueOrDefault(session.Id),
                    session is null ? 0 : packBookingCounts.GetValueOrDefault(session.Id));
                var startTime = session?.EffectiveStartTime(classGroup.StartTime) ?? classGroup.StartTime;
                slots.Add(new OpenClassSlot(
                    classGroup.Id,
                    classGroup.Name,
                    date,
                    Format(startTime),
                    Format(startTime.AddMinutes(classGroup.DurationMinutes)),
                    instructorNames.GetValueOrDefault(session?.EffectiveInstructorId(classGroup.InstructorId) ?? classGroup.InstructorId),
                    classGroup.Location,
                    Math.Max(spotsLeft, 0)));
            }
        }

        return [.. slots.OrderBy(slot => slot.Date).ThenBy(slot => slot.StartTime, StringComparer.Ordinal)];
    }

    private static string Format(TimeOnly time) => time.ToString(ClassSchedule.TimeFormat, CultureInfo.InvariantCulture);
}
