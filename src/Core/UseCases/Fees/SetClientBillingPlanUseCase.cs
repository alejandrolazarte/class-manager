using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Clients;
using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.UseCases.Fees;

public sealed record SetClientBillingPlanRequest(BillingPlanKind? Kind, decimal? CustomFee, string? EffectiveFrom)
{
    public SetClientBillingPlanCommand ToCommand(Guid clientId) => new(clientId, Kind, CustomFee, EffectiveFrom);
}

public sealed record SetClientBillingPlanCommand(Guid ClientId, BillingPlanKind? Kind, decimal? CustomFee, string? EffectiveFrom);

public sealed class SetClientBillingPlanUseCase(
    IClientRepository clientRepository,
    IFeeScheduleRepository feeScheduleRepository,
    IUnitOfWork unitOfWork,
    IBusinessCalendarService businessCalendar,
    TimeProvider timeProvider)
    : IUseCase<SetClientBillingPlanCommand, ClientBillingResponse>
{
    private const string ClientNotFoundMessage = "The client does not exist.";

    public async Task<Result<ClientBillingResponse>> ExecuteAsync(SetClientBillingPlanCommand command, CancellationToken cancellationToken)
    {
        var client = await clientRepository.GetByIdAsync(command.ClientId, cancellationToken);
        if (client is null)
        {
            return Result.NotFound<ClientBillingResponse>(ClientNotFoundMessage, ClientErrorCodes.NotFound);
        }

        var today = await businessCalendar.TodayAsync(cancellationToken);
        var effectiveFrom = EffectiveMonth.Resolve(command.EffectiveFrom, today, nameof(SetClientBillingPlanCommand.EffectiveFrom));
        if (effectiveFrom.IsFailure)
        {
            return effectiveFrom.Error!;
        }

        var change = ClientBillingPlanChange.Create(client.Id, effectiveFrom.Value!, command.Kind, command.CustomFee, today, timeProvider.GetUtcNow());
        if (change.IsFailure)
        {
            return change.Error!;
        }

        var existingChange = await feeScheduleRepository.FindClientPlanChangeForUpdateAsync(client.Id, change.Value!.EffectiveFrom, cancellationToken);
        if (existingChange is null)
        {
            feeScheduleRepository.Add(change.Value);
        }
        else
        {
            existingChange.ReplaceWith(change.Value);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var changes = await feeScheduleRepository.ListClientPlanChangesAsync([client.Id], cancellationToken);
        return ClientBillingResponse.From(changes, today);
    }
}
