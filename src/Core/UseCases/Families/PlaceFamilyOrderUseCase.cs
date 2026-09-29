using ClassManager.Core.Abstractions.Notifications;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Orders;
using ClassManager.Core.Domain.Products;
using ClassManager.Core.UseCases.Orders;

namespace ClassManager.Core.UseCases.Families;

public sealed record FamilyOrderLine(Guid? ClassPackId, Guid? ProductVariantId, int? Quantity);

public sealed record PlaceFamilyOrderCommand(
    IReadOnlyList<FamilyOrderLine>? Lines,
    DeliveryMethod? Delivery = null,
    Guid? DeliveryClassGroupId = null);

public sealed class PlaceFamilyOrderUseCase(
    IFamilyAccess familyAccess,
    IClassPackRepository classPackRepository,
    IProductRepository productRepository,
    IStockMovementRepository stockMovementRepository,
    IStockLock stockLock,
    IOrderRepository orderRepository,
    IClassGroupRepository classGroupRepository,
    IStudentRepository studentRepository,
    IEnrollmentRepository enrollmentRepository,
    IUnitOfWork unitOfWork,
    IBusinessCalendarService businessCalendar,
    IOrderNotificationService orderNotifications,
    TimeProvider timeProvider)
    : IUseCase<PlaceFamilyOrderCommand, FamilyOrderResponse>
{
    public async Task<Result<FamilyOrderResponse>> ExecuteAsync(PlaceFamilyOrderCommand command, CancellationToken cancellationToken)
    {
        var access = await familyAccess.GetAsync(cancellationToken);
        if (access is null)
        {
            return FamilyFailures.NoAccess();
        }

        var requestedLines = (command.Lines ?? [])
            .Select(line => new RequestedOrderLine(line.ClassPackId, line.ProductVariantId, line.Quantity, null))
            .ToList();
        var draft = await OrderDrafts.BuildAsync(requestedLines, OrderAudience.Family, classPackRepository, productRepository, cancellationToken);
        if (draft.IsFailure)
        {
            return draft.Error!;
        }

        var now = timeProvider.GetUtcNow();
        var order = Order.Request(access.ClientId, draft.Value!.Lines, access.UserId, now);
        if (order.IsFailure)
        {
            return order.Error!;
        }

        if (command.Delivery is not null && order.Value!.HasProducts)
        {
            var today = await businessCalendar.TodayAsync(cancellationToken);
            var delivery = await DeliveryClasses.ChooseAsync(
                order.Value, command.Delivery, command.DeliveryClassGroupId, today, studentRepository, enrollmentRepository, classGroupRepository, cancellationToken);
            if (delivery.IsFailure)
            {
                return delivery.Error!;
            }
        }

        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);
        var stockProblem = await OrderDrafts.LockAndCheckStockAsync(draft.Value, stockLock, stockMovementRepository, cancellationToken);
        if (stockProblem is not null)
        {
            return stockProblem;
        }

        if (await orderRepository.CountRequestedByClientAsync(access.ClientId, cancellationToken) >= Order.MaximumOpenRequestsPerFamily)
        {
            return FamilyFailures.TooManyOpenOrders();
        }

        foreach (var line in order.Value!.Lines.Where(line => line.Kind == OrderLineKind.Product && draft.Value.ProductOf(line).TracksStock))
        {
            stockMovementRepository.Add(StockMovement.Reserve(line.ProductVariantId!.Value, line.Quantity, order.Value.Id, access.UserId, now));
        }

        orderRepository.Add(order.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        await orderNotifications.OrderPlacedAsync(order.Value, cancellationToken);

        return (await FamilyOrderResponses.OfAsync([order.Value], classGroupRepository, cancellationToken))[0];
    }
}
