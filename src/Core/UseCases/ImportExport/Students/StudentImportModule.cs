using System.Globalization;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Clients;
using ClassManager.Core.Domain.Students;
using ClassManager.ImportExport.Columns;
using ClassManager.ImportExport.Parsing;

namespace ClassManager.Core.UseCases.ImportExport.Students;

public sealed class StudentImportModule(
    IBusinessRepository businessRepository,
    IClientRepository clientRepository,
    IStudentRepository studentRepository,
    TimeProvider timeProvider)
    : IImportModule
{
    public const string ModuleName = "students";
    public const string StudentNameKey = "studentName";
    public const string PhoneKey = "phone";
    public const string EmailKey = "email";
    public const string BirthDateKey = "birthDate";
    public const string StudentNotesKey = "studentNotes";
    public const string ContactNameKey = "contactName";
    public const string ContactNotesKey = "contactNotes";

    private const string ExportDateFormat = "dd/MM/yyyy";
    private const string InternationalPrefix = "+";
    private const string CountryCodeSeparator = " ";
    private const string AlreadyRegisteredMessage = "The family already has a student with this name.";
    private const string ContactMismatchMessage = "This phone belongs to a family with another Responsable. Check the phone or the Responsable.";

    public string Name => ModuleName;

    public IReadOnlyList<ImportColumn> Columns { get; } =
    [
        new(StudentNameKey, "Alumno", ColumnType.Text, IsRequired: true)
        {
            Aliases = ["alumna", "nombre", "nombre alumno", "nombre y apellido", "student"],
            Example = "Lucas Gómez",
        },
        new(PhoneKey, "Teléfono", ColumnType.Phone, IsRequired: true)
        {
            Aliases = ["tel", "móvil", "celular", "whatsapp", "phone"],
            Example = "611 222 333",
        },
        new(EmailKey, "Email", ColumnType.Email)
        {
            Aliases = ["correo", "mail", "correo electrónico"],
            Example = "maria@example.com",
        },
        new(BirthDateKey, "Fecha de nacimiento", ColumnType.Date)
        {
            Aliases = ["nacimiento", "fecha nac", "cumpleaños", "birth date"],
            Example = "07/03/2015",
        },
        new(StudentNotesKey, "Notas alumno", ColumnType.Text)
        {
            Aliases = ["notas", "observaciones"],
        },
        new(ContactNameKey, "Responsable", ColumnType.Text)
        {
            Aliases = ["tutor", "tutora", "padre", "madre", "cliente", "contacto", "contact"],
            Example = "María Gómez",
        },
        new(ContactNotesKey, "Notas responsable", ColumnType.Text)
        {
            Aliases = ["notas contacto", "notas tutor"],
        },
    ];

    public async Task<Result<ImportPlan>> PlanAsync(IReadOnlyList<ImportRow> rows, CancellationToken cancellationToken)
    {
        var business = await businessRepository.GetCurrentAsync(cancellationToken);
        if (business is null)
        {
            return Result.Unauthorized<ImportPlan>(BusinessErrorCodes.CurrentBusinessNotFoundMessage, BusinessErrorCodes.CurrentBusinessNotFound);
        }

        var now = timeProvider.GetUtcNow();
        var families = await LoadExistingFamiliesAsync(rows, business.DefaultCountryCallingCode, cancellationToken);
        var context = new StudentImportContext(business.DefaultCountryCallingCode, business.TodayAt(now), now, families);
        var results = rows.Select(context.Plan).ToList();

        return new ImportPlan(results, () => AddValidRows(context));
    }

    public async Task<Result<IReadOnlyList<IReadOnlyDictionary<string, string?>>>> ExportRowsAsync(CancellationToken cancellationToken)
    {
        var business = await businessRepository.GetCurrentAsync(cancellationToken);
        if (business is null)
        {
            return Result.Unauthorized<IReadOnlyList<IReadOnlyDictionary<string, string?>>>(
                BusinessErrorCodes.CurrentBusinessNotFoundMessage,
                BusinessErrorCodes.CurrentBusinessNotFound);
        }

        var clientsById = (await clientRepository.ListAllAsync(cancellationToken)).ToDictionary(client => client.Id);
        var students = await studentRepository.ListAllAsync(cancellationToken);

        return students
            .Where(student => clientsById.ContainsKey(student.ClientId))
            .Select(student => (IReadOnlyDictionary<string, string?>)ToExportRow(student, clientsById[student.ClientId], business.DefaultCountryCallingCode))
            .ToList();
    }

    private static Dictionary<string, string?> ToExportRow(Student student, Client client, string defaultCountryCallingCode) =>
        new Dictionary<string, string?>
        {
            [StudentNameKey] = student.FullName,
            [PhoneKey] = ToExportPhoneNumber(client.PhoneNumber, defaultCountryCallingCode),
            [EmailKey] = client.Email,
            [BirthDateKey] = student.BirthDate?.ToString(ExportDateFormat, CultureInfo.InvariantCulture),
            [StudentNotesKey] = student.Notes,
            [ContactNameKey] = StringComparer.CurrentCultureIgnoreCase.Equals(client.FullName, student.FullName) ? null : client.FullName,
            [ContactNotesKey] = client.Notes,
        };

    private static string ToExportPhoneNumber(PhoneNumber phoneNumber, string defaultCountryCallingCode)
    {
        var localPrefix = InternationalPrefix + defaultCountryCallingCode;
        return phoneNumber.Value.StartsWith(localPrefix, StringComparison.Ordinal)
            ? localPrefix + CountryCodeSeparator + phoneNumber.Value[localPrefix.Length..]
            : phoneNumber.Value;
    }

    private async Task<Dictionary<PhoneNumber, StudentImportFamily>> LoadExistingFamiliesAsync(
        IReadOnlyList<ImportRow> rows,
        string defaultCountryCallingCode,
        CancellationToken cancellationToken)
    {
        var phoneNumbers = rows
            .Select(row => PhoneNumber.Create(row.GetText(PhoneKey), defaultCountryCallingCode))
            .Where(phoneNumber => phoneNumber.IsSuccess)
            .Select(phoneNumber => phoneNumber.Value!)
            .Distinct()
            .ToList();

        var clients = await clientRepository.ListByPhoneNumbersAsync(phoneNumbers, cancellationToken);
        var students = await studentRepository.ListByClientIdsAsync(clients.Select(client => client.Id).ToList(), cancellationToken);

        return clients.ToDictionary(
            client => client.PhoneNumber,
            client => new StudentImportFamily(
                client,
                isNew: false,
                students.Where(student => student.ClientId == client.Id).Select(student => student.FullName)));
    }

    private void AddValidRows(StudentImportContext context)
    {
        foreach (var family in context.NewFamilies)
        {
            clientRepository.Add(family.Client);
        }

        foreach (var student in context.NewStudents)
        {
            studentRepository.Add(student);
        }
    }

    private sealed class StudentImportContext(
        string defaultCountryCallingCode,
        DateOnly today,
        DateTimeOffset now,
        Dictionary<PhoneNumber, StudentImportFamily> familiesByPhoneNumber)
    {
        public List<StudentImportFamily> NewFamilies { get; } = [];

        public List<Student> NewStudents { get; } = [];

        public ImportRowResult Plan(ImportRow row)
        {
            var phoneNumber = PhoneNumber.Create(row.GetText(PhoneKey), defaultCountryCallingCode);
            if (phoneNumber.IsFailure)
            {
                return Error(row, PhoneKey, phoneNumber.Error!);
            }

            var explicitContactName = row.GetText(ContactNameKey);
            var family = familiesByPhoneNumber.GetValueOrDefault(phoneNumber.Value!);
            if (family is not null && explicitContactName is not null && !family.HasContactName(explicitContactName))
            {
                return ImportRowResult.Error(row.LineNumber, new ImportCellError(ContactNameKey, ImportUseCaseErrorCodes.ContactMismatch, ContactMismatchMessage));
            }

            if (family is null)
            {
                var client = Client.Create(
                    explicitContactName ?? row.GetText(StudentNameKey),
                    phoneNumber.Value!,
                    row.GetText(EmailKey),
                    row.GetText(ContactNotesKey),
                    now);
                if (client.IsFailure)
                {
                    return Error(row, ClientFieldKey(client.Error!.FieldName, explicitContactName), client.Error);
                }

                family = new StudentImportFamily(client.Value!, isNew: true, []);
            }

            var student = Student.Create(
                family.Client.Id,
                row.GetText(StudentNameKey),
                row.GetDate(BirthDateKey),
                row.GetText(StudentNotesKey),
                today,
                now);
            if (student.IsFailure)
            {
                return Error(row, StudentFieldKey(student.Error!.FieldName), student.Error);
            }

            if (family.HasPlannedStudent(student.Value!.FullName))
            {
                return ImportRowResult.Error(row.LineNumber, ImportFlow.DuplicateInFile(StudentNameKey));
            }

            if (family.HasExistingStudent(student.Value.FullName))
            {
                return ImportRowResult.Skipped(
                    row.LineNumber,
                    new ImportCellError(StudentNameKey, StudentErrorCodes.AlreadyRegistered, AlreadyRegisteredMessage));
            }

            if (family.IsNew && familiesByPhoneNumber.TryAdd(phoneNumber.Value!, family))
            {
                NewFamilies.Add(family);
            }

            family.PlanStudent(student.Value.FullName);
            NewStudents.Add(student.Value);
            return ImportRowResult.Valid(row.LineNumber);
        }

        private static ImportRowResult Error(ImportRow row, string key, ResultError error) =>
            ImportRowResult.Error(row.LineNumber, new ImportCellError(key, error.Code, error.Message));

        private static string ClientFieldKey(string? fieldName, string? explicitContactName) => fieldName switch
        {
            nameof(Client.Email) => EmailKey,
            nameof(Client.Notes) => ContactNotesKey,
            _ => explicitContactName is null ? StudentNameKey : ContactNameKey,
        };

        private static string StudentFieldKey(string? fieldName) => fieldName switch
        {
            nameof(Student.BirthDate) => BirthDateKey,
            nameof(Student.Notes) => StudentNotesKey,
            _ => StudentNameKey,
        };
    }

    private sealed class StudentImportFamily(Client client, bool isNew, IEnumerable<string> existingStudentNames)
    {
        private readonly HashSet<string> _existingStudentNames = new(existingStudentNames, StringComparer.CurrentCultureIgnoreCase);
        private readonly HashSet<string> _plannedStudentNames = new(StringComparer.CurrentCultureIgnoreCase);

        public Client Client => client;

        public bool IsNew => isNew;

        public bool HasContactName(string contactName) =>
            StringComparer.CurrentCultureIgnoreCase.Equals(client.FullName, contactName.Trim());

        public bool HasExistingStudent(string fullName) => _existingStudentNames.Contains(fullName);

        public bool HasPlannedStudent(string fullName) => _plannedStudentNames.Contains(fullName);

        public void PlanStudent(string fullName) => _plannedStudentNames.Add(fullName);
    }
}
