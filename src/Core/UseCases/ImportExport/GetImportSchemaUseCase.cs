using ClassManager.Core.Common;
using ClassManager.ImportExport.Columns;

namespace ClassManager.Core.UseCases.ImportExport;

public sealed record GetImportSchemaQuery(string Module) : IQuery;

public sealed record ImportSchemaResponse(string Module, IReadOnlyList<ImportColumn> Columns);

public sealed class GetImportSchemaUseCase(IEnumerable<IImportModule> modules)
    : IUseCase<GetImportSchemaQuery, ImportSchemaResponse>
{
    public Task<Result<ImportSchemaResponse>> ExecuteAsync(GetImportSchemaQuery query, CancellationToken cancellationToken)
    {
        var module = ImportFlow.FindModule(modules, query.Module);
        var result = module.IsFailure
            ? module.Error!
            : Result.Success(new ImportSchemaResponse(module.Value!.Name, module.Value.Columns));

        return Task.FromResult(result);
    }
}
