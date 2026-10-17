using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Clients;
using ClassManager.Core.Domain.Students;
using ClassManager.Core.Domain.Users;
using ClassManager.Core.UseCases.Students;
using ClassManager.Core.UseCases.Users;

namespace ClassManager.Core.UseCases.Clients;

public sealed record RegisterClientCommand(
    string? FullName,
    string? PhoneNumber,
    string? Email,
    string? Notes,
    IReadOnlyList<NewStudent>? Students,
    Guid? SamePersonUserId = null) : ICommand;

public sealed class RegisterClientUseCase(
    IBusinessRepository businessRepository,
    IClientRepository clientRepository,
    IStudentRepository studentRepository,
    IUserRepository userRepository,
    IPeopleLock peopleLock,
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

        var newClient = client.Value!;
        var students = CreateStudents(newClient, command, business.TodayAt(now), now);
        if (students.IsFailure)
        {
            return students.Error!;
        }

        await peopleLock.LockAsync(cancellationToken);
        var existingClient = await clientRepository.FindByPhoneNumberAsync(phoneNumber.Value!, cancellationToken);
        if (existingClient is not null)
        {
            return PhoneNumberTaken(existingClient.Id);
        }

        var samePerson = await SamePerson.FindAsync(
            userRepository,
            newClient.User,
            newClient.User.Email,
            command.SamePersonUserId,
            roles => !roles.IsClient && !roles.IsStudent,
            nameof(RegisterClientCommand.Email),
            cancellationToken);
        if (samePerson.IsFailure)
        {
            return samePerson.Error!;
        }

        if (samePerson.Value is { } contact)
        {
            newClient = Client.CreateFor(contact, phoneNumber.Value!, command.Notes, now).Value!;
            students = CreateStudents(newClient, command, business.TodayAt(now), now);
            if (students.IsFailure)
            {
                return students.Error!;
            }
        }

        if (currentUser.UserId is { } userId)
        {
            newClient.RecordRegisteredBy(userId);
        }

        var people = new List<(User Person, string FieldName)> { (newClient.User, nameof(RegisterClientCommand.Email)) };
        people.AddRange(students.Value!
            .Select((student, index) => (student.User, StudentFieldName(index, nameof(User.Email))))
            .Where(person => person.User != newClient.User));
        if (await PeopleEmails.TakenErrorAsync(userRepository, people, cancellationToken) is { } emailTaken)
        {
            return emailTaken;
        }

        clientRepository.Add(newClient);
        foreach (var student in students.Value!)
        {
            studentRepository.Add(student);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ClientDetailsResponse.Registered(newClient, students.Value!, business.TodayAt(timeProvider.GetUtcNow()));
    }

    private static Result<IReadOnlyList<Student>> CreateStudents(
        Client client,
        RegisterClientCommand command,
        DateOnly today,
        DateTimeOffset now)
    {
        var newStudents = command.Students ?? [];
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
            var student = AttendingContact.NamesTheContact(command.FullName, newStudent.FullName)
                ? Student.CreateAttendingContact(client, newStudent.BirthDate, newStudent.Notes, today, now)
                : Student.Create(client.Id, newStudent.FullName, newStudent.BirthDate, newStudent.Notes, today, now, newStudent.Email);
            if (student.IsFailure)
            {
                return student.Error! with { FieldName = StudentFieldName(index, student.Error.FieldName) };
            }

            if (!fullNames.Add(student.Value!.User.FullName))
            {
                return Result.Validation<IReadOnlyList<Student>>(
                    DuplicateStudentNameMessage,
                    StudentErrorCodes.DuplicateName,
                    StudentFieldName(index, nameof(User.FullName)));
            }

            var minorContact = student.Value.IsTheContactOf(client)
                ? AttendingContact.MinorError(student.Value.User.BirthDate, today, StudentFieldName(index, nameof(User.BirthDate)))
                : null;
            if (minorContact is not null)
            {
                return minorContact;
            }

            if (FamilyEmails.IsTakenByAnotherPerson(student.Value.User, [client.User, .. students.Select(other => other.User)]))
            {
                return FamilyEmails.EmailOfAnotherPerson(StudentFieldName(index, nameof(User.Email)));
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
