using ClassManager.Core.Common;
using ClassManager.ImportExport.Columns;
using ClassManager.ImportExport.Parsing;

namespace ClassManager.Core.UseCases.ImportExport;

public interface IImportModule
{
    string Name { get; }

    IReadOnlyList<ImportColumn> Columns { get; }

    Task<Result<ImportPlan>> PlanAsync(IReadOnlyList<ImportRow> rows, CancellationToken cancellationToken);
}
