using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Storage;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Audit;
using ClassManager.Core.Common;
using ClassManager.Core.UseCases.Fees;

namespace ClassManager.Core.UseCases.ClassPacks;

public sealed record DeleteClassPackPriceChangeCommand(long ClassPackId, [property: AuditPlain] string EffectiveFrom) : ICommand;

public sealed class DeleteClassPackPriceChangeUseCase(
    IClassPackRepository classPackRepository,
    IUnitOfWork unitOfWork,
    IBusinessCalendarService businessCalendar,
    IDocumentStorageService documentStorage)
    : IUseCase<DeleteClassPackPriceChangeCommand, ClassPackResponse>
{
    public async Task<Result<ClassPackResponse>> ExecuteAsync(DeleteClassPackPriceChangeCommand command, CancellationToken cancellationToken)
    {
        var classPack = await classPackRepository.GetByIdAsync(command.ClassPackId, cancellationToken);
        if (classPack is null)
        {
            return ClassPackFailures.NotFound();
        }

        var today = await businessCalendar.TodayAsync(cancellationToken);
        var effectiveFrom = UpcomingMonth.Resolve(command.EffectiveFrom, today, nameof(DeleteClassPackPriceChangeCommand.EffectiveFrom));
        if (effectiveFrom.IsFailure)
        {
            return effectiveFrom.Error!;
        }

        var change = await classPackRepository.FindPriceChangeForUpdateAsync(classPack.Id, effectiveFrom.Value!.FirstDay, cancellationToken);
        if (change is null)
        {
            return ClassPackFailures.PriceChangeNotFound();
        }

        classPackRepository.Remove(change);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await ClassPackResponses.ForAsync(classPack, classPackRepository, businessCalendar, documentStorage, cancellationToken);
    }
}
