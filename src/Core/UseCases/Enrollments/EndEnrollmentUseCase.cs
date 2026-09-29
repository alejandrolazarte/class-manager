using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Enrollments;

namespace ClassManager.Core.UseCases.Enrollments;

public sealed record EndEnrollmentRequest(DateOnly? EndDate)
{
    public EndEnrollmentCommand ToCommand(Guid enrollmentId) => new(enrollmentId, EndDate);
}

public sealed record EndEnrollmentCommand(Guid EnrollmentId, DateOnly? EndDate);

public sealed record EndEnrollmentResponse(Guid EnrollmentId, bool WasRemoved);

public sealed class EndEnrollmentUseCase(
    IEnrollmentRepository enrollmentRepository,
    IClassGroupRepository classGroupRepository,
    IUnitOfWork unitOfWork,
    IBusinessCalendarService businessCalendar,
    IAccessScopes accessScopes)
    : IUseCase<EndEnrollmentCommand, EndEnrollmentResponse>
{
    private const string NotFoundMessage = "The enrollment does not exist.";

    public async Task<Result<EndEnrollmentResponse>> ExecuteAsync(EndEnrollmentCommand command, CancellationToken cancellationToken)
    {
        var enrollment = await enrollmentRepository.GetForUpdateAsync(command.EnrollmentId, cancellationToken);
        if (enrollment is null)
        {
            return Result.NotFound<EndEnrollmentResponse>(NotFoundMessage, EnrollmentErrorCodes.NotFound);
        }

        var classGroup = await classGroupRepository.GetByIdAsync(enrollment.ClassGroupId, cancellationToken);
        var scope = await accessScopes.ForInstructorsAsync(Permissions.Enrollments.Manage, cancellationToken);
        if (classGroup is null || !scope.Includes(classGroup.InstructorId))
        {
            return AccessRules.NotYours();
        }

        var endDate = command.EndDate ?? await businessCalendar.TodayAsync(cancellationToken);
        var neverStarted = enrollment.EndDate is null && endDate < enrollment.StartDate;
        if (neverStarted)
        {
            enrollmentRepository.Remove(enrollment);
        }
        else
        {
            var end = enrollment.End(endDate);
            if (end.IsFailure)
            {
                return end.Error!;
            }
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new EndEnrollmentResponse(enrollment.Id, neverStarted);
    }
}
