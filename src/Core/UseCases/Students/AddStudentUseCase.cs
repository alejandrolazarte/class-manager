using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Clients;
using ClassManager.Core.Domain.Students;

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

        var client = await clientRepository.GetByIdAsync(command.ClientId, cancellationToken);
        if (client is null || !await AccessRules.CanReachClientsAsync(accessScopes, clientRepository, [client.Id], cancellationToken))
        {
            return Result.NotFound<StudentResponse>(ClientNotFoundMessage, ClientErrorCodes.NotFound);
        }

        var now = timeProvider.GetUtcNow();
        var student = Student.Create(command.ClientId, command.FullName, command.BirthDate, command.Notes, business.TodayAt(now), now, command.Email);
        if (student.IsFailure)
        {
            return student.Error!;
        }

        var siblings = await studentRepository.ListByClientAsync(command.ClientId, cancellationToken);
        if (FamilyEmails.IsTakenByAnotherPerson(student.Value!.Email, client.Email, siblings))
        {
            return FamilyEmails.EmailOfAnotherPerson(nameof(Student.Email));
        }

        var existingStudent = await studentRepository.FindByClientAndNameAsync(command.ClientId, student.Value!.FullName, cancellationToken);
        if (existingStudent is not null)
        {
            return AlreadyRegistered(existingStudent.Id);
        }

        studentRepository.Add(student.Value);
        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (UniqueConstraintViolationException)
        {
            var winnerStudent = await studentRepository.FindByClientAndNameAsync(command.ClientId, student.Value.FullName, cancellationToken);
            return AlreadyRegistered(winnerStudent?.Id);
        }

        return StudentResponse.From(student.Value);
    }

    private static Result<StudentResponse> AlreadyRegistered(Guid? existingStudentId) =>
        Result.Conflict<StudentResponse>(
            AlreadyRegisteredMessage,
            StudentErrorCodes.AlreadyRegistered,
            new Dictionary<string, object?> { [StudentErrorCodes.ExistingStudentIdDetail] = existingStudentId });
}
