using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Orders;
using ClassManager.Subscriptions.Access;

namespace ClassManager.Core.UseCases.Orders;

public sealed record RestoreOrderPaymentCommand(long OrderId, long PaymentId) : ICommand;

public sealed class RestoreOrderPaymentUseCase(
    IOrderRepository orderRepository,
    IClientRepository clientRepository,
    IClassGroupRepository classGroupRepository,
    IUnitOfWork unitOfWork,
    IAccessScopes accessScopes,
    ICurrentMember currentMember,
    IFeatureAccess featureAccess)
    : IUseCase<RestoreOrderPaymentCommand, OrderResponse>
{
    private const string DeletedPaymentNotFoundMessage = "The deleted payment does not exist.";

    public async Task<Result<OrderResponse>> ExecuteAsync(RestoreOrderPaymentCommand command, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetForUpdateAsync(command.OrderId, cancellationToken);
        if (order is null || !await OrderAccess.CanReachAsync(accessScopes, clientRepository, order, cancellationToken))
        {
            return OrderFailures.NotFound();
        }

        if (await OrderAccess.PlanRefusalAsync(featureAccess, order, cancellationToken) is { } planRefusal)
        {
            return planRefusal;
        }

        var payment = await orderRepository.FindDeletedPaymentForUpdateAsync(order.Id, command.PaymentId, cancellationToken);
        if (payment is null)
        {
            return Result.NotFound<OrderResponse>(DeletedPaymentNotFoundMessage, OrderErrorCodes.PaymentNotFound);
        }

        if (!await OrderAccess.CanUndoPaymentAsync(currentMember, payment, cancellationToken))
        {
            return AccessRules.NotYours();
        }

        var restoration = order.RestorePayment(payment);
        if (restoration.IsFailure)
        {
            return restoration.Error!;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return await OrderResponses.OfAsync(order, clientRepository, classGroupRepository, cancellationToken);
    }
}
