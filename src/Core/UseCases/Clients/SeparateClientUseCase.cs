using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Clients;
using ClassManager.Core.UseCases.Users;

namespace ClassManager.Core.UseCases.Clients;

public sealed record SeparateClientCommand(Guid ClientId, string? FullName, string? Email) : ICommand;

public sealed class SeparateClientUseCase(
    IClientRepository clientRepository,
    IStudentRepository studentRepository,
    IClientAccountRepository clientAccountRepository,
    IClientInvitationRepository invitationRepository,
    IUserRepository userRepository,
    IPeopleLock peopleLock,
    IAccessScopes accessScopes,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IUseCase<SeparateClientCommand, SeparatedPersonResponse>
{
    private const string NotFoundMessage = "The client does not exist.";

    public async Task<Result<SeparatedPersonResponse>> ExecuteAsync(SeparateClientCommand command, CancellationToken cancellationToken)
    {
        await peopleLock.LockAsync(cancellationToken);
        var client = await clientRepository.GetForUpdateAsync(command.ClientId, cancellationToken);
        if (client is null || !await AccessRules.CanReachClientsAsync(accessScopes, clientRepository, [client.Id], cancellationToken))
        {
            return Result.NotFound<SeparatedPersonResponse>(NotFoundMessage, ClientErrorCodes.NotFound);
        }

        var previousPerson = client.User;
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

        person.Value!.ChangePhoneNumber(previousPerson.PhoneNumber);
        client.ChangePerson(person.Value);
        var familyStudents = await studentRepository.ListByClientForUpdateAsync(client.Id, cancellationToken);
        var movedStudentIds = new HashSet<Guid?> { null };
        foreach (var student in familyStudents.Where(student => student.UserId == previousPerson.Id))
        {
            student.ChangePerson(person.Value);
            movedStudentIds.Add(student.Id);
        }

        foreach (var account in await clientAccountRepository.ListForUpdateByClientAsync(client.Id, cancellationToken))
        {
            if (account.UserId == previousPerson.Id && movedStudentIds.Contains(account.StudentId))
            {
                clientAccountRepository.Remove(account);
            }
        }

        foreach (var invitation in await invitationRepository.ListPendingForUpdateByPersonAsync(client.Id, null, now, cancellationToken))
        {
            invitation.Revoke(now);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new SeparatedPersonResponse(person.Value.Id, person.Value.FullName);
    }
}
