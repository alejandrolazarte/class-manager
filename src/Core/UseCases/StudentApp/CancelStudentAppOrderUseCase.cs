using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Orders;
using ClassManager.Core.UseCases.Orders;

namespace ClassManager.Core.UseCases.StudentApp;

public sealed record CancelStudentAppOrderCommand(long OrderId) : ICommand;

public sealed class CancelStudentAppOrderUseCase(
    IStudentAppFamilyService studentAppFamily,
    IOrderRepository orderRepository,
    IClassGroupRepository classGroupRepository,
    IStockMovementRepository stockMovementRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IUseCase<CancelStudentAppOrderCommand, StudentAppOrderResponse>
{
    public async Task<Result<StudentAppOrderResponse>> ExecuteAsync(CancelStudentAppOrderCommand command, CancellationToken cancellationToken)
    {
        var family = await studentAppFamily.GetAsync(cancellationToken);
        if (family is null)
        {
            return StudentAppFailures.NoAccess();
        }

        var access = family.Access;

        var order = await orderRepository.GetForUpdateAsync(command.OrderId, cancellationToken);
        if (order is null || order.ClientId != access.ClientId || order.Channel != OrderChannel.App || !family.SeesOrderPlacedBy(order.CreatedByUserId))
        {
            return StudentAppFailures.OrderNotFound();
        }

        var now = timeProvider.GetUtcNow();
        var cancellation = order.CancelUnpaid(access.UserId, now);
        if (cancellation.IsFailure)
        {
            return cancellation.Error!;
        }

        await OrderReservations.ReleaseAsync(order, stockMovementRepository, access.UserId, now, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return (await StudentAppOrderResponses.OfAsync([order], classGroupRepository, cancellationToken))[0];
    }
}
