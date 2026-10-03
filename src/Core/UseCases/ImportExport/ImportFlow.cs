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
        PlanLimitCheck planLimitCheck,
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
        if (plan.IsFailure)
        {
            return plan.Error!;
        }

        var rows = parsed.Import.Rows
            .Where(row => row.HasErrors)
            .Select(row => ImportRowResult.Error(row.LineNumber, row.Errors))
            .Concat(plan.Value!.Rows)
            .OrderBy(row => row.Line)
            .ToList();

        var summary = ImportSummary.From(rows);
        var planLimit = await PlanLimitAsync(module.Value.CountedFeatureCode, summary.Valid, planLimitCheck, cancellationToken);
        var report = new ImportReport(module.Value.Name, parsed.Import.Mapping.Headers, summary, rows, planLimit);
        return (report, plan.Value);
    }

    private static async Task<ImportPlanLimit?> PlanLimitAsync(
        string? countedFeatureCode,
        int newRecordCount,
        PlanLimitCheck planLimitCheck,
        CancellationToken cancellationToken)
    {
        if (countedFeatureCode is null || newRecordCount == 0)
        {
            return null;
        }

        var features = await planLimitCheck.FeatureAccess.GetCurrentAsync(cancellationToken);
        if (!features.IsActive || features.LimitOf(countedFeatureCode) is not { } limit)
        {
            return null;
        }

        var used = await planLimitCheck.FeatureUsage.CountAsync(countedFeatureCode, cancellationToken);
        var remaining = Math.Max(0, limit - used);
        return newRecordCount > remaining ? new ImportPlanLimit(countedFeatureCode, remaining) : null;
    }

    public static ImportCellError DuplicateInFile(string key) =>
        new(key, ImportUseCaseErrorCodes.DuplicateInFile, DuplicateInFileMessage);
}
