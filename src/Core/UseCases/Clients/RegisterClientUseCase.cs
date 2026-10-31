using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Audit;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Clients;
using ClassManager.Core.Domain.Fees;
using ClassManager.Core.Domain.Students;
using ClassManager.Core.Domain.Users;
using ClassManager.Core.UseCases.Students;
using ClassManager.Core.UseCases.Users;

namespace ClassManager.Core.UseCases.Clients;

public sealed record RegisterClientCommand(
    [property: AuditPersonal] string? FullName,
    [property: AuditPersonal] string? PhoneNumber,
    [property: AuditPersonal] string? Email,
    [property: AuditPersonal] string? Notes,
    IReadOnlyList<NewStudent>? Students,
    long? SamePersonUserId = null) : ICommand;

public sealed class RegisterClientUseCase(
    IBusinessRepository businessRepository,
    IClientRepository clientRepository,
    IStudentRepository studentRepository,
    IUserRepository userRepository,
    IFeeScheduleRepository feeScheduleRepository,
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
        if (currentUser.UserId is { } userId)
        {
            newClient.RecordRegisteredBy(userId);
        }

        var students = CreateStudents(newClient, command.Students ?? [], business.TodayAt(now), now);
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

        if (await JoinSamePeopleAsync(newClient, students.Value!, command, cancellationToken) is { } samePersonError)
        {
            return samePersonError;
        }

        var defaultFeeChanges = await feeScheduleRepository.ListDefaultFeeChangesAsync(cancellationToken);
        var billingPlanChanges = ClientBillingPlanChange.ForNewClient(newClient, defaultFeeChanges, business.TodayAt(now), now);
        clientRepository.Add(newClient);
        foreach (var student in students.Value!)
        {
            studentRepository.Add(student);
        }

        foreach (var billingPlanChange in billingPlanChanges)
        {
            feeScheduleRepository.Add(billingPlanChange);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ClientDetailsResponse.Registered(newClient, students.Value!, billingPlanChanges, defaultFeeChanges, business.TodayAt(now));
    }

    private async Task<ResultError?> JoinSamePeopleAsync(
        Client client,
        IReadOnlyList<Student> students,
        RegisterClientCommand command,
        CancellationToken cancellationToken)
    {
        var contact = client.User;
        var contactSamePerson = await FindSamePersonAsync(contact, command.SamePersonUserId, nameof(RegisterClientCommand.Email), cancellationToken);
        if (contactSamePerson.IsFailure)
        {
            return contactSamePerson.Error;
        }

        if (contactSamePerson.Value is { } contactPerson)
        {
            contactPerson.CompleteWith(contact);
            client.ChangePerson(contactPerson);
        }

        for (var index = 0; index < students.Count; index++)
        {
            var student = students[index];
            if (ReferenceEquals(student.User, contact))
            {
                student.ChangePerson(client.User);
                continue;
            }

            var samePerson = await FindSamePersonAsync(
                student.User, command.Students![index].SamePersonUserId, StudentFieldName(index, nameof(User.Email)), cancellationToken);
            if (samePerson.IsFailure)
            {
                return samePerson.Error;
            }

            if (samePerson.Value is { } person)
            {
                person.CompleteWith(student.User);
                student.ChangePerson(person);
            }
        }

        return null;
    }

    private Task<Result<User?>> FindSamePersonAsync(User person, long? samePersonUserId, string fieldName, CancellationToken cancellationToken) =>
        SamePerson.FindAsync(
            userRepository,
            person,
            person.Email,
            samePersonUserId,
            roles => !roles.IsClient && !roles.IsStudent,
            fieldName,
            cancellationToken);

    private static Result<IReadOnlyList<Student>> CreateStudents(
        Client client,
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
            var student = AttendingContact.NamesTheContact(client, newStudent.FullName)
                ? Student.CreateAttendingContact(client, newStudent.BirthDate, newStudent.Notes, today, now)
                : Student.Create(client, newStudent.FullName, newStudent.BirthDate, newStudent.Notes, today, now, newStudent.Email);
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

    private static Result<ClientDetailsResponse> PhoneNumberTaken(long? existingClientId) =>
        Result.Conflict<ClientDetailsResponse>(
            PhoneNumberTakenMessage,
            ClientErrorCodes.PhoneNumberTaken,
            new Dictionary<string, object?> { [ClientErrorCodes.ExistingClientIdDetail] = existingClientId });
}
