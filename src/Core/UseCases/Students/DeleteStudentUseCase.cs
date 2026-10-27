using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Students;

namespace ClassManager.Core.UseCases.Students;

public sealed record DeleteStudentCommand(long StudentId) : ICommand;

public sealed record DeleteStudentResponse(long StudentId, int EndedEnrollments);

public sealed class DeleteStudentUseCase(
    IBusinessRepository businessRepository,
    IClientRepository clientRepository,
    IStudentRepository studentRepository,
    IEnrollmentRepository enrollmentRepository,
    IClientAccountRepository clientAccountRepository,
    IClientInvitationRepository invitationRepository,
    IUserRepository userRepository,
    IPeopleLock peopleLock,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    IAccessScopes accessScopes)
    : IUseCase<DeleteStudentCommand, DeleteStudentResponse>
{
    private const string NotFoundMessage = "The student does not exist.";

    public async Task<Result<DeleteStudentResponse>> ExecuteAsync(DeleteStudentCommand command, CancellationToken cancellationToken)
    {
        var business = await businessRepository.GetCurrentAsync(cancellationToken);
        if (business is null)
        {
            return Result.Unauthorized<DeleteStudentResponse>(BusinessErrorCodes.CurrentBusinessNotFoundMessage, BusinessErrorCodes.CurrentBusinessNotFound);
        }

        await peopleLock.LockAsync(cancellationToken);
        var student = await studentRepository.FindForUpdateAsync(command.StudentId, cancellationToken);
        if (student is null || !await AccessRules.CanReachClientsAsync(accessScopes, clientRepository, [student.ClientId], cancellationToken))
        {
            return Result.NotFound<DeleteStudentResponse>(NotFoundMessage, StudentErrorCodes.NotFound);
        }

        var now = timeProvider.GetUtcNow();
        var endedEnrollments = await EndEnrollmentsAsync(student, business.TodayAt(now), cancellationToken);
        await RemoveAppAccessAsync(student, now, cancellationToken);

        var roles = await userRepository.GetRolesAsync(student.UserId, cancellationToken);
        student.Delete(now);
        if (!roles.IsClient && !roles.IsInstructor && !roles.IsTeamMember)
        {
            student.User.Delete(now);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new DeleteStudentResponse(student.Id, endedEnrollments);
    }

    private async Task<int> EndEnrollmentsAsync(Student student, DateOnly today, CancellationToken cancellationToken)
    {
        var enrollments = await enrollmentRepository.ListOpenByStudentForUpdateAsync(student.Id, cancellationToken);
        foreach (var enrollment in enrollments)
        {
            if (today < enrollment.StartDate)
            {
                enrollmentRepository.Remove(enrollment);
            }
            else
            {
                enrollment.End(today);
            }
        }

        return enrollments.Count;
    }

    private async Task RemoveAppAccessAsync(Student student, DateTimeOffset now, CancellationToken cancellationToken)
    {
        foreach (var account in await clientAccountRepository.ListForUpdateByClientAsync(student.ClientId, cancellationToken))
        {
            if (account.StudentId == student.Id)
            {
                clientAccountRepository.Remove(account);
            }
        }

        foreach (var invitation in await invitationRepository.ListPendingForUpdateByPersonAsync(student.ClientId, student.Id, now, cancellationToken))
        {
            invitation.Revoke(now);
        }
    }
}
