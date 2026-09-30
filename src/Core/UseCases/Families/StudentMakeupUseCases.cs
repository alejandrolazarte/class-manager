using System.Globalization;
using ClassManager.Core.Abstractions.Notifications;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.Domain.Makeups;
using ClassManager.Core.Domain.Sessions;
using ClassManager.Core.UseCases.Sessions;

namespace ClassManager.Core.UseCases.Families;

public sealed record GetFamilyMakeupsQuery(Guid StudentId);

public sealed record BookMakeupCommand(Guid StudentId, Guid ClassGroupId, DateOnly Date);

public sealed record CancelMakeupCommand(Guid StudentId, Guid ClassGroupId, DateOnly Date);

public sealed class GetFamilyMakeupsUseCase(
    IFamilyAccess familyAccess,
    IStudentRepository studentRepository,
    IClassGroupRepository classGroupRepository,
    IInstructorRepository instructorRepository,
    MakeupRepositories makeupRepositories,
    IBusinessCalendarService businessCalendar)
    : IUseCase<GetFamilyMakeupsQuery, FamilyMakeupsResponse>
{
    public async Task<Result<FamilyMakeupsResponse>> ExecuteAsync(GetFamilyMakeupsQuery command, CancellationToken cancellationToken)
    {
        var studentError = await FamilyStudentRules.FindOwnStudentErrorAsync(familyAccess, studentRepository, command.StudentId, cancellationToken);
        if (studentError is not null)
        {
            return studentError;
        }

        var today = await businessCalendar.TodayAsync(cancellationToken);
        var localNow = await businessCalendar.LocalNowAsync(cancellationToken);
        var lastDate = today.AddDays(GetFamilyHomeUseCase.LookAheadDays);
        var missedClasses = await MakeupRules.ListMissedClassesAsync(makeupRepositories, command.StudentId, today, cancellationToken);
        var bookedDates = await MakeupRules.ListBookedDatesAsync(makeupRepositories, command.StudentId, today, cancellationToken);
        var balance = MakeupCredits.Calculate([.. missedClasses.Select(missed => missed.Source)], bookedDates, today);

        var sessions = (await makeupRepositories.Sessions.ListBetweenAsync(today, lastDate, cancellationToken))
            .ToDictionary(session => (session.ClassGroupId, session.Date));
        IReadOnlyCollection<Guid> sessionIds = [.. sessions.Values.Select(session => session.Id)];
        var noticeCounts = await makeupRepositories.Notices.CountBySessionsAsync(sessionIds, cancellationToken);
        var makeupCounts = await makeupRepositories.Bookings.CountBySessionsAsync(sessionIds, cancellationToken);
        var booked = (await makeupRepositories.Bookings.ListByStudentsBetweenAsync([command.StudentId], today, lastDate, cancellationToken))
            .Select(booking => (booking.ClassGroupId, booking.Date))
            .ToHashSet();
        var enrollments = await makeupRepositories.Enrollments.ListCurrentByStudentAsync(command.StudentId, today, cancellationToken);
        var classGroups = await classGroupRepository.ListActiveAsync(cancellationToken);
        var instructorNames = (await instructorRepository.ListAllAsync(cancellationToken))
            .ToDictionary(instructor => instructor.Id, instructor => instructor.FullName);

        var slots = new List<FamilyMakeupSlotResponse>();
        for (var date = today; date <= lastDate; date = date.AddDays(1))
        {
            var enrolledCounts = await makeupRepositories.Enrollments.CountActiveOnByClassGroupAsync(date, cancellationToken);
            foreach (var classGroup in classGroups.Where(classGroup => classGroup.Schedule.MeetsOn(date.DayOfWeek)))
            {
                var session = sessions.GetValueOrDefault((classGroup.Id, date));
                if (session?.IsCancelled == true
                    || MakeupRules.HasStarted(classGroup, session, date, localNow)
                    || enrollments.Any(enrollment => enrollment.ClassGroupId == classGroup.Id && enrollment.IsActiveOn(date)))
                {
                    continue;
                }

                var spotsLeft = classGroup.Capacity
                    - enrolledCounts.GetValueOrDefault(classGroup.Id)
                    + (session is null ? 0 : noticeCounts.GetValueOrDefault(session.Id))
                    - (session is null ? 0 : makeupCounts.GetValueOrDefault(session.Id));
                var startTime = session?.EffectiveStartTime(classGroup.StartTime) ?? classGroup.StartTime;
                slots.Add(new FamilyMakeupSlotResponse(
                    classGroup.Id,
                    classGroup.Name,
                    date,
                    Format(startTime),
                    Format(startTime.AddMinutes(classGroup.DurationMinutes)),
                    instructorNames.GetValueOrDefault(session?.EffectiveInstructorId(classGroup.InstructorId) ?? classGroup.InstructorId),
                    classGroup.Location,
                    Math.Max(spotsLeft, 0),
                    booked.Contains((classGroup.Id, date))));
            }
        }

        return new FamilyMakeupsResponse(
            [.. balance.Available.Select(credit => new FamilyMakeupCreditResponse(credit.MissedOn, credit.Reason, credit.ExpiresOn))],
            [.. slots.OrderBy(slot => slot.Date).ThenBy(slot => slot.StartTime, StringComparer.Ordinal)]);
    }

    private static string Format(TimeOnly time) => time.ToString(ClassSchedule.TimeFormat, CultureInfo.InvariantCulture);
}

public sealed class BookMakeupUseCase(
    IFamilyAccess familyAccess,
    IStudentRepository studentRepository,
    IClassGroupRepository classGroupRepository,
    MakeupRepositories makeupRepositories,
    IUnitOfWork unitOfWork,
    IBusinessCalendarService businessCalendar,
    ITeamNotificationService teamNotifications,
    TimeProvider timeProvider)
    : IUseCase<BookMakeupCommand, bool>
{
    private const string AlreadyInClassMessage = "The student already has this class.";
    private const string CancelledMessage = "The class is cancelled on that date.";
    private const string ClassStartedMessage = "The class already started.";
    private const string NoCreditMessage = "The student has no class to make up.";
    private const string FullMessage = "The class has no room left.";
    private const string ConcurrentUpdateMessage = "The class changed at the same time. Try again.";

    public async Task<Result<bool>> ExecuteAsync(BookMakeupCommand command, CancellationToken cancellationToken)
    {
        var studentError = await FamilyStudentRules.FindOwnStudentErrorAsync(familyAccess, studentRepository, command.StudentId, cancellationToken);
        if (studentError is not null)
        {
            return studentError;
        }

        var classGroup = await SessionRules.FindScheduledClassGroupAsync(classGroupRepository, command.ClassGroupId, command.Date, cancellationToken);
        if (classGroup.IsFailure)
        {
            return classGroup.Error!;
        }

        if (!classGroup.Value!.IsActive)
        {
            return SessionRules.ClassGroupNotFound();
        }

        var roster = await makeupRepositories.Enrollments.ListRosterOnAsync(command.ClassGroupId, command.Date, cancellationToken);
        if (roster.Any(entry => entry.StudentId == command.StudentId))
        {
            return Result.Validation<bool>(AlreadyInClassMessage, SessionErrorCodes.MakeupAlreadyInClass, nameof(BookMakeupCommand.ClassGroupId));
        }

        var session = await makeupRepositories.Sessions.FindForUpdateAsync(command.ClassGroupId, command.Date, cancellationToken);
        if (session?.IsCancelled == true)
        {
            return Result.Conflict<bool>(CancelledMessage, SessionErrorCodes.Cancelled);
        }

        if (MakeupRules.HasStarted(classGroup.Value, session, command.Date, await businessCalendar.LocalNowAsync(cancellationToken)))
        {
            return Result.Validation<bool>(ClassStartedMessage, SessionErrorCodes.ClassStarted, nameof(BookMakeupCommand.Date));
        }

        if (session is not null && await makeupRepositories.Bookings.FindForUpdateAsync(session.Id, command.StudentId, cancellationToken) is not null)
        {
            return true;
        }

        var today = await businessCalendar.TodayAsync(cancellationToken);
        IReadOnlyCollection<MakeupSource> sources =
            [.. (await MakeupRules.ListMissedClassesAsync(makeupRepositories, command.StudentId, today, cancellationToken)).Select(missed => missed.Source)];
        var bookedDates = await MakeupRules.ListBookedDatesAsync(makeupRepositories, command.StudentId, today, cancellationToken);
        if (MakeupCredits.Calculate(sources, [.. bookedDates, command.Date], today).UncoveredBookings
            > MakeupCredits.Calculate(sources, bookedDates, today).UncoveredBookings)
        {
            return Result.Conflict<bool>(NoCreditMessage, SessionErrorCodes.MakeupNoCredit);
        }

        var notices = session is null ? 0 : (await makeupRepositories.Notices.CountBySessionsAsync([session.Id], cancellationToken)).GetValueOrDefault(session.Id);
        var makeups = session is null ? 0 : (await makeupRepositories.Bookings.CountBySessionsAsync([session.Id], cancellationToken)).GetValueOrDefault(session.Id);
        if (classGroup.Value.Capacity - roster.Count + notices - makeups <= 0)
        {
            return Result.Conflict<bool>(FullMessage, SessionErrorCodes.MakeupFull);
        }

        var now = timeProvider.GetUtcNow();
        if (session is null)
        {
            session = ClassSession.Create(command.ClassGroupId, command.Date, now);
            makeupRepositories.Sessions.Add(session);
        }

        makeupRepositories.Bookings.Add(MakeupBooking.Create(session.Id, command.StudentId, now));
        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (UniqueConstraintViolationException)
        {
            return Result.Conflict<bool>(ConcurrentUpdateMessage, SessionErrorCodes.ConcurrentUpdate);
        }

        await teamNotifications.MakeupBookedAsync(command.StudentId, classGroup.Value, session, cancellationToken);
        return true;
    }
}

public sealed class CancelMakeupUseCase(
    IFamilyAccess familyAccess,
    IStudentRepository studentRepository,
    IClassGroupRepository classGroupRepository,
    MakeupRepositories makeupRepositories,
    IUnitOfWork unitOfWork,
    IBusinessCalendarService businessCalendar)
    : IUseCase<CancelMakeupCommand, bool>
{
    private const string ClassStartedMessage = "The class already started.";

    public async Task<Result<bool>> ExecuteAsync(CancelMakeupCommand command, CancellationToken cancellationToken)
    {
        var studentError = await FamilyStudentRules.FindOwnStudentErrorAsync(familyAccess, studentRepository, command.StudentId, cancellationToken);
        if (studentError is not null)
        {
            return studentError;
        }

        var classGroup = await SessionRules.FindScheduledClassGroupAsync(classGroupRepository, command.ClassGroupId, command.Date, cancellationToken);
        if (classGroup.IsFailure)
        {
            return classGroup.Error!;
        }

        var session = await makeupRepositories.Sessions.FindForUpdateAsync(command.ClassGroupId, command.Date, cancellationToken);
        var booking = session is null ? null : await makeupRepositories.Bookings.FindForUpdateAsync(session.Id, command.StudentId, cancellationToken);
        if (booking is null)
        {
            return true;
        }

        if (MakeupRules.HasStarted(classGroup.Value!, session, command.Date, await businessCalendar.LocalNowAsync(cancellationToken)))
        {
            return Result.Validation<bool>(ClassStartedMessage, SessionErrorCodes.ClassStarted, nameof(CancelMakeupCommand.Date));
        }

        makeupRepositories.Bookings.Remove(booking);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
