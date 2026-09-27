using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.ImportExport.Parsing;

namespace ClassManager.Core.UseCases.ImportExport;

public sealed record ImportFileCommand(string Module, Stream File);

public sealed class ImportFileUseCase(IEnumerable<IImportModule> modules, IImportParser parser, IUnitOfWork unitOfWork)
    : IUseCase<ImportFileCommand, ImportReport>
{
    private const string ConcurrentChangeMessage = "The data changed while importing. Preview the file again.";

    public async Task<Result<ImportReport>> ExecuteAsync(ImportFileCommand command, CancellationToken cancellationToken)
    {
        var planned = await ImportFlow.PlanAsync(modules, parser, command.Module, command.File, cancellationToken);
        if (planned.IsFailure)
        {
            return planned.Error!;
        }

        var (report, plan) = planned.Value;
        if (report.Summary.Valid == 0)
        {
            return report;
        }

        plan.AddValidRows();
        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (UniqueConstraintViolationException)
        {
            return Result.Conflict<ImportReport>(ConcurrentChangeMessage, ImportUseCaseErrorCodes.ConcurrentChange);
        }

        return report;
    }
}
