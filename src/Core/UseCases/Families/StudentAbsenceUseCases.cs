using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.Domain.Sessions;
using ClassManager.Core.Domain.Students;
using ClassManager.Core.UseCases.Sessions;

namespace ClassManager.Core.UseCases.Families;

public sealed record NotifyAbsenceCommand(Guid StudentId, Guid ClassGroupId, DateOnly Date);

public sealed record WithdrawAbsenceCommand(Guid StudentId, Guid ClassGroupId, DateOnly Date);

internal sealed record AbsenceTarget(ClassGroup ClassGroup, ClassSession? Session);

internal static class AbsenceRules
{
    private const string StudentNotFoundMessage = "The student is not part of this family.";
    private const string StudentNotEnrolledMessage = "The student isn't enrolled in this class on that date.";
    private const string CancelledMessage = "The class is cancelled on that date.";
    private const string ClassStartedMessage = "The class already started.";

    public static async Task<Result<AbsenceTarget>> FindAsync(
        Guid studentId,
        Guid classGroupId,
        DateOnly date,
        IFamilyAccess familyAccess,
        IStudentRepository studentRepository,
        IClassGroupRepository classGroupRepository,
        IEnrollmentRepository enrollmentRepository,
        IClassSessionRepository sessionRepository,
        IBusinessCalendarService businessCalendar,
        CancellationToken cancellationToken)
    {
        var access = await familyAccess.GetAsync(cancellationToken);
        if (access is null)
        {
            return FamilyFailures.NoAccess();
        }

        var students = await studentRepository.ListByClientAsync(access.ClientId, cancellationToken);
        if (students.All(student => student.Id != studentId))
        {
            return Result.NotFound<AbsenceTarget>(StudentNotFoundMessage, StudentErrorCodes.NotFound);
        }

        var classGroup = await SessionRules.FindScheduledClassGroupAsync(classGroupRepository, classGroupId, date, cancellationToken);
        if (classGroup.IsFailure)
        {
            return classGroup.Error!;
        }

        var roster = await enrollmentRepository.ListRosterOnAsync(classGroupId, date, cancellationToken);
        if (roster.All(entry => entry.StudentId != studentId))
        {
            return Result.Validation<AbsenceTarget>(
                StudentNotEnrolledMessage, SessionErrorCodes.StudentNotEnrolled, nameof(NotifyAbsenceCommand.StudentId));
        }

        var session = await sessionRepository.FindForUpdateAsync(classGroupId, date, cancellationToken);
        if (session?.IsCancelled == true)
        {
            return Result.Conflict<AbsenceTarget>(CancelledMessage, SessionErrorCodes.Cancelled);
        }

        var startTime = session?.EffectiveStartTime(classGroup.Value!.StartTime) ?? classGroup.Value!.StartTime;
        if (date.ToDateTime(startTime) <= await businessCalendar.LocalNowAsync(cancellationToken))
        {
            return Result.Validation<AbsenceTarget>(ClassStartedMessage, SessionErrorCodes.ClassStarted, nameof(NotifyAbsenceCommand.Date));
        }

        return new AbsenceTarget(classGroup.Value!, session);
    }
}

public sealed class NotifyAbsenceUseCase(
    IFamilyAccess familyAccess,
    IStudentRepository studentRepository,
    IClassGroupRepository classGroupRepository,
    IEnrollmentRepository enrollmentRepository,
    IClassSessionRepository sessionRepository,
    IAbsenceNoticeRepository absenceNoticeRepository,
    IBusinessRepository businessRepository,
    IUnitOfWork unitOfWork,
    IBusinessCalendarService businessCalendar,
    TimeProvider timeProvider)
    : IUseCase<NotifyAbsenceCommand, bool>
{
    private const string ConcurrentUpdateMessage = "The class changed at the same time. Try again.";

    public async Task<Result<bool>> ExecuteAsync(NotifyAbsenceCommand command, CancellationToken cancellationToken)
    {
        var target = await AbsenceRules.FindAsync(
            command.StudentId, command.ClassGroupId, command.Date, familyAccess, studentRepository,
            classGroupRepository, enrollmentRepository, sessionRepository, businessCalendar, cancellationToken);
        if (target.IsFailure)
        {
            return target.Error!;
        }

        var now = timeProvider.GetUtcNow();
        var session = target.Value!.Session;
        if (session is null)
        {
            session = ClassSession.Create(command.ClassGroupId, command.Date, now);
            sessionRepository.Add(session);
        }
        else if (await absenceNoticeRepository.FindForUpdateAsync(session.Id, command.StudentId, cancellationToken) is not null)
        {
            return true;
        }

        var keepsStreak = (await businessRepository.GetCurrentAsync(cancellationToken))?.NoticedAbsencesKeepStreak ?? true;
        absenceNoticeRepository.Add(AbsenceNotice.Create(session.Id, command.StudentId, keepsStreak, now));
        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (UniqueConstraintViolationException)
        {
            return Result.Conflict<bool>(ConcurrentUpdateMessage, SessionErrorCodes.ConcurrentUpdate);
        }

        return true;
    }
}

public sealed class WithdrawAbsenceUseCase(
    IFamilyAccess familyAccess,
    IStudentRepository studentRepository,
    IClassGroupRepository classGroupRepository,
    IEnrollmentRepository enrollmentRepository,
    IClassSessionRepository sessionRepository,
    IAbsenceNoticeRepository absenceNoticeRepository,
    IUnitOfWork unitOfWork,
    IBusinessCalendarService businessCalendar)
    : IUseCase<WithdrawAbsenceCommand, bool>
{
    public async Task<Result<bool>> ExecuteAsync(WithdrawAbsenceCommand command, CancellationToken cancellationToken)
    {
        var target = await AbsenceRules.FindAsync(
            command.StudentId, command.ClassGroupId, command.Date, familyAccess, studentRepository,
            classGroupRepository, enrollmentRepository, sessionRepository, businessCalendar, cancellationToken);
        if (target.IsFailure)
        {
            return target.Error!;
        }

        var session = target.Value!.Session;
        var notice = session is null ? null : await absenceNoticeRepository.FindForUpdateAsync(session.Id, command.StudentId, cancellationToken);
        if (notice is not null)
        {
            absenceNoticeRepository.Remove(notice);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return true;
    }
}
