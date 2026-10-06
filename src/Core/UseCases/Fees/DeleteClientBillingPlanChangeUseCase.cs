using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Clients;
using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.UseCases.Fees;

public sealed record DeleteClientBillingPlanChangeCommand(Guid ClientId, string EffectiveFrom) : ICommand;

public sealed class DeleteClientBillingPlanChangeUseCase(
    IClientRepository clientRepository,
    IFeeScheduleRepository feeScheduleRepository,
    IUnitOfWork unitOfWork,
    IBusinessCalendarService businessCalendar,
    TimeProvider timeProvider,
    IAccessScopes accessScopes)
    : IUseCase<DeleteClientBillingPlanChangeCommand, ClientBillingResponse>
{
    private const string ClientNotFoundMessage = "The client does not exist.";
    private const string ChangeNotFoundMessage = "The client has no fee change for that month.";

    public async Task<Result<ClientBillingResponse>> ExecuteAsync(DeleteClientBillingPlanChangeCommand command, CancellationToken cancellationToken)
    {
        var client = await clientRepository.GetByIdAsync(command.ClientId, cancellationToken);
        if (client is null)
        {
            return Result.NotFound<ClientBillingResponse>(ClientNotFoundMessage, ClientErrorCodes.NotFound);
        }

        if (!await MoneyRules.CanCollectFromAsync(accessScopes, clientRepository, client.Id, cancellationToken))
        {
            return AccessRules.NotYours();
        }

        var today = await businessCalendar.TodayAsync(cancellationToken);
        var effectiveFrom = UpcomingMonth.Resolve(command.EffectiveFrom, today, nameof(DeleteClientBillingPlanChangeCommand.EffectiveFrom));
        if (effectiveFrom.IsFailure)
        {
            return effectiveFrom.Error!;
        }

        var change = await feeScheduleRepository.FindClientPlanChangeForUpdateAsync(client.Id, effectiveFrom.Value!.FirstDay, cancellationToken);
        if (change is null)
        {
            return Result.NotFound<ClientBillingResponse>(ChangeNotFoundMessage, FeeErrorCodes.FeeChangeNotFound);
        }

        change.Delete(timeProvider.GetUtcNow());
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var changes = await feeScheduleRepository.ListClientPlanChangesAsync([client.Id], cancellationToken);
        return ClientBillingResponse.From(changes, today);
    }
}
