using ClassManager.Core.Common;
using ClassManager.ImportExport.Parsing;
using ClassManager.Subscriptions.Access;

namespace ClassManager.Core.UseCases.ImportExport;

public sealed record PreviewImportCommand(string Module, Stream File);

public sealed class PreviewImportUseCase(
    IEnumerable<IImportModule> modules,
    IImportParser parser,
    IFeatureAccess featureAccess,
    IFeatureUsage featureUsage)
    : IUseCase<PreviewImportCommand, ImportReport>
{
    public async Task<Result<ImportReport>> ExecuteAsync(PreviewImportCommand command, CancellationToken cancellationToken)
    {
        var planned = await ImportFlow.PlanAsync(modules, parser, command.Module, command.File, new PlanLimitCheck(featureAccess, featureUsage), cancellationToken);
        return planned.IsFailure ? planned.Error! : planned.Value.Report;
    }
}
