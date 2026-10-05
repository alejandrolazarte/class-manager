using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Clients;
using ClassManager.Core.Domain.Students;
using ClassManager.Core.UseCases.Students;

namespace ClassManager.Core.UseCases.Clients;

public sealed record RegisterClientCommand(
    string? FullName,
    string? PhoneNumber,
    string? Email,
    string? Notes,
    IReadOnlyList<NewStudent>? Students) : ICommand;

public sealed class RegisterClientUseCase(
    IBusinessRepository businessRepository,
    IClientRepository clientRepository,
    IStudentRepository studentRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    ICurrentUser currentUser)
    : IUseCase<RegisterClientCommand, ClientDetailsResponse>
{
    public const int MaximumStudentsPerRegistration = 10;

    private const string PhoneNumberTakenMessage = "A client with this phone number is already registered.";
    private const string TooManyStudentsMessage = "A registration can include at most 10 students.";
    private const string DuplicateStudentNameMessage = "Another student in this registration has the same name.";

    public async Task<Result<ClientDetailsResponse>> ExecuteAsync(RegisterClientCommand command, CancellationToken cancellationToken)
    {
        var business = await businessRepository.GetCurrentAsync(cancellationToken);
        if (business is null)
        {
            return Result.Unauthorized<ClientDetailsResponse>(BusinessErrorCodes.CurrentBusinessNotFoundMessage, BusinessErrorCodes.CurrentBusinessNotFound);
        }

        var phoneNumber = PhoneNumber.Create(command.PhoneNumber, business.DefaultCountryCallingCode);
        if (phoneNumber.IsFailure)
        {
            return phoneNumber.Error! with { FieldName = nameof(RegisterClientCommand.PhoneNumber) };
        }

        var now = timeProvider.GetUtcNow();
        var client = Client.Create(command.FullName, phoneNumber.Value!, command.Email, command.Notes, now);
        if (client.IsFailure)
        {
            return client.Error!;
        }

        if (currentUser.UserId is { } userId)
        {
            client.Value!.RecordRegisteredBy(userId);
        }

        var students = CreateStudents(client.Value!.Id, command.Students ?? [], business.TodayAt(now), now);
        if (students.IsFailure)
        {
            return students.Error!;
        }

        var existingClient = await clientRepository.FindByPhoneNumberAsync(phoneNumber.Value!, cancellationToken);
        if (existingClient is not null)
        {
            return PhoneNumberTaken(existingClient.Id);
        }

        clientRepository.Add(client.Value);
        foreach (var student in students.Value!)
        {
            studentRepository.Add(student);
        }

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (UniqueConstraintViolationException)
        {
            var winnerClient = await clientRepository.FindByPhoneNumberAsync(phoneNumber.Value!, cancellationToken);
            return PhoneNumberTaken(winnerClient?.Id);
        }

        return ClientDetailsResponse.From(client.Value, students.Value!, [], business.TodayAt(timeProvider.GetUtcNow()), StudentAppAccessResponse.NotInvited);
    }

    private static Result<IReadOnlyList<Student>> CreateStudents(
        Guid clientId,
        IReadOnlyList<NewStudent> newStudents,
        DateOnly today,
        DateTimeOffset now)
    {
        if (newStudents.Count > MaximumStudentsPerRegistration)
        {
            return Result.Validation<IReadOnlyList<Student>>(
                TooManyStudentsMessage,
                StudentErrorCodes.TooMany,
                nameof(RegisterClientCommand.Students));
        }

        var students = new List<Student>();
        var fullNames = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);
        for (var index = 0; index < newStudents.Count; index++)
        {
            var newStudent = newStudents[index];
            var student = Student.Create(clientId, newStudent.FullName, newStudent.BirthDate, newStudent.Notes, today, now);
            if (student.IsFailure)
            {
                return student.Error! with { FieldName = StudentFieldName(index, student.Error.FieldName) };
            }

            if (!fullNames.Add(student.Value!.FullName))
            {
                return Result.Validation<IReadOnlyList<Student>>(
                    DuplicateStudentNameMessage,
                    StudentErrorCodes.DuplicateName,
                    StudentFieldName(index, nameof(Student.FullName)));
            }

            students.Add(student.Value);
        }

        return students;
    }

    private static string StudentFieldName(int index, string? fieldName) =>
        $"{nameof(RegisterClientCommand.Students)}[{index}].{fieldName}";

    private static Result<ClientDetailsResponse> PhoneNumberTaken(Guid? existingClientId) =>
        Result.Conflict<ClientDetailsResponse>(
            PhoneNumberTakenMessage,
            ClientErrorCodes.PhoneNumberTaken,
            new Dictionary<string, object?> { [ClientErrorCodes.ExistingClientIdDetail] = existingClientId });
}
