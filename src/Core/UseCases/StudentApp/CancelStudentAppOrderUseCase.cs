using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.UseCases.Orders;

namespace ClassManager.Core.UseCases.StudentApp;

public sealed record CancelStudentAppOrderCommand(Guid OrderId);

public sealed class CancelStudentAppOrderUseCase(
    IStudentAppAccess studentAppAccess,
    IOrderRepository orderRepository,
    IClassGroupRepository classGroupRepository,
    IStockMovementRepository stockMovementRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IUseCase<CancelStudentAppOrderCommand, StudentAppOrderResponse>
{
    public async Task<Result<StudentAppOrderResponse>> ExecuteAsync(CancelStudentAppOrderCommand command, CancellationToken cancellationToken)
    {
        var access = await studentAppAccess.GetAsync(cancellationToken);
        if (access is null)
        {
            return StudentAppFailures.NoAccess();
        }

        var order = await orderRepository.GetForUpdateAsync(command.OrderId, cancellationToken);
        if (order is null || order.ClientId != access.ClientId)
        {
            return StudentAppFailures.OrderNotFound();
        }

        var now = timeProvider.GetUtcNow();
        var cancellation = order.Cancel(access.UserId, now);
        if (cancellation.IsFailure)
        {
            return cancellation.Error!;
        }

        await OrderPayments.ReleaseReservationsAsync(order, stockMovementRepository, access.UserId, now, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return (await StudentAppOrderResponses.OfAsync([order], classGroupRepository, cancellationToken))[0];
    }
}
