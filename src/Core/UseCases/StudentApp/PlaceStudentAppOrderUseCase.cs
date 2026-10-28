using ClassManager.Core.Abstractions.Fees;
using ClassManager.Core.Abstractions.Notifications;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Orders;
using ClassManager.Core.Domain.Products;
using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Core.UseCases.Orders;
using ClassManager.Core.UseCases.Products;
using ClassManager.Subscriptions.Access;

namespace ClassManager.Core.UseCases.StudentApp;

public sealed record StudentAppOrderLine(long? ClassPackId, long? ProductVariantId, int? Quantity);

public sealed record PlaceStudentAppOrderCommand(
    IReadOnlyList<StudentAppOrderLine>? Lines,
    DeliveryMethod? Delivery = null,
    long? DeliveryClassGroupId = null) : ICommand;

public sealed class PlaceStudentAppOrderUseCase(
    IStudentAppFamilyService studentAppFamily,
    IClassPackRepository classPackRepository,
    IProductRepository productRepository,
    IStockMovementRepository stockMovementRepository,
    IStockLock stockLock,
    IOrderNumbers orderNumbers,
    IOrderRepository orderRepository,
    IClassGroupRepository classGroupRepository,
    IStudentRepository studentRepository,
    IEnrollmentRepository enrollmentRepository,
    IUnitOfWork unitOfWork,
    IBusinessCalendarService businessCalendar,
    IBusinessCurrencyService businessCurrency,
    IOrderNotificationService orderNotifications,
    TimeProvider timeProvider,
    IFeatureAccess featureAccess)
    : IUseCase<PlaceStudentAppOrderCommand, StudentAppOrderResponse>
{
    public async Task<Result<StudentAppOrderResponse>> ExecuteAsync(PlaceStudentAppOrderCommand command, CancellationToken cancellationToken)
    {
        var family = await studentAppFamily.GetAsync(cancellationToken);
        if (family is null)
        {
            return StudentAppFailures.NoAccess();
        }

        if (!family.CanBuy)
        {
            return StudentAppFailures.BuyerMustBeAdult();
        }

        var access = family.Access;

        var requestedLines = (command.Lines ?? [])
            .Select(line => new RequestedOrderLine(line.ClassPackId, line.ProductVariantId, line.Quantity, null))
            .ToList();
        if (requestedLines.Any(line => line.ProductVariantId is not null)
            && !(await featureAccess.GetCurrentAsync(cancellationToken)).Has(Features.Shop))
        {
            return ProductFailures.VariantNotFound();
        }

        var draft = await OrderDrafts.BuildAsync(requestedLines, OrderAudience.Student, classPackRepository, productRepository, cancellationToken);
        if (draft.IsFailure)
        {
            return draft.Error!;
        }

        var now = timeProvider.GetUtcNow();
        var currencyCode = await businessCurrency.CurrencyCodeAsync(cancellationToken);
        var order = Order.Request(access.ClientId, draft.Value!.Lines, currencyCode, access.UserId, now);
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

        if (await orderRepository.CountUnpaidRequestsByClientAsync(access.ClientId, cancellationToken) >= Order.MaximumOpenRequestsPerClient)
        {
            return StudentAppFailures.TooManyOpenOrders();
        }

        order.Value!.AssignNumber(await orderNumbers.TakeNextAsync(cancellationToken));
        orderRepository.Add(order.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        foreach (var line in order.Value.Lines.Where(line => line.Kind == OrderLineKind.Product && draft.Value.ProductOf(line).TracksStock))
        {
            stockMovementRepository.Add(StockMovement.Reserve(line.ProductVariantId!.Value, line.Quantity, order.Value.Id, access.UserId, now));
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        await orderNotifications.OrderPlacedAsync(order.Value, cancellationToken);

        return (await StudentAppOrderResponses.OfAsync([order.Value], classGroupRepository, cancellationToken))[0];
    }
}
