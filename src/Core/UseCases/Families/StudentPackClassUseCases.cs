using ClassManager.Core.Abstractions.Fees;
using ClassManager.Core.Abstractions.Notifications;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Sessions;
using ClassManager.Core.UseCases.Sessions;

namespace ClassManager.Core.UseCases.Families;

public sealed record GetFamilyPackClassesQuery(Guid StudentId);

public sealed record BookPackClassCommand(Guid StudentId, Guid ClassGroupId, DateOnly Date);

public sealed record CancelPackClassCommand(Guid StudentId, Guid ClassGroupId, DateOnly Date);

public sealed record PackCreditRepositories(
    IStudentRepository Students,
    IClassPackRepository ClassPacks,
    IClassBalanceService ClassBalances);

internal sealed record PackCoverage(ClassPackUsage Usage, ClassPack ClassPack);

internal static class PackCredits
{
    public static async Task<IReadOnlyList<PackCoverage>> ListActiveAsync(
        Guid clientId,
        PackCreditRepositories repositories,
        CancellationToken cancellationToken)
    {
        var balances = await repositories.ClassBalances.CalculateAsync([clientId], cancellationToken);
        var coverages = new List<PackCoverage>();
        foreach (var usage in balances[clientId].Purchases.Where(usage => usage.Status == ClassPackPurchaseStatus.Active))
        {
            if (await repositories.ClassPacks.GetByIdAsync(usage.Purchase.ClassPackId, cancellationToken) is { ClassGroupIds.Count: > 0 } classPack)
            {
                coverages.Add(new PackCoverage(usage, classPack));
            }
        }

        return coverages;
    }

    public static async Task<int> WaitingBookingsAsync(
        Guid clientId,
        PackCreditRepositories repositories,
        IPackBookingRepository packBookingRepository,
        CancellationToken cancellationToken)
    {
        var studentIds = (await repositories.Students.ListByClientAsync(clientId, cancellationToken)).Select(student => student.Id).ToList();
        return await packBookingRepository.CountWaitingForAttendanceAsync(studentIds, cancellationToken);
    }

    public static int ClassesLeft(IEnumerable<PackCoverage> coverages, int waitingBookings) =>
        Math.Max(coverages.Sum(coverage => coverage.Usage.RemainingClasses) - waitingBookings, 0);
}

public sealed class GetFamilyPackClassesUseCase(
    IFamilyAccess familyAccess,
    IClassGroupRepository classGroupRepository,
    IInstructorRepository instructorRepository,
    MakeupRepositories makeupRepositories,
    PackCreditRepositories packCreditRepositories,
    IBusinessCalendarService businessCalendar)
    : IUseCase<GetFamilyPackClassesQuery, FamilyPackClassesResponse>
{
    public async Task<Result<FamilyPackClassesResponse>> ExecuteAsync(GetFamilyPackClassesQuery command, CancellationToken cancellationToken)
    {
        var studentError = await FamilyStudentRules.FindOwnStudentErrorAsync(
            familyAccess, packCreditRepositories.Students, command.StudentId, cancellationToken);
        if (studentError is not null)
        {
            return studentError;
        }

        var clientId = (await familyAccess.GetAsync(cancellationToken))!.ClientId;
        var coverages = await PackCredits.ListActiveAsync(clientId, packCreditRepositories, cancellationToken);
        var waitingBookings = await PackCredits.WaitingBookingsAsync(clientId, packCreditRepositories, makeupRepositories.PackBookings, cancellationToken);
        var coveredClassGroupIds = coverages.SelectMany(coverage => coverage.ClassPack.ClassGroupIds).ToHashSet();

        var today = await businessCalendar.TodayAsync(cancellationToken);
        var localNow = await businessCalendar.LocalNowAsync(cancellationToken);
        var booked = (await makeupRepositories.PackBookings.ListByStudentsBetweenAsync(
                [command.StudentId], today, today.AddDays(GetFamilyHomeUseCase.LookAheadDays), cancellationToken))
            .Select(booking => (booking.ClassGroupId, booking.Date))
            .ToHashSet();
        coveredClassGroupIds.UnionWith(booked.Select(booking => booking.ClassGroupId));
        var slots = await OpenClassSlots.ListAsync(
            command.StudentId,
            classGroup => coveredClassGroupIds.Contains(classGroup.Id),
            classGroupRepository,
            instructorRepository,
            makeupRepositories,
            today,
            localNow,
            cancellationToken);

        return new FamilyPackClassesResponse(
            PackCredits.ClassesLeft(coverages, waitingBookings),
            [.. slots.Select(slot => new FamilyClassSlotResponse(
                slot.ClassGroupId,
                slot.Name,
                slot.Date,
                slot.StartTime,
                slot.EndTime,
                slot.InstructorFullName,
                slot.Location,
                slot.SpotsLeft,
                booked.Contains((slot.ClassGroupId, slot.Date))))]);
    }
}

public sealed class BookPackClassUseCase(
    IFamilyAccess familyAccess,
    IClassGroupRepository classGroupRepository,
    MakeupRepositories makeupRepositories,
    PackCreditRepositories packCreditRepositories,
    IUnitOfWork unitOfWork,
    IBusinessCalendarService businessCalendar,
    ITeamNotificationService teamNotifications,
    TimeProvider timeProvider)
    : IUseCase<BookPackClassCommand, bool>
{
    private const string AlreadyInClassMessage = "The student already has this class.";
    private const string CancelledMessage = "The class is cancelled on that date.";
    private const string ClassStartedMessage = "The class already started.";
    private const string NoClassesMessage = "No pack of the family has classes left for this class.";
    private const string FullMessage = "The class has no room left.";
    private const string ConcurrentUpdateMessage = "The class changed at the same time. Try again.";

    public async Task<Result<bool>> ExecuteAsync(BookPackClassCommand command, CancellationToken cancellationToken)
    {
        var studentError = await FamilyStudentRules.FindOwnStudentErrorAsync(
            familyAccess, packCreditRepositories.Students, command.StudentId, cancellationToken);
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
            return Result.Validation<bool>(AlreadyInClassMessage, SessionErrorCodes.MakeupAlreadyInClass, nameof(BookPackClassCommand.ClassGroupId));
        }

        var session = await makeupRepositories.Sessions.FindForUpdateAsync(command.ClassGroupId, command.Date, cancellationToken);
        if (session?.IsCancelled == true)
        {
            return Result.Conflict<bool>(CancelledMessage, SessionErrorCodes.Cancelled);
        }

        if (MakeupRules.HasStarted(classGroup.Value, session, command.Date, await businessCalendar.LocalNowAsync(cancellationToken)))
        {
            return Result.Validation<bool>(ClassStartedMessage, SessionErrorCodes.ClassStarted, nameof(BookPackClassCommand.Date));
        }

        if (session is not null && await makeupRepositories.PackBookings.FindForUpdateAsync(session.Id, command.StudentId, cancellationToken) is not null)
        {
            return true;
        }

        var clientId = (await familyAccess.GetAsync(cancellationToken))!.ClientId;
        var coverages = (await PackCredits.ListActiveAsync(clientId, packCreditRepositories, cancellationToken))
            .Where(coverage => coverage.ClassPack.CoversClassGroup(command.ClassGroupId) && coverage.Usage.Purchase.IsValidOn(command.Date));
        var waitingBookings = await PackCredits.WaitingBookingsAsync(clientId, packCreditRepositories, makeupRepositories.PackBookings, cancellationToken);
        if (PackCredits.ClassesLeft(coverages, waitingBookings) <= 0)
        {
            return Result.Conflict<bool>(NoClassesMessage, SessionErrorCodes.PackNoClasses);
        }

        if (await OpenClassSlots.SpotsLeftAsync(classGroup.Value, roster.Count, session, makeupRepositories, cancellationToken) <= 0)
        {
            return Result.Conflict<bool>(FullMessage, SessionErrorCodes.MakeupFull);
        }

        var now = timeProvider.GetUtcNow();
        if (session is null)
        {
            session = ClassSession.Create(command.ClassGroupId, command.Date, now);
            makeupRepositories.Sessions.Add(session);
        }

        makeupRepositories.PackBookings.Add(PackBooking.Create(session.Id, command.StudentId, now));
        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (UniqueConstraintViolationException)
        {
            return Result.Conflict<bool>(ConcurrentUpdateMessage, SessionErrorCodes.ConcurrentUpdate);
        }

        await teamNotifications.PackClassBookedAsync(command.StudentId, classGroup.Value, session, cancellationToken);
        return true;
    }
}

public sealed class CancelPackClassUseCase(
    IFamilyAccess familyAccess,
    IClassGroupRepository classGroupRepository,
    MakeupRepositories makeupRepositories,
    IStudentRepository studentRepository,
    IUnitOfWork unitOfWork,
    IBusinessCalendarService businessCalendar)
    : IUseCase<CancelPackClassCommand, bool>
{
    private const string ClassStartedMessage = "The class already started.";

    public async Task<Result<bool>> ExecuteAsync(CancelPackClassCommand command, CancellationToken cancellationToken)
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
        var booking = session is null ? null : await makeupRepositories.PackBookings.FindForUpdateAsync(session.Id, command.StudentId, cancellationToken);
        if (booking is null)
        {
            return true;
        }

        if (MakeupRules.HasStarted(classGroup.Value!, session, command.Date, await businessCalendar.LocalNowAsync(cancellationToken)))
        {
            return Result.Validation<bool>(ClassStartedMessage, SessionErrorCodes.ClassStarted, nameof(CancelPackClassCommand.Date));
        }

        makeupRepositories.PackBookings.Remove(booking);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
