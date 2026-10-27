using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Instructors;
using ClassManager.Core.Domain.Users;
using ClassManager.ImportExport.Columns;
using ClassManager.ImportExport.Parsing;

namespace ClassManager.Core.UseCases.ImportExport.Instructors;

public sealed class InstructorImportModule(
    IInstructorRepository instructorRepository,
    IUserRepository userRepository,
    TimeProvider timeProvider)
    : IImportModule
{
    public const string ModuleName = "instructors";
    public const string FullNameKey = "fullName";
    public const string EmailKey = "email";

    private const string FullNameHeader = "Profesor";
    private const string FullNameExample = "Laura Gómez";
    private const string EmailHeader = "Email";
    private const string EmailExample = "laura@example.com";
    private const string NameTakenMessage = "An instructor with this name already exists.";
    private const string EmailTakenMessage = "Another person of the business already uses this email.";

    private static readonly ImportColumn FullNameColumn = new(FullNameKey, FullNameHeader, ColumnType.Text, IsRequired: true)
    {
        Aliases = ["profesora", "profe", "nombre", "coach", "entrenador", "entrenadora", "monitor", "monitora", "instructor", "instructora"],
        Example = FullNameExample,
    };

    private static readonly ImportColumn EmailColumn = new(EmailKey, EmailHeader, ColumnType.Email)
    {
        Aliases = ["mail", "correo", "correo electrónico", "e-mail"],
        Example = EmailExample,
    };

    public string Name => ModuleName;

    public string? CountedFeatureCode => null;

    public string RowNameKey => FullNameKey;

    public IReadOnlyList<ImportColumn> Columns { get; } = [FullNameColumn, EmailColumn];

    public async Task<Result<ImportPlan>> PlanAsync(IReadOnlyList<ImportRow> rows, CancellationToken cancellationToken)
    {
        var existingInstructors = await instructorRepository.ListAllAsync(cancellationToken);
        var existingNames = existingInstructors.Select(instructor => instructor.User.FullName).ToHashSet(StringComparer.CurrentCultureIgnoreCase);
        var takenEmails = await userRepository.ListEmailsAsync(cancellationToken);
        var namesInFile = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);
        var emailsInFile = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var newInstructors = new List<Instructor>();
        var results = new List<ImportRowResult>();

        var now = timeProvider.GetUtcNow();
        foreach (var row in rows)
        {
            var instructor = Instructor.Create(row.GetText(FullNameKey), now, row.GetText(EmailKey));
            if (instructor.IsFailure)
            {
                results.Add(ImportRowResult.Error(row.LineNumber, new ImportCellError(FieldKey(instructor.Error!.FieldName), instructor.Error.Code, instructor.Error.Message)));
            }
            else if (existingNames.Contains(instructor.Value!.User.FullName))
            {
                results.Add(ImportRowResult.Skipped(row.LineNumber, new ImportCellError(FullNameKey, InstructorErrorCodes.NameTaken, NameTakenMessage)));
            }
            else if (!namesInFile.Add(instructor.Value.User.FullName))
            {
                results.Add(ImportRowResult.Error(row.LineNumber, ImportFlow.DuplicateInFile(FullNameKey)));
            }
            else if (instructor.Value.User.Email is { } email && (takenEmails.Contains(email) || !emailsInFile.Add(email)))
            {
                results.Add(ImportRowResult.Error(row.LineNumber, new ImportCellError(EmailKey, UserErrorCodes.EmailTaken, EmailTakenMessage)));
            }
            else
            {
                newInstructors.Add(instructor.Value);
                results.Add(ImportRowResult.Valid(row.LineNumber));
            }
        }

        return new ImportPlan(results, () => newInstructors.ForEach(instructorRepository.Add));
    }

    public async Task<Result<IReadOnlyList<IReadOnlyDictionary<string, string?>>>> ExportRowsAsync(CancellationToken cancellationToken)
    {
        var instructors = await instructorRepository.ListAllAsync(cancellationToken);
        return instructors
            .Select(instructor => (IReadOnlyDictionary<string, string?>)new Dictionary<string, string?>
            {
                [FullNameKey] = instructor.User.FullName,
                [EmailKey] = instructor.User.Email,
            })
            .ToList();
    }

    private static string FieldKey(string? fieldName) =>
        string.Equals(fieldName, nameof(User.Email), StringComparison.Ordinal) ? EmailKey : FullNameKey;
}
