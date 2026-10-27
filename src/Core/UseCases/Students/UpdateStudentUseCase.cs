using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Audit;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Clients;
using ClassManager.Core.Domain.Students;
using ClassManager.Core.Domain.Users;
using ClassManager.Core.UseCases.Clients;
using ClassManager.Core.UseCases.Users;

namespace ClassManager.Core.UseCases.Students;

public sealed record UpdateStudentRequest(string? FullName, DateOnly? BirthDate, string? Notes, string? Email)
{
    public UpdateStudentCommand ToCommand(long studentId) => new(studentId, FullName, BirthDate, Notes, Email);
}

public sealed record UpdateStudentCommand(
    long StudentId,
    [property: AuditPersonal] string? FullName,
    [property: AuditPersonal] DateOnly? BirthDate,
    [property: AuditPersonal] string? Notes,
    [property: AuditPersonal] string? Email) : ICommand;

public sealed class UpdateStudentUseCase(
    IBusinessRepository businessRepository,
    IClientRepository clientRepository,
    IStudentRepository studentRepository,
    IClientAccountRepository clientAccountRepository,
    IIdentityService identityService,
    IUserRepository userRepository,
    IPeopleLock peopleLock,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    IAccessScopes accessScopes)
    : IUseCase<UpdateStudentCommand, StudentResponse>
{
    private const string NotFoundMessage = "The student does not exist.";
    private const string AlreadyRegisteredMessage = "The client already has a student with this name.";
    private const string EmailUsedToSignInMessage = "The student signs in to the app with this email; only they can change it.";

    public async Task<Result<StudentResponse>> ExecuteAsync(UpdateStudentCommand command, CancellationToken cancellationToken)
    {
        var business = await businessRepository.GetCurrentAsync(cancellationToken);
        if (business is null)
        {
            return Result.Unauthorized<StudentResponse>(BusinessErrorCodes.CurrentBusinessNotFoundMessage, BusinessErrorCodes.CurrentBusinessNotFound);
        }

        await peopleLock.LockAsync(cancellationToken);
        var student = await studentRepository.FindForUpdateAsync(command.StudentId, cancellationToken);
        if (student is null || !await AccessRules.CanReachClientsAsync(accessScopes, clientRepository, [student.ClientId], cancellationToken))
        {
            return Result.NotFound<StudentResponse>(NotFoundMessage, StudentErrorCodes.NotFound);
        }

        var client = await clientRepository.GetForUpdateAsync(student.ClientId, cancellationToken);
        var today = business.TodayAt(timeProvider.GetUtcNow());
        var update = student.IsTheContactOf(client!)
            ? UpdateTheContact(student, command, today)
            : await UpdateAnotherPersonAsync(student, client!, command, today, cancellationToken);
        if (update.IsFailure)
        {
            return update.Error!;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return StudentResponse.NotInvited(student);
    }

    private static Result UpdateTheContact(Student student, UpdateStudentCommand command, DateOnly today)
    {
        if (AttendingContact.MinorError(command.BirthDate, today, nameof(UpdateStudentCommand.BirthDate)) is { } minorError)
        {
            return Result.Failure(minorError);
        }

        return student.UpdateDetails(command.BirthDate, command.Notes, today);
    }

    private async Task<Result> UpdateAnotherPersonAsync(
        Student student,
        Client client,
        UpdateStudentCommand command,
        DateOnly today,
        CancellationToken cancellationToken)
    {
        if (!student.User.HasEmail(command.Email) && await SignsInWithAnotherEmailAsync(student, command.Email, cancellationToken))
        {
            return Result.Failure(new ResultError(ClientErrorCodes.EmailUsedToSignIn, EmailUsedToSignInMessage, ErrorKind.Conflict)
            {
                FieldName = nameof(UpdateStudentCommand.Email),
            });
        }

        var update = student.Update(command.FullName, command.BirthDate, command.Notes, command.Email, today);
        if (update.IsFailure)
        {
            return update;
        }

        var namesake = await studentRepository.FindByClientAndNameAsync(student.ClientId, student.User.FullName, cancellationToken);
        if ((namesake is not null && namesake.Id != student.Id) || AttendingContact.NamesTheContact(client, student.User.FullName))
        {
            return Result.Failure(new ResultError(StudentErrorCodes.AlreadyRegistered, AlreadyRegisteredMessage, ErrorKind.Conflict)
            {
                FieldName = nameof(UpdateStudentCommand.FullName),
            });
        }

        var siblings = await studentRepository.ListByClientAsync(student.ClientId, cancellationToken);
        if (FamilyEmails.IsTakenByAnotherPerson(student.User, [client.User, .. siblings.Where(sibling => sibling.Id != student.Id).Select(sibling => sibling.User)]))
        {
            return Result.Failure(FamilyEmails.EmailOfAnotherPerson(nameof(User.Email)));
        }

        return await PeopleEmails.TakenErrorAsync(userRepository, [(student.User, nameof(UpdateStudentCommand.Email))], cancellationToken) is { } emailTaken
            ? Result.Failure(emailTaken)
            : Result.Success();
    }

    private async Task<bool> SignsInWithAnotherEmailAsync(Student student, string? email, CancellationToken cancellationToken)
    {
        var accounts = await clientAccountRepository.ListByClientAsync(student.ClientId, cancellationToken);
        var accountIds = accounts
            .Where(account => account.StudentId == student.Id)
            .Select(account => account.User.AccountId)
            .OfType<Guid>()
            .ToList();
        var signInEmail = await SignInEmails.FirstAsync(identityService, accountIds, cancellationToken);
        return accountIds.Count > 0 && !string.Equals(email?.Trim(), signInEmail, StringComparison.OrdinalIgnoreCase);
    }
}
