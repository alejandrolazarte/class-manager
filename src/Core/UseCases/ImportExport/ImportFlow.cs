using ClassManager.Core.Common;
using ClassManager.ImportExport;
using ClassManager.ImportExport.Parsing;

namespace ClassManager.Core.UseCases.ImportExport;

internal static class ImportFlow
{
    private const string ModuleNotFoundMessage = "There is no import for this kind of data.";
    private const string DuplicateInFileMessage = "An earlier row of the file has the same value in this column.";

    public static Result<IImportModule> FindModule(IEnumerable<IImportModule> modules, string moduleName)
    {
        var module = modules.FirstOrDefault(candidate => string.Equals(candidate.Name, moduleName, StringComparison.OrdinalIgnoreCase));
        return module is null
            ? Result.NotFound<IImportModule>(ModuleNotFoundMessage, ImportUseCaseErrorCodes.ModuleNotFound)
            : Result.Success(module);
    }

    public static async Task<Result<(ImportReport Report, ImportPlan Plan)>> PlanAsync(
        IEnumerable<IImportModule> modules,
        IImportParser parser,
        string moduleName,
        Stream file,
        CancellationToken cancellationToken)
    {
        var module = FindModule(modules, moduleName);
        if (module.IsFailure)
        {
            return module.Error!;
        }

        var parsed = await parser.ParseAsync(file, module.Value!.Columns, ImportLimits.Default, cancellationToken);
        if (parsed.Error is not null)
        {
            return Result.Validation<(ImportReport, ImportPlan)>(parsed.Error.Message, parsed.Error.Code, ImportUseCaseErrorCodes.FileFieldName);
        }

        var rowsWithoutCellErrors = parsed.Import!.Rows.Where(row => !row.HasErrors).ToList();
        var plan = await module.Value.PlanAsync(rowsWithoutCellErrors, cancellationToken);

        var rows = parsed.Import.Rows
            .Where(row => row.HasErrors)
            .Select(row => ImportRowResult.Error(row.LineNumber, row.Errors))
            .Concat(plan.Rows)
            .OrderBy(row => row.Line)
            .ToList();

        var report = new ImportReport(module.Value.Name, parsed.Import.Mapping.Headers, ImportSummary.From(rows), rows);
        return (report, plan);
    }

    public static ImportCellError DuplicateInFile(string key) =>
        new(key, ImportUseCaseErrorCodes.DuplicateInFile, DuplicateInFileMessage);
}
