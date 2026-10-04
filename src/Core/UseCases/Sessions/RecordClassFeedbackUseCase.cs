using ClassManager.Core.Abstractions.Notifications;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.UseCases.Sessions;

public sealed record RecordClassFeedbackRequest(string? Text);

public sealed record RecordClassFeedbackCommand(Guid ClassGroupId, DateOnly Date, Guid StudentId, string? Text);

public sealed class RecordClassFeedbackUseCase(
    IClassGroupRepository classGroupRepository,
    IEnrollmentRepository enrollmentRepository,
    IMakeupBookingRepository makeupBookingRepository,
    IPackBookingRepository packBookingRepository,
    IClassSessionRepository sessionRepository,
    IClassFeedbackRepository feedbackRepository,
    IUnitOfWork unitOfWork,
    IBusinessCalendarService businessCalendar,
    IStudentAppNotificationService studentAppNotifications,
    TimeProvider timeProvider,
    IAccessScopes accessScopes)
    : IUseCase<RecordClassFeedbackCommand, bool>
{
    private const string InFutureMessage = "A comment can't be left before the class date.";
    private const string StudentNotEnrolledMessage = "The student isn't enrolled in this class on that date.";
    private const string CancelledMessage = "The class is cancelled on that date.";
    private const string ConcurrentUpdateMessage = "The session changed at the same time. Try again.";

    public async Task<Result<bool>> ExecuteAsync(RecordClassFeedbackCommand command, CancellationToken cancellationToken)
    {
        var classGroup = await SessionRules.FindScheduledClassGroupAsync(classGroupRepository, command.ClassGroupId, command.Date, cancellationToken);
        if (classGroup.IsFailure)
        {
            return classGroup.Error!;
        }

        var session = await sessionRepository.FindForUpdateAsync(command.ClassGroupId, command.Date, cancellationToken);
        var scope = await accessScopes.ForInstructorsAsync(Permissions.Attendance.RecordAll, cancellationToken);
        if (!SessionRules.IsInScope(scope, classGroup.Value!, session))
        {
            return AccessRules.NotYours();
        }

        if (command.Date > await businessCalendar.TodayAsync(cancellationToken))
        {
            return Result.Validation<bool>(InFutureMessage, SessionErrorCodes.InFuture, nameof(RecordClassFeedbackCommand.Date));
        }

        if (!await SessionRules.IsInClassAsync(
            enrollmentRepository, makeupBookingRepository, packBookingRepository, command.ClassGroupId, command.Date, session, command.StudentId, cancellationToken))
        {
            return Result.Validation<bool>(
                StudentNotEnrolledMessage, SessionErrorCodes.StudentNotEnrolled, nameof(RecordClassFeedbackCommand.StudentId));
        }

        if (session?.IsCancelled == true)
        {
            return Result.Conflict<bool>(CancelledMessage, SessionErrorCodes.Cancelled);
        }

        var now = timeProvider.GetUtcNow();
        if (session is null)
        {
            session = ClassSession.Create(command.ClassGroupId, command.Date, now);
            sessionRepository.Add(session);
        }

        var instructorId = session.EffectiveInstructorId(classGroup.Value!.InstructorId);
        var feedback = await feedbackRepository.FindForUpdateAsync(session.Id, command.StudentId, cancellationToken);
        ClassFeedback? createdFeedback = null;
        if (string.IsNullOrWhiteSpace(command.Text))
        {
            if (feedback is not null)
            {
                feedbackRepository.Remove(feedback);
            }
        }
        else if (feedback is null)
        {
            var created = ClassFeedback.Create(session.Id, command.StudentId, instructorId, command.Text, now);
            if (created.IsFailure)
            {
                return created.Error!;
            }

            createdFeedback = created.Value!;
            feedbackRepository.Add(createdFeedback);
        }
        else
        {
            var rewrite = feedback.Rewrite(command.Text, instructorId, now);
            if (rewrite.IsFailure)
            {
                return rewrite.Error!;
            }
        }

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (UniqueConstraintViolationException)
        {
            return Result.Conflict<bool>(ConcurrentUpdateMessage, SessionErrorCodes.ConcurrentUpdate);
        }

        if (createdFeedback is not null)
        {
            await studentAppNotifications.FeedbackLeftAsync(createdFeedback, cancellationToken);
        }

        return true;
    }
}
