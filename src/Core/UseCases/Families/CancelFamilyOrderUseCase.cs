using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.UseCases.Orders;

namespace ClassManager.Core.UseCases.Families;

public sealed record CancelFamilyOrderCommand(Guid OrderId);

public sealed class CancelFamilyOrderUseCase(
    IFamilyAccess familyAccess,
    IOrderRepository orderRepository,
    IClassGroupRepository classGroupRepository,
    IStockMovementRepository stockMovementRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IUseCase<CancelFamilyOrderCommand, FamilyOrderResponse>
{
    public async Task<Result<FamilyOrderResponse>> ExecuteAsync(CancelFamilyOrderCommand command, CancellationToken cancellationToken)
    {
        var access = await familyAccess.GetAsync(cancellationToken);
        if (access is null)
        {
            return FamilyFailures.NoAccess();
        }

        var order = await orderRepository.GetForUpdateAsync(command.OrderId, cancellationToken);
        if (order is null || order.ClientId != access.ClientId)
        {
            return FamilyFailures.OrderNotFound();
        }

        var now = timeProvider.GetUtcNow();
        var cancellation = order.Cancel(access.UserId, now);
        if (cancellation.IsFailure)
        {
            return cancellation.Error!;
        }

        await OrderPayments.ReleaseReservationsAsync(order, stockMovementRepository, access.UserId, now, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return (await FamilyOrderResponses.OfAsync([order], classGroupRepository, cancellationToken))[0];
    }
}
