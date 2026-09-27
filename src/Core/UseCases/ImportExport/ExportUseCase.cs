using System.Globalization;
using ClassManager.Core.Common;
using ClassManager.ImportExport.Tabular;

namespace ClassManager.Core.UseCases.ImportExport;

public sealed record ExportQuery(string Module);

public sealed class ExportUseCase(IEnumerable<IImportModule> modules, ITabularWriter writer, TimeProvider timeProvider)
    : IUseCase<ExportQuery, ExportFile>
{
    private const string FileNameDateFormat = "yyyy-MM-dd";
    private const string FileNameSeparator = "-";

    public async Task<Result<ExportFile>> ExecuteAsync(ExportQuery query, CancellationToken cancellationToken)
    {
        var module = ImportFlow.FindModule(modules, query.Module);
        if (module.IsFailure)
        {
            return module.Error!;
        }

        var rows = await module.Value!.ExportRowsAsync(cancellationToken);
        if (rows.IsFailure)
        {
            return rows.Error!;
        }

        var date = timeProvider.GetUtcNow().ToString(FileNameDateFormat, CultureInfo.InvariantCulture);
        return await ExportFiles.WriteAsync(writer, module.Value.Name + FileNameSeparator + date, module.Value.Columns, rows.Value!, cancellationToken);
    }
}
