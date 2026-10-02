using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Clients;
using ClassManager.Core.Domain.Fees;
using ClassManager.Core.Domain.Orders;
using ClassManager.Core.Domain.Products;

namespace ClassManager.Core.UseCases.Orders;

public sealed record CounterSaleLine(Guid? ClassPackId, Guid? ProductVariantId, int? Quantity, decimal? UnitPrice);

public sealed record CreateCounterSaleCommand(
    Guid? ClientId,
    IReadOnlyList<CounterSaleLine>? Lines,
    PaymentMethod? Method,
    DateOnly? PaidOn,
    string? Notes,
    bool IsDelivered,
    DeliveryMethod? Delivery = null,
    Guid? DeliveryClassGroupId = null);

public sealed class CreateCounterSaleUseCase(
    IClientRepository clientRepository,
    IClassPackRepository classPackRepository,
    IClassPackPurchaseRepository purchaseRepository,
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
    TimeProvider timeProvider,
    IAccessScopes accessScopes,
    ICurrentMember currentMember)
    : IUseCase<CreateCounterSaleCommand, OrderResponse>
{
    private const string ClientNotFoundMessage = "The client does not exist.";

    public async Task<Result<OrderResponse>> ExecuteAsync(CreateCounterSaleCommand command, CancellationToken cancellationToken)
    {
        Client? client = null;
        if (command.ClientId is { } clientId)
        {
            client = await clientRepository.GetByIdAsync(clientId, cancellationToken);
            if (client is null)
            {
                return Result.NotFound<OrderResponse>(ClientNotFoundMessage, ClientErrorCodes.NotFound);
            }

            if (!await AccessRules.CanReachClientsAsync(accessScopes, clientRepository, [client.Id], Permissions.Orders.ViewAll, cancellationToken))
            {
                return AccessRules.NotYours();
            }
        }

        var requestedLines = (command.Lines ?? [])
            .Select(line => new RequestedOrderLine(line.ClassPackId, line.ProductVariantId, line.Quantity, line.UnitPrice))
            .ToList();
        var draft = await OrderDrafts.BuildAsync(requestedLines, OrderAudience.Team, classPackRepository, productRepository, cancellationToken);
        if (draft.IsFailure)
        {
            return draft.Error!;
        }

        var today = await businessCalendar.TodayAsync(cancellationToken);
        var now = timeProvider.GetUtcNow();
        var access = await currentMember.GetAccessAsync(cancellationToken);
        var order = Order.CounterSale(
            client?.Id, draft.Value!.Lines, command.Method, command.PaidOn ?? today, command.Notes, command.IsDelivered, today, access?.UserId, now);
        if (order.IsFailure)
        {
            return order.Error!;
        }

        if (command.Delivery is not null && order.Value!.AwaitsPickup)
        {
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

        var credit = OrderPayments.CreditClassPacks(order.Value!, draft.Value.Packs, purchaseRepository, today, now, access?.UserId);
        if (credit.IsFailure)
        {
            return credit.Error!;
        }

        foreach (var line in order.Value!.Lines.Where(line => line.Kind == OrderLineKind.Product && draft.Value.ProductOf(line).TracksStock))
        {
            stockMovementRepository.Add(StockMovement.Sale(line.ProductVariantId!.Value, line.Quantity, order.Value.Id, access?.UserId, now));
        }

        order.Value.AssignNumber(await orderNumbers.TakeNextAsync(cancellationToken));
        orderRepository.Add(order.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return await OrderResponses.OfAsync(order.Value, clientRepository, classGroupRepository, cancellationToken);
    }
}
