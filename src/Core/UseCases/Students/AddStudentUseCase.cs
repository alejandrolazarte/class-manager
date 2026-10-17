using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Clients;
using ClassManager.Core.Domain.Students;
using ClassManager.Core.Domain.Users;
using ClassManager.Core.UseCases.Clients;
using ClassManager.Core.UseCases.Users;

namespace ClassManager.Core.UseCases.Students;

public sealed record AddStudentCommand(
    Guid ClientId,
    string? FullName,
    DateOnly? BirthDate,
    string? Notes,
    string? Email = null) : ICommand;

public sealed class AddStudentUseCase(
    IBusinessRepository businessRepository,
    IClientRepository clientRepository,
    IStudentRepository studentRepository,
    IUserRepository userRepository,
    IPeopleLock peopleLock,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    IAccessScopes accessScopes)
    : IUseCase<AddStudentCommand, StudentResponse>
{
    private const string ClientNotFoundMessage = "The client does not exist.";
    private const string AlreadyRegisteredMessage = "The client already has a student with this name.";

    public async Task<Result<StudentResponse>> ExecuteAsync(AddStudentCommand command, CancellationToken cancellationToken)
    {
        var business = await businessRepository.GetCurrentAsync(cancellationToken);
        if (business is null)
        {
            return Result.Unauthorized<StudentResponse>(BusinessErrorCodes.CurrentBusinessNotFoundMessage, BusinessErrorCodes.CurrentBusinessNotFound);
        }

        var client = await clientRepository.GetForUpdateAsync(command.ClientId, cancellationToken);
        if (client is null || !await AccessRules.CanReachClientsAsync(accessScopes, clientRepository, [client.Id], cancellationToken))
        {
            return Result.NotFound<StudentResponse>(ClientNotFoundMessage, ClientErrorCodes.NotFound);
        }

        var now = timeProvider.GetUtcNow();
        var today = business.TodayAt(now);
        var student = AttendingContact.NamesTheContact(client, command.FullName)
            ? Student.CreateAttendingContact(client, command.BirthDate, command.Notes, today, now)
            : Student.Create(command.ClientId, command.FullName, command.BirthDate, command.Notes, today, now, command.Email);
        if (student.IsFailure)
        {
            return student.Error!;
        }

        await peopleLock.LockAsync(cancellationToken);
        var siblings = await studentRepository.ListByClientAsync(command.ClientId, cancellationToken);
        if (FamilyEmails.IsTakenByAnotherPerson(student.Value!.User, [client.User, .. siblings.Select(sibling => sibling.User)]))
        {
            return FamilyEmails.EmailOfAnotherPerson(nameof(User.Email));
        }

        var existingStudent = await studentRepository.FindByClientAndNameAsync(command.ClientId, student.Value.User.FullName, cancellationToken);
        if (existingStudent is not null)
        {
            return AlreadyRegistered(existingStudent.Id);
        }

        if (await PeopleEmails.TakenErrorAsync(userRepository, [(student.Value.User, nameof(AddStudentCommand.Email))], cancellationToken) is { } emailTaken)
        {
            return emailTaken;
        }

        studentRepository.Add(student.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return StudentResponse.NotInvited(student.Value);
    }

    private static Result<StudentResponse> AlreadyRegistered(Guid? existingStudentId) =>
        Result.Conflict<StudentResponse>(
            AlreadyRegisteredMessage,
            StudentErrorCodes.AlreadyRegistered,
            new Dictionary<string, object?> { [StudentErrorCodes.ExistingStudentIdDetail] = existingStudentId });
}
