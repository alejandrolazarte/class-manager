using ClassManager.Core.Common;
using ClassManager.ImportExport.Tabular;

namespace ClassManager.Core.UseCases.ImportExport;

public sealed record GetImportTemplateQuery(string Module) : IQuery;

public sealed class GetImportTemplateUseCase(IEnumerable<IImportModule> modules, ITabularWriter writer)
    : IUseCase<GetImportTemplateQuery, ExportFile>
{
    private const string TemplateFileNameSuffix = "-template";

    public async Task<Result<ExportFile>> ExecuteAsync(GetImportTemplateQuery query, CancellationToken cancellationToken)
    {
        var module = ImportFlow.FindModule(modules, query.Module);
        if (module.IsFailure)
        {
            return module.Error!;
        }

        var columns = module.Value!.Columns;
        var exampleRow = columns.ToDictionary(column => column.Key, column => column.Example);

        return await ExportFiles.WriteAsync(writer, module.Value.Name + TemplateFileNameSuffix, columns, [exampleRow], cancellationToken);
    }
}
