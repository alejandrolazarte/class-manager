using ClassManager.Core.Abstractions.Notifications;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Sessions;
using ClassManager.Core.UseCases.ClassGroups;
using ClassManager.Core.UseCases.PrivateLessons;

namespace ClassManager.Core.UseCases.Sessions;

public sealed record AssignSubstituteRequest(Guid? InstructorId);

public sealed record AssignSubstituteCommand(Guid ClassGroupId, DateOnly Date, Guid? InstructorId) : ICommand;

public sealed class AssignSubstituteUseCase(
    IClassGroupRepository classGroupRepository,
    IInstructorRepository instructorRepository,
    IClassSessionRepository sessionRepository,
    IPrivateLessonRepository privateLessonRepository,
    IUnitOfWork unitOfWork,
    IStudentAppNotificationService studentAppNotifications,
    TimeProvider timeProvider)
    : IUseCase<AssignSubstituteCommand, SessionStatusResponse>
{
    private const string ConcurrentUpdateMessage = "The session changed at the same time. Try again.";

    public async Task<Result<SessionStatusResponse>> ExecuteAsync(AssignSubstituteCommand command, CancellationToken cancellationToken)
    {
        var classGroup = await SessionRules.FindScheduledClassGroupAsync(classGroupRepository, command.ClassGroupId, command.Date, cancellationToken);
        if (classGroup.IsFailure)
        {
            return classGroup.Error!;
        }

        var substitute = await ClassGroupRules.FindActiveInstructorAsync(instructorRepository, command.InstructorId, cancellationToken);
        if (substitute.IsFailure)
        {
            return substitute.Error!;
        }

        var session = await sessionRepository.FindForUpdateAsync(command.ClassGroupId, command.Date, cancellationToken);
        if (substitute.Value!.Id != classGroup.Value!.InstructorId)
        {
            var startTime = session?.EffectiveStartTime(classGroup.Value.StartTime) ?? classGroup.Value.StartTime;
            var conflict = await SessionRules.FindInstructorConflictAsync(
                new InstructorAgendaRepositories(classGroupRepository, sessionRepository, privateLessonRepository),
                substitute.Value.Id,
                classGroup.Value,
                command.Date,
                startTime,
                cancellationToken);
            if (conflict is not null)
            {
                return conflict;
            }
        }

        if (session is null)
        {
            session = ClassSession.Create(command.ClassGroupId, command.Date, timeProvider.GetUtcNow());
            sessionRepository.Add(session);
        }

        var assign = session.AssignSubstitute(substitute.Value.Id, classGroup.Value.InstructorId, timeProvider.GetUtcNow());
        if (assign.IsFailure)
        {
            return assign.Error!;
        }

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (UniqueConstraintViolationException)
        {
            return Result.Conflict<SessionStatusResponse>(ConcurrentUpdateMessage, SessionErrorCodes.ConcurrentUpdate);
        }

        await studentAppNotifications.ClassChangedAsync(session, cancellationToken);
        return new SessionStatusResponse(session.ClassGroupId, session.Date, session.IsCancelled, session.CancellationReason);
    }
}
