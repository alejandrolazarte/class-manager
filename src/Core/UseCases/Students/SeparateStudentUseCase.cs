using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Audit;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Students;
using ClassManager.Core.UseCases.Users;

namespace ClassManager.Core.UseCases.Students;

public sealed record SeparateStudentCommand(long StudentId, [property: AuditPersonal] string? FullName, [property: AuditPersonal] string? Email) : ICommand;

public sealed class SeparateStudentUseCase(
    IClientRepository clientRepository,
    IStudentRepository studentRepository,
    IClientAccountRepository clientAccountRepository,
    IClientInvitationRepository invitationRepository,
    IUserRepository userRepository,
    IPeopleLock peopleLock,
    IAccessScopes accessScopes,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IUseCase<SeparateStudentCommand, SeparatedPersonResponse>
{
    private const string NotFoundMessage = "The student does not exist.";
    private const string IsTheContactMessage = "The student is the contact of the client; separate the client instead.";
    private const string AlreadyRegisteredMessage = "The client already has a student with this name.";

    public async Task<Result<SeparatedPersonResponse>> ExecuteAsync(SeparateStudentCommand command, CancellationToken cancellationToken)
    {
        await peopleLock.LockAsync(cancellationToken);
        var student = await studentRepository.FindForUpdateAsync(command.StudentId, cancellationToken);
        if (student is null || !await AccessRules.CanReachClientsAsync(accessScopes, clientRepository, [student.ClientId], cancellationToken))
        {
            return Result.NotFound<SeparatedPersonResponse>(NotFoundMessage, StudentErrorCodes.NotFound);
        }

        var client = await clientRepository.GetForUpdateAsync(student.ClientId, cancellationToken);
        if (client!.UserId == student.UserId)
        {
            return Result.Conflict<SeparatedPersonResponse>(IsTheContactMessage, StudentErrorCodes.IsTheContact);
        }

        var previousPerson = student.User;
        var roles = await userRepository.GetRolesAsync(previousPerson.Id, cancellationToken);
        if (!roles.IsInstructor && !roles.IsTeamMember)
        {
            return SeparatedPerson.NothingToSeparate();
        }

        var now = timeProvider.GetUtcNow();
        var person = await SeparatedPerson.CreateAsync(userRepository, command.FullName, command.Email, now, cancellationToken);
        if (person.IsFailure)
        {
            return person.Error!;
        }

        var namesake = await studentRepository.FindByClientAndNameAsync(student.ClientId, person.Value!.FullName, cancellationToken);
        if (namesake is not null && namesake.Id != student.Id)
        {
            return Result.Conflict<SeparatedPersonResponse>(AlreadyRegisteredMessage, StudentErrorCodes.AlreadyRegistered);
        }

        student.ChangePerson(person.Value);
        foreach (var account in await clientAccountRepository.ListForUpdateByClientAsync(student.ClientId, cancellationToken))
        {
            if (account.UserId == previousPerson.Id && account.StudentId == student.Id)
            {
                clientAccountRepository.Remove(account);
            }
        }

        foreach (var invitation in await invitationRepository.ListPendingForUpdateByPersonAsync(student.ClientId, student.Id, now, cancellationToken))
        {
            invitation.Revoke(now);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new SeparatedPersonResponse(person.Value.Id, person.Value.FullName);
    }
}
