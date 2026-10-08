using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Instructors;
using ClassManager.ImportExport.Columns;
using ClassManager.ImportExport.Parsing;

namespace ClassManager.Core.UseCases.ImportExport.Instructors;

public sealed class InstructorImportModule(IInstructorRepository instructorRepository) : IImportModule
{
    public const string ModuleName = "instructors";
    public const string FullNameKey = "fullName";

    private const string FullNameHeader = "Profesor";
    private const string FullNameExample = "Laura Gómez";
    private const string NameTakenMessage = "An instructor with this name already exists.";

    private static readonly ImportColumn FullNameColumn = new(FullNameKey, FullNameHeader, ColumnType.Text, IsRequired: true)
    {
        Aliases = ["profesora", "profe", "nombre", "coach", "entrenador", "entrenadora", "monitor", "monitora", "instructor", "instructora"],
        Example = FullNameExample,
    };

    public string Name => ModuleName;

    public string? CountedFeatureCode => null;

    public IReadOnlyList<ImportColumn> Columns { get; } = [FullNameColumn];

    public async Task<Result<ImportPlan>> PlanAsync(IReadOnlyList<ImportRow> rows, CancellationToken cancellationToken)
    {
        var existingInstructors = await instructorRepository.ListAllAsync(cancellationToken);
        var existingNames = existingInstructors.Select(instructor => instructor.FullName).ToHashSet(StringComparer.CurrentCultureIgnoreCase);
        var namesInFile = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);
        var newInstructors = new List<Instructor>();
        var results = new List<ImportRowResult>();

        foreach (var row in rows)
        {
            var instructor = Instructor.Create(row.GetText(FullNameKey));
            if (instructor.IsFailure)
            {
                results.Add(ImportRowResult.Error(row.LineNumber, new ImportCellError(FullNameKey, instructor.Error!.Code, instructor.Error.Message)));
            }
            else if (existingNames.Contains(instructor.Value!.FullName))
            {
                results.Add(ImportRowResult.Skipped(row.LineNumber, new ImportCellError(FullNameKey, InstructorErrorCodes.NameTaken, NameTakenMessage)));
            }
            else if (!namesInFile.Add(instructor.Value.FullName))
            {
                results.Add(ImportRowResult.Error(row.LineNumber, ImportFlow.DuplicateInFile(FullNameKey)));
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
            .Select(instructor => (IReadOnlyDictionary<string, string?>)new Dictionary<string, string?> { [FullNameKey] = instructor.FullName })
            .ToList();
    }
}
