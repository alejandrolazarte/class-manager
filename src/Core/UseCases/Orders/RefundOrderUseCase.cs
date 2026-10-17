using ClassManager.Core.Abstractions.Fees;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Orders;
using ClassManager.Core.Domain.Products;

namespace ClassManager.Core.UseCases.Orders;

public sealed record RefundLine(Guid LineId, int? Quantity, bool ReturnToStock);

public sealed record RefundOrderRequest(IReadOnlyList<RefundLine>? Lines)
{
    public RefundOrderCommand ToCommand(Guid orderId) => new(orderId, Lines);
}

public sealed record RefundOrderCommand(Guid OrderId, IReadOnlyList<RefundLine>? Lines) : ICommand;

public sealed class RefundOrderUseCase(
    IOrderRepository orderRepository,
    IClientRepository clientRepository,
    IClassGroupRepository classGroupRepository,
    IClassPackPurchaseRepository purchaseRepository,
    IClassBalanceService classBalanceService,
    IProductRepository productRepository,
    IStockMovementRepository stockMovementRepository,
    IUnitOfWork unitOfWork,
    IAccessScopes accessScopes,
    ICurrentMember currentMember,
    TimeProvider timeProvider)
    : IUseCase<RefundOrderCommand, OrderResponse>
{
    public async Task<Result<OrderResponse>> ExecuteAsync(RefundOrderCommand command, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetForUpdateAsync(command.OrderId, cancellationToken);
        if (order is null || !await OrderAccess.CanReachAsync(accessScopes, clientRepository, order, cancellationToken))
        {
            return OrderFailures.NotFound();
        }

        var refundLines = command.Lines ?? [];
        if (refundLines.Count == 0 || refundLines.DistinctBy(refundLine => refundLine.LineId).Count() != refundLines.Count)
        {
            return OrderFailures.LineNotFound();
        }

        var access = await currentMember.GetAccessAsync(cancellationToken);
        var now = timeProvider.GetUtcNow();
        foreach (var refundLine in refundLines)
        {
            var line = order.FindLine(refundLine.LineId);
            if (line is null)
            {
                return OrderFailures.LineNotFound();
            }

            var refund = line.Kind == OrderLineKind.ClassPack
                ? await OrderRefunds.RefundClassesAsync(order, line, purchaseRepository, classBalanceService, cancellationToken)
                : await RefundProductAsync(order, line, refundLine, access?.UserId, now, cancellationToken);
            if (refund.IsFailure)
            {
                return refund.Error!;
            }
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return await OrderResponses.OfAsync(order, clientRepository, classGroupRepository, cancellationToken);
    }

    private async Task<Result> RefundProductAsync(
        Order order,
        OrderLine line,
        RefundLine refundLine,
        Guid? userId,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var refund = order.RefundProductUnits(line, refundLine.Quantity);
        if (refund.IsFailure)
        {
            return refund;
        }

        var product = await productRepository.GetByIdAsync(line.ProductId!.Value, cancellationToken);
        if (refundLine.ReturnToStock && product is { TracksStock: true })
        {
            stockMovementRepository.Add(StockMovement.Return(line.ProductVariantId!.Value, refundLine.Quantity!.Value, order.Id, userId, now));
        }

        return Result.Success();
    }
}
